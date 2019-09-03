
using System.ComponentModel;
using Dapplo.Config.Ini;
using Dapplo.Log.LogFile;


namespace Pip.Configuration
{
    /// <summary>
    ///     Store all log specific settings, currently only FileLogger settings are here
    /// </summary>
    [IniSection("Log")]
    [Description("Log configuration")]
    public interface ILogConfiguration : IFileLoggerConfiguration, IIniSection
    {
    }
}