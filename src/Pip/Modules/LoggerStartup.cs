
using Dapplo.Addons;
using Dapplo.CaliburnMicro;
using Dapplo.Log;
using Pip.Configuration;

#if !DEBUG
using Dapplo.Log.LogFile;
#endif

namespace Pip.Modules
{
    /// <summary>
    ///     Initialize the logging
    /// </summary>
    [Service(nameof(LoggerStartup), nameof(CaliburnServices.ConfigurationService))]
    public class LoggerStartup : IStartup
    {
        private readonly ILogConfiguration _logConfiguration;

        /// <inheritdoc />
        public LoggerStartup(ILogConfiguration logConfiguration)
        {
            _logConfiguration = logConfiguration;
        }

        /// <summary>
        ///     Initialize the logging
        /// </summary>
        public void Startup()
        {
            _logConfiguration.PreFormat = true;
            _logConfiguration.WriteInterval = 100;

            // TODO: Decide on the log level, make available in the UI?
            _logConfiguration.LogLevel = LogLevels.Debug;
#if !DEBUG
            LogSettings.RegisterDefaultLogger<FileLogger>(_logConfiguration);
#endif
        }
    }
}