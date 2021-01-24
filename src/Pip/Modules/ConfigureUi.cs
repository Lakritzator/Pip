﻿// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dapplo.Addons;
using Dapplo.CaliburnMicro;

namespace Pip.Modules
{
    /// <inheritdoc />
    [Service(nameof(ConfigureUiDefaults), nameof(CaliburnServices.ConfigurationService), TaskSchedulerName = "ui")]
    public class ConfigureUiDefaults : IStartup
    {
        private readonly ResourceManager _resourceManager;

        /// <inheritdoc />
        public ConfigureUiDefaults(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }

        /// <inheritdoc />
        public void Startup()
        {
            var pipResourceDirectory = new Uri("pack://application:,,,/Pip;component/PipResourceDirectory.xaml", UriKind.RelativeOrAbsolute);
            _resourceManager.AddResourceDictionary(pipResourceDirectory);
        }
    }
}
