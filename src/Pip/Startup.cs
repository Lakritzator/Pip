// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;
using Autofac;
using Caliburn.Micro;
using Dapplo.Addons.Bootstrapper;
using Dapplo.CaliburnMicro.Dapp;
using Dapplo.Config.Ini.Converters;
using Dapplo.Log;
using Dapplo.Log.Loggers;
using Pip.Ui.ViewModels;
#if DEBUG

#else
using Dapplo.Log.LogFile;
#endif


namespace Pip
{
    /// <summary>
    ///     This takes care or starting the Application
    /// </summary>
    public static class Startup
    {
        private static readonly LogSource Log = new LogSource();

        /// <summary>
        ///     Start the application
        /// </summary>
        [STAThread]
        public static int Main()
        {
#if DEBUG
            // Initialize a debug logger for Dapplo packages
            LogSettings.RegisterDefaultLogger<DebugLogger>(LogLevels.Verbose);
#else
            LogSettings.RegisterDefaultLogger<ForwardingLogger>(LogLevels.Debug);
#endif

            // TODO: Set via build
            StringEncryptionTypeConverter.RgbIv = "dlgjowejgogkklwj";
            StringEncryptionTypeConverter.RgbKey = "lsjvkwhvwujkagfauguwcsjgu2wueuff";

            Log.Info().WriteLine("Windows version {0}", Environment.OSVersion.Version);
            var applicationConfig = ApplicationConfigBuilder
                .Create()
                .WithApplicationName("Pip")
                .WithMutex("214222E8-9878-451F-BF9B-B788F591E7DD")
                .WithCaliburnMicro()
                .WithoutCopyOfEmbeddedAssemblies()
#if NET471
                .WithoutCopyOfAssembliesToProbingPath()
#endif
                .BuildApplicationConfig();

            var application = new Dapplication(applicationConfig)
            {
                ShutdownMode = ShutdownMode.OnExplicitShutdown
            };

            // Prevent multiple instances
            if (application.WasAlreadyRunning)
            {
                Log.Warn().WriteLine("{0} was already running.", applicationConfig.ApplicationName);
                // Don't start the dapplication, exit with -1
                application.Shutdown(-1);
                return -1;
            }

            RegisterErrorHandlers(application);

            application.Run();
            return 0;
        }

        /// <summary>
        /// Make sure all exception handlers are hooked
        /// </summary>
        /// <param name="application">Dapplication</param>
        private static void RegisterErrorHandlers(Dapplication application)
        {
            application.OnUnhandledAppDomainException += (exception, b) => DisplayErrorViewModel(exception);
            application.OnUnhandledDispatcherException += DisplayErrorViewModel;
            application.OnUnhandledTaskException += DisplayErrorViewModel;
        }

        /// <summary>
        /// Show the exception
        /// </summary>
        /// <param name="exception">Exception</param>
        private static void DisplayErrorViewModel(Exception exception)
        {
            var windowManager = Dapplication.Current.Bootstrapper.Container.Resolve<IWindowManager>();
            var errorViewModel = Dapplication.Current.Bootstrapper.Container.Resolve<ErrorViewModel>();
            if (windowManager == null || errorViewModel == null)
            {
                return;
            }

            errorViewModel.SetExceptionToDisplay(exception);
            windowManager.ShowWindow(errorViewModel);
        }
    }
}