// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.DesktopWindowsManager;
using Dapplo.Windows.DesktopWindowsManager.Structs;
using Dapplo.Windows.Enums;
using Dapplo.Windows.Messages.Enumerations;
using Dapplo.Windows.User32;
using Pip.Configuration;
using Pip.Modules;

namespace Pip.Ui
{
    /// <summary>
    /// This is the Thumbnail which represents the "Picture" in the Picture.
    /// It contains the logic to make the mirroring possible and handles mouse or configuration events. 
    /// </summary>
    public sealed class ThumbnailForm : Form
    {
        private const int HtCaption = 0x2;
        private const int HtBottomRight = 17;
        private const int GripSize = 16;      // Grip size

        private readonly IPipConfiguration _pipConfiguration;
        private readonly LocationPool _locationPool;
        private readonly IntPtr _hWnd;
        private IntPtr _phThumbnail;
        private readonly IDisposable _windowCloseMonitor;
        private readonly IDisposable _windowSizeChangeMonitor;
        private readonly IDisposable _configurationMonitor;
        private readonly NativeRect _thumbnailRect;

        private static readonly string[] PropertiesToMonitor = new[] {nameof(IPipConfiguration.Opacity), nameof(IPipConfiguration.SourceClientAreaOnly) };

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="pipConfiguration">IPipConfiguration</param>
        /// <param name="locationPool">LocationPool used to find a location</param>
        /// <param name="hWnd">IntPtr with the hWnd to mirror</param>
        /// <param name="uiSynchronizationContext">SynchronizationContext used to make it possible to modify the UI</param>
        public ThumbnailForm(IPipConfiguration pipConfiguration, LocationPool locationPool, IntPtr hWnd, SynchronizationContext uiSynchronizationContext)
        {
            _thumbnailRect = locationPool.Pool();
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            _pipConfiguration = pipConfiguration;
            _locationPool = locationPool;

            // Make sure that changes to the settings are applied
            _configurationMonitor = _pipConfiguration.OnPropertyChanged()
                .Where(args => PropertiesToMonitor.Contains(args.PropertyName))
                .Throttle(TimeSpan.FromMilliseconds(200))
                .SubscribeOn(uiSynchronizationContext)
                .ObserveOn(uiSynchronizationContext)
                .Subscribe(args => UpdateThumbnail());

            _hWnd = hWnd;

            // Make sure the PIP closes when the source closes
            _windowCloseMonitor = WinEventHook.Create(WinEvents.EVENT_OBJECT_DESTROY)
                .SubscribeOn(uiSynchronizationContext)
                .ObserveOn(uiSynchronizationContext)
                .Subscribe(info =>
                {
                    if (info.Handle == _hWnd)
                    {
                        Close();
                    }
                });

            // Make sure the thumbnail changes when the window changes
            _windowSizeChangeMonitor = WinEventHook.Create(WinEvents.EVENT_OBJECT_LOCATIONCHANGE)
                .SubscribeOn(uiSynchronizationContext)
                .ObserveOn(uiSynchronizationContext)
                .Subscribe(info =>
                {
                    if (info.Handle == _hWnd)
                    {
                        UpdateThumbnail();
                    }
                });

            Location = _thumbnailRect.Location;
            Size = _thumbnailRect.Size;
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            Text = "PIP";
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Color.Gray;
            Enabled = false;
            ShowInTaskbar = false;
        }

        /// <summary>
        /// Make the window movable
        /// </summary>
        /// <param name="message">Message</param>
        protected override void WndProc(ref Message message)
        {
            var windowsMessage = (WindowsMessages) message.Msg;
            switch (windowsMessage)
            {
                case WindowsMessages.WM_NCHITTEST:
                    Point pos = new Point(message.LParam.ToInt32());
                    pos = this.PointToClient(pos);
                    if (pos.X >= this.ClientSize.Width - GripSize && pos.Y >= this.ClientSize.Height - GripSize)
                    {
                        message.Result = (IntPtr)HtBottomRight;
                        return;
                    }
                    message.Result = (IntPtr)HtCaption;
                    return;
                // As we make the total window the "non client" area, we check the Window message NC RBUTTON up
                case WindowsMessages.WM_NCRBUTTONUP:
                    Close();
                    return;
                case WindowsMessages.WM_SIZE:
                    UpdateThumbnail();
                    break;
            }
            base.WndProc(ref message);
        }

        /// <summary>
        /// Make sure the monitoring is disabled and the Thumbnail is unregistered
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnClosed(EventArgs e)
        {
            _locationPool.Return(_thumbnailRect);
            _windowSizeChangeMonitor.Dispose();
            _windowCloseMonitor.Dispose();
            _configurationMonitor.Dispose();
            Dwm.DwmUnregisterThumbnail(_phThumbnail);
            base.OnClosed(e);
        }

        /// <summary>
        /// Create the thumbnail if the form is shown
        /// </summary>
        /// <param name="e">EventArgs</param>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            if (_phThumbnail == IntPtr.Zero)
            {
                var result = Dwm.DwmRegisterThumbnail(Handle, _hWnd, out _phThumbnail);
                if (result.Failed())
                {
                    Close();
                    return;
                }
                UpdateThumbnail();
            }
            User32Api.BringWindowToTop(Handle);
        }

        /// <summary>
        /// Change the thumbnail settings
        /// </summary>
        private void UpdateThumbnail()
        {
            // Retrieve the current information about the window, this could changed
            var interopWindow = InteropWindowFactory.CreateFor(_hWnd).Fill();
            var sourceBounds = interopWindow.Info.Value.Bounds;

            Opacity = Math.Max((byte)0x01, _pipConfiguration.Opacity) / 255d;
            // Prepare the displaying of the Thumbnail
            var props = new DwmThumbnailProperties
            {
                Opacity = _pipConfiguration.Opacity,
                Visible = true,
                SourceClientAreaOnly = _pipConfiguration.SourceClientAreaOnly,
                // This is the size of the DMW Thumbnail
                Destination = new NativeRect(0, 0, Width, Height),
                // Here it would be possible to select only a part of the window, but this is slightly tricky of someone resizes the window
                Source = new NativeRect(0,0, sourceBounds.Width, sourceBounds.Height)
            };
            Dwm.DwmUpdateThumbnailProperties(_phThumbnail, ref props);
        }
    }
}
