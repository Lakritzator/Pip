// Dapplo - building blocks for desktop applications
// Copyright (C) 2017-2019  Dapplo
// 
// For more information see: http://dapplo.net/
// Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
// This file is part of Pip
// 
// Pip is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Pip is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
// 
// You should have a copy of the GNU Lesser General Public License
// along with Pip. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using Autofac;
using Autofac.Features.AttributeFilters;
using Dapplo.Addons;
using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Menu;
using Dapplo.CaliburnMicro.Metro;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.CaliburnMicro.NotifyIconWpf;
using Dapplo.Config.Ini;
using Dapplo.Config.Language;
using Pip.Configuration;
using Pip.Modules;
using Pip.Ui.ViewModels;

namespace Pip
{
    /// <inheritdoc />
    public class PipAddonModule : AddonModule
    {
        /// <inheritdoc />
        protected override void Load(ContainerBuilder builder)
        {
            // All IMenuItem with the context they belong to
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IMenuItem>()
                .As<IMenuItem>()
                .SingleInstance();

            builder
                .Register(c => Language<IContextMenuTranslations>.Create())
                .As<ILanguage>()
                .As<IContextMenuTranslations>()
                .SingleInstance();

            builder
                .Register(c => Language<IPipTranslations>.Create())
                .As<ILanguage>()
                .As<IPipTranslations>()
                .SingleInstance();

            builder
                .Register(c => IniSection<ILogConfiguration>.Create())
                .As<IIniSection>()
                .As<ILogConfiguration>()
                .SingleInstance();

            builder
                .Register(c =>
                {
                    var metroConfiguration = IniSection<IPipConfiguration>.Create();

                    // add specific code
                    var metroThemeManager = c.Resolve<MetroThemeManager>();

                    metroConfiguration.RegisterAfterLoad(iniSection =>
                    {
                        if (!(iniSection is IMetroUiConfiguration metroConfig))
                        {
                            return;
                        }

                        metroThemeManager.ChangeTheme(metroConfig.Theme, metroConfig.ThemeColor);
                    });
                    return metroConfiguration;
                })
                .As<IMetroUiConfiguration>()
                .As<IPipConfiguration>()
                .As<IIniSection>()
                .SingleInstance();

            // All config screens
            builder.RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IConfigScreen>()
                .As<IConfigScreen>()
                .SingleInstance();

            builder
                .RegisterType<ConfigureUiDefaults>()
                .As<IService>()
                .SingleInstance();

            builder
                .RegisterType<ConfigStartup>()
                .As<IService>()
                .SingleInstance();

            builder
                .RegisterType<LoggerStartup>()
                .As<IService>()
                .SingleInstance();

            builder
                .RegisterType<PipService>()
                .As<IService>()
                .SingleInstance();

            builder
                .RegisterType<SystemTrayContextMenuViewModel>()
                .As<ITrayIconViewModel>()
                .WithAttributeFiltering()
                .SingleInstance();

            builder
                .RegisterType<ConfigViewModel>()
                .AsSelf();

            builder
                .RegisterType<ErrorViewModel>()
                .AsSelf();
        }
    }
}
