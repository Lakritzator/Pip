using Dapplo.CaliburnMicro.Configuration;
using Dapplo.CaliburnMicro.Extensions;
using Pip.Configuration;

namespace Pip.Ui.ViewModels
{
    /// <summary>
    ///     The fiddler config ViewModel
    /// </summary>
    public sealed class PipConfigViewModel : SimpleConfigScreen
    {

        /// <summary>
        ///     construct the ViewModel
        /// </summary>
        /// <param name="pipConfiguration">IPipConfiguration</param>
        /// <param name="pipTranslations">IPipTranslations</param>
        public PipConfigViewModel(
            IPipConfiguration pipConfiguration,
            IPipTranslations pipTranslations)
        {
            Id = "C_Pip";
            PipConfiguration = pipConfiguration;
            PipTranslations = pipTranslations;
            pipTranslations.CreateDisplayNameBinding(this, nameof(IPipTranslations.Title));
        }

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IPipConfiguration PipConfiguration { get; }

        /// <summary>
        ///     Used from the View
        /// </summary>
        public IPipTranslations PipTranslations { get; }

    }
}