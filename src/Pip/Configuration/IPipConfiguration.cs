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
    }
}