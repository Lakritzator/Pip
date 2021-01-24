// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Toasts;
using Pip.Ui.ViewModels;

namespace Pip.Modules
{
    [Service(nameof(StartupUserNotify), nameof(CaliburnServices.ToastConductor), TaskSchedulerName = "ui")]
    public class StartupUserNotify : IStartup
    {
        private readonly ToastConductor _toastConductor;
        private readonly StartupReadyToastViewModel _startupReadyToastViewModel;

        public StartupUserNotify(ToastConductor toastConductor, StartupReadyToastViewModel startupReadyToastViewModel)
        {
            _toastConductor = toastConductor;
            _startupReadyToastViewModel = startupReadyToastViewModel;
        }

        /// <inheritdoc />
        public void Startup()
        {
            _toastConductor.ActivateItem(_startupReadyToastViewModel);
        }
    }
}
