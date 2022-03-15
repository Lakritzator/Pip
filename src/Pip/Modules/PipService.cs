// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading;
using Dapplo.Addons;
using Dapplo.Windows.Desktop;
using Dapplo.Windows.Input.Keyboard;
using Pip.Configuration;
using Pip.Ui;

namespace Pip.Modules
{
    [Service(nameof(PipService), TaskSchedulerName = "ui")]
    public class PipService : IStartup
    {
        private readonly IPipConfiguration _pipConfiguration;
        private readonly LocationPool _locationPool;
        private readonly ThumbnailRegistry _thumbnailRegistry;

        public PipService(IPipConfiguration pipConfiguration, LocationPool locationPool, ThumbnailRegistry thumbnailRegistry)
        {
            _pipConfiguration = pipConfiguration;
            _locationPool = locationPool;
            _thumbnailRegistry = thumbnailRegistry;
        }
        public void Startup()
        {
            var uiSynchronizationContext = SynchronizationContext.Current;

            var keyHandler = new KeyCombinationHandler(_pipConfiguration.HotKey)
            {
                CanRepeat = false,
                IsPassThrough = false
            };

            KeyboardHook.KeyboardEvents
                .Where(keyHandler)
                .SubscribeOn(uiSynchronizationContext)
                .ObserveOn(uiSynchronizationContext)
                .Subscribe(keyboardHookEventArgs =>
            {
                // Get the current active window
                var pipSource = InteropWindowQuery.GetForegroundWindow();
                while (pipSource.GetParent() != IntPtr.Zero)
                {
                    pipSource = InteropWindowFactory.CreateFor(pipSource.GetParent());
                }

                // If there is already a form, close it and remove it from the dictionary
                if (_thumbnailRegistry.Thumbnails.TryGetValue(pipSource.Handle, out var thumbnailForm))
                {
                    thumbnailForm.Close();
                    _thumbnailRegistry.Thumbnails.Remove(pipSource.Handle);
                    return;
                }

                // Check if we have a location available
                if (!_locationPool.HasAvailable)
                {
                    return;
                }
                thumbnailForm = new ThumbnailForm(_pipConfiguration, _locationPool, pipSource.Handle, uiSynchronizationContext, _thumbnailRegistry);
                _thumbnailRegistry.Thumbnails[pipSource.Handle] = thumbnailForm;
                thumbnailForm.Show();
            });
        }
    }
}
