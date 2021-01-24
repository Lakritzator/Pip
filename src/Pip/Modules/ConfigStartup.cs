// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dapplo.Addons;
using Dapplo.Config.Ini.Converters;

namespace Pip.Modules
{
    /// <summary>
    ///     Initialize the Configuration framework
    /// </summary>
    [Service(nameof(ConfigStartup))]
    public class ConfigStartup : IStartup
    {
        /// <summary>
        ///     Initialize the Configuration framework
        /// </summary>
        public void Startup()
        {
            StringEncryptionTypeConverter.RgbIv = "pgf02URf@h1!f2rA";
            StringEncryptionTypeConverter.RgbKey = "dKjjh@fjh34g8tg$d0o56SDFgFH23eo0";
        }
    }
}