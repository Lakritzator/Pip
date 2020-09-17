using System;
using Autofac.Features.OwnedInstances;
using Caliburn.Micro;
using Dapplo.CaliburnMicro.Extensions;
using Dapplo.CaliburnMicro.Menu;
using MahApps.Metro.IconPacks;
using Pip.Configuration;
using Pip.Ui.ViewModels;

namespace Pip.Ui
{
    /// <summary>
    /// Defines the systemtray config menu item
    /// </summary>
    [Menu("systemtray")]
    public sealed class ConfigMenuItem : ClickableMenuItem
    {
        /// <inheritdoc />
        public ConfigMenuItem(
            IPipTranslations pipTranslations,
            IWindowManager windowManager,
            Func<Owned<ConfigViewModel>> configViewModelFactory)
        {
            Style = MenuItemStyles.Default;
            Id = "B_Config";
            Icon = new PackIconMaterial
            {
                Kind = PackIconMaterialKind.Cog
            };

            ClickAction = item =>
            {
                IsEnabled = false;
                using (var ownedConfigViewModel = configViewModelFactory())
                {
                    windowManager.ShowDialog(ownedConfigViewModel.Value);
                }

                IsEnabled = true;
            };
            pipTranslations.CreateDisplayNameBinding(this, nameof(IPipTranslations.Configuration));
        }
    }
}
