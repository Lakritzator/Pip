// Copyright (c) Dapplo and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Pip.Ui;

namespace Pip.Modules
{
    public class ThumbnailRegistry
    {
        private readonly Dictionary<IntPtr, ThumbnailForm> _thumbnailForms = new Dictionary<IntPtr, ThumbnailForm>();

        public IDictionary<IntPtr, ThumbnailForm> Thumbnails => _thumbnailForms;
    }
}
