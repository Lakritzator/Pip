using System.ComponentModel;
using Dapplo.CaliburnMicro.Metro.Configuration;
using Dapplo.Config.Ini;

namespace Pip.Configuration
{
    /// <summary>
    ///     Store all pip specific settings
    /// </summary>
    [IniSection("Pip")]
    [Description("Pip configuration")]
    public interface IPipConfiguration : IIniSection, IMetroUiConfiguration
    {
        [DefaultValue("Ctrl + Shift + P")]
        string HotKey { get; set; }

        [Description("The transparency of the pip window, 255 is fully opague.")]
        [DefaultValue(255)]
        byte Opacity { get; set; }

        [Description("true to exclude the window border and title, default is false which means the whole window")]
        [DefaultValue(false)]
        bool SourceClientAreaOnly { get; set; }
    }
}