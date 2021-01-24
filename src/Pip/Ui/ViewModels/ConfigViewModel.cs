// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dapplo.CaliburnMicro;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Translations;
using Pip.Configuration;

namespace Pip.Ui.ViewModels
{
    /// <summary>
    ///     The settings view model is, well... for the settings :)
    ///     It is a conductor where one item is active.
    /// </summary>
    public sealed class ConfigViewModel : Config<IConfigScreen>, IMaintainPosition
    {
        /// <summary>
        ///     Constructor which takes care of exporting the ConfigMenuItem
        /// </summary>
        public ConfigViewModel(
            IEnumerable<Lazy<IConfigScreen>> configScreens,
            IPipTranslations pipTranslations)
        {
            ConfigScreens = configScreens;
            CoreTranslations = pipTranslations;
            ConfigTranslations = pipTranslations;
            PipTranslations = pipTranslations;
        }

        /// <summary>
        ///     The core translations for the view (ok / cancel)
        /// </summary>
        public ICoreTranslations CoreTranslations { get; private set; }

        /// <summary>
        ///     The translations for the config view
        /// </summary>
        public IConfigTranslations ConfigTranslations { get; set; }

        /// <summary>
        ///     The CallINGTranslations (configuration)
        /// </summary>
        public IPipTranslations PipTranslations { get; set; }

        /// <inheritdoc />
        protected override void OnActivate()
        {
            base.OnActivate();
            // automatically update the DisplayName
            PipTranslations.CreateDisplayNameBinding(this, nameof(IPipTranslations.Configuration));
        }
    }
}