using System.ComponentModel;
using Dapplo.CaliburnMicro.Translations;
using Dapplo.Config.Language;

namespace Pip.Configuration
{
    /// <summary>
    ///     The translations for Pip
    /// </summary>
    [Language("Pip")]
    public interface IPipTranslations : ILanguage, IConfigTranslations, ICoreTranslations
    {
        /// <summary>
        ///     The hotkey used to trigger the PIP functionality
        /// </summary>
        [DefaultValue("Hotkey")]
        string Hotkey { get; }

        /// <summary>
        ///     The title of the application
        /// </summary>
        [DefaultValue("Pip")]
        string Title { get; }

        /// <summary>
        ///     This describes the name of the configuration window and system tray icon
        /// </summary>
        [DefaultValue("Configuration")]
        string Configuration { get; }
    }
}