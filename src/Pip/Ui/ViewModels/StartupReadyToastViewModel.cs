// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dapplo.CaliburnMicro.Toasts.ViewModels;
using Pip.Configuration;

namespace Pip.Ui.ViewModels
{
    /// <inheritdoc />
    public class StartupReadyToastViewModel : ToastBaseViewModel
    {
        private readonly IPipTranslations _pipTranslations;
        private readonly IPipConfiguration _pipConfiguration;

        public StartupReadyToastViewModel(IPipTranslations pipTranslations, IPipConfiguration pipConfiguration)
        {
            _pipTranslations = pipTranslations;
            _pipConfiguration = pipConfiguration;
        }

        /// <summary>
        /// This contains the message for the ViewModel
        /// </summary>
        public string Message => string.Format(_pipTranslations.StartupNotify, string.Join(" + ", _pipConfiguration.HotKey.Select(vkc => vkc.ToString())));
    }
}
