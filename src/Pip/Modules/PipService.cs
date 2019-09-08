using System;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.Addons;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using Pip.Configuration;
using Pip.Ui;

namespace Pip.Modules
{
    [Service(nameof(PipService), TaskSchedulerName = "ui")]
    public class PipService : IStartup
    {
        private readonly IPipConfiguration _pipConfiguration;
        private ThumbnailForm _thumbnailForm;

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
                _thumbnailForm = new ThumbnailForm(_pipConfiguration, pipSource.Handle, uiSynchronizationContext);
 
                _thumbnailForm.Show();
            });
        }
    }
}
