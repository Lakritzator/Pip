// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
using Dapplo.Windows.Input.Enums;
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
                        if (iniSection is IMetroUiConfiguration metroConfig)
                        {
                            metroThemeManager.ChangeTheme(metroConfig.Theme, metroConfig.ThemeColor);
                        }
                        if (iniSection is IPipConfiguration pipConfiguration)
                        {
                            pipConfiguration.HotKey ??= new[] { VirtualKeyCode.LeftControl, VirtualKeyCode.LeftShift, VirtualKeyCode.KeyP };
                        }

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
                .RegisterType<LocationPool>()
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

            builder
                .RegisterType<StartupReadyToastViewModel>()
                .AsSelf();

            builder.RegisterType<StartupUserNotify>()
                .As<IService>()
                .SingleInstance();
        }
    }
}
