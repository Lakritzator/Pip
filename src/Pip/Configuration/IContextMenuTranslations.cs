// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using Dapplo.Config.Language;

namespace Pip.Configuration
{
    /// <summary>
    ///     The translations for the context menu
    /// </summary>
    [Language("ContextMenu")]
    public interface IContextMenuTranslations : ILanguage
    {
        /// <summary>
        ///     The translation of the exit entry in the context menu
        /// </summary>
        [DefaultValue("Exit")]
        string Exit { get; }

        /// <summary>
        ///     The translation of the title of the context menu
        /// </summary>
        [DefaultValue("PIP")]
        string Title { get; }
    }
}