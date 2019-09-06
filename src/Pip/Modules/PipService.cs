using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Threading;
using System.Windows.Forms;
using Dapplo.Addons;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.DesktopWindowsManager;
using Dapplo.Windows.DesktopWindowsManager.Structs;
using Dapplo.Windows.Enums;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using Dapplo.Windows.User32;
using Pip.Configuration;

namespace Pip.Modules
{
    [Service(nameof(PipService), TaskSchedulerName = "ui")]
    public class PipService : IStartup
    {
        private readonly IPipConfiguration _pipConfiguration;
        private Form _thumbnailForm;

        public PipService(IPipConfiguration pipConfiguration)
        {
            _pipConfiguration = pipConfiguration;
        }
        public void Startup()
        {
            var uiSynchronizationContext = SynchronizationContext.Current;
            var keyHandler = new KeyCombinationHandler(VirtualKeyCode.LeftControl, VirtualKeyCode.LeftShift, VirtualKeyCode.KeyP);
            KeyboardHook.KeyboardEvents
                .Where(keyHandler)
                .SubscribeOn(uiSynchronizationContext)
                .ObserveOn(uiSynchronizationContext)
                .Subscribe(keyboardHookEventArgs =>
            {
                // If there is already a form, close it
                if (_thumbnailForm != null)
                {
                    _thumbnailForm.Close();
                    _thumbnailForm = null;
                    return;
                }

                // Get the current active window
                var pipSource = InteropWindowQuery.GetForegroundWindow();
                while (pipSource.GetParent() != IntPtr.Zero)
                {
                    pipSource = InteropWindowFactory.CreateFor(pipSource.GetParent());
                }
                var screenBounds = DisplayInfo.ScreenBounds;
                var pipBounds = new NativeSize(screenBounds.Width / 5, screenBounds.Height / 5);
                var pipLocation = new NativePoint(screenBounds.Width - pipBounds.Width, 0);
                _thumbnailForm = new Form
                {
                    Location = pipLocation,
                    Size = pipBounds,
                    StartPosition = FormStartPosition.Manual,
                    TopMost = true,
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = Color.White,
                    Enabled = false,
                    ShowInTaskbar = false,
                    Cursor = Cursors.Default
                };

                var result = Dwm.DwmRegisterThumbnail(_thumbnailForm.Handle, pipSource.Handle, out var phThumbnail);
                result.ThrowOnFailure();

                // Prepare the displaying of the Thumbnail
                var props = new DwmThumbnailProperties
                {
                    Opacity = _pipConfiguration.Opacity,
                    Visible = true,
                    SourceClientAreaOnly = _pipConfiguration.SourceClientAreaOnly,
                    Destination = new NativeRect(0, 0, pipBounds.Width, pipBounds.Height)
                };
                Dwm.DwmUpdateThumbnailProperties(phThumbnail, ref props);

                // Make sure the PIP closes when the source closes
                var windowMonitor = WinEventHook.Create(WinEvents.EVENT_OBJECT_DESTROY)
                    .SubscribeOn(uiSynchronizationContext)
                    .ObserveOn(uiSynchronizationContext)
                    .Subscribe(info =>
                {
                    if (info.Handle == pipSource.Handle)
                    {
                        _thumbnailForm.Close();
                    }
                });

                _thumbnailForm.Show();
                User32Api.BringWindowToTop(_thumbnailForm.Handle);
                _thumbnailForm.Closed += (sender, args) =>
                {
                    windowMonitor.Dispose();
                    Dwm.DwmUnregisterThumbnail(phThumbnail);
                };
            });
        }
    }
}
