// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Caliburn.Micro;
using Dapplo.CaliburnMicro;

namespace Pip.Ui.ViewModels
{
    /// <summary>
    /// The ViewModel for the errors
    /// </summary>
    public class ErrorViewModel : Screen
    {
        /// <summary>
        /// The Version-Provider to show the current and potential next version
        /// </summary>
        public IVersionProvider VersionProvider { get; }

        /// <summary>
        /// Constructor for the dependencies
        /// </summary>
        /// <param name="versionProvider">IVersionProvider</param>
        public ErrorViewModel(IVersionProvider versionProvider = null)
        {
            VersionProvider = versionProvider;
        }

        /// <summary>
        /// Checks if the current version is the latest
        /// </summary>
        public bool IsMostRecent => VersionProvider?.CurrentVersion?.Equals(VersionProvider?.LatestVersion) ?? true;

        /// <summary>
        /// Set the exception to display
        /// </summary>
        public void SetExceptionToDisplay(Exception exception)
        {
            Stacktrace = exception.ToString();//ToStringDemystified();
            Message = exception.Message;
        }

        /// <summary>
        /// The stacktrace to display
        /// </summary>
        public string Stacktrace { get; set; }

        /// <summary>
        /// The message to display
        /// </summary>
        public string Message { get; set; }
    }
}
