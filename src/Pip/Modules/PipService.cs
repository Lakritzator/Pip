using System;
using System.Drawing;
using System.Windows.Forms;
using Dapplo.Addons;
using Dapplo.Windows.Common.Extensions;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.DesktopWindowsManager;
using Dapplo.Windows.DesktopWindowsManager.Structs;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using Dapplo.Windows.User32;

namespace Pip.Modules
{
    [Service(nameof(PipService), TaskSchedulerName = "ui")]
    public class PipService : IStartup
    {
        private Form _thumbnailForm;

        public void Startup()
        {
            var keyHandler = new KeyCombinationHandler(VirtualKeyCode.LeftControl, VirtualKeyCode.LeftShift, VirtualKeyCode.KeyP);
            KeyboardHook.KeyboardEvents.Where(keyHandler).Subscribe(keyboardHookEventArgs =>
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
                var pipLocation = new NativePoint(screenBounds.Width - pipBounds.Width, screenBounds.Height - pipBounds.Height);
                _thumbnailForm = new Form
                {
                    Location = pipLocation,
                    Size = pipBounds,
                    TopMost = true,
                    FormBorderStyle = FormBorderStyle.None,
                    BackColor = Color.White,
                    Enabled = false,
                    ShowInTaskbar = false,
                    Cursor = Cursors.Default,
                    
                };

                var result = Dwm.DwmRegisterThumbnail(_thumbnailForm.Handle, pipSource.Handle, out var phThumbnail);
                result.ThrowOnFailure();

                // Prepare the displaying of the Thumbnail
                var props = new DwmThumbnailProperties
                {
                    Opacity = 255,
                    Visible = true,
                    SourceClientAreaOnly = false,
                    Destination = new NativeRect(0, 0, pipBounds.Width, pipBounds.Height)
                };
                Dwm.DwmUpdateThumbnailProperties(phThumbnail, ref props);

                _thumbnailForm.Show();
                _thumbnailForm.Closed += (sender, args) => { Dwm.DwmUnregisterThumbnail(phThumbnail); };
            });
        }
    }
}
