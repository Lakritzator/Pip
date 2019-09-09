using System.Collections.Generic;
using Dapplo.Windows.Common.Structs;
using Dapplo.Windows.User32;

namespace Pip.Modules
{
    public class LocationPool
    {
        private readonly List<NativeRect> _availableLocations = new List<NativeRect>();

        public LocationPool()
        {
            var screenBounds = DisplayInfo.ScreenBounds;
            var pipNativeSize = new NativeSize(screenBounds.Width / 5, screenBounds.Height / 5);
            for (int i = 0; i < 5; i++)
            {
                var pipLocation = new NativePoint(screenBounds.Width - pipNativeSize.Width, i*pipNativeSize.Height);
                var currentSize = new NativeRect(pipLocation, pipNativeSize);
                _availableLocations.Add(currentSize);
            }
        }

        public bool HasAvailable => _availableLocations.Count > 0;

        public NativeRect Pool()
        {
            try
            {
                return _availableLocations[0];
            }
            finally
            {
                _availableLocations.RemoveAt(0);
            }
        }

        public void Return(NativeRect pooledNativeRect)
        {
            _availableLocations.Add(pooledNativeRect);
            _availableLocations.Sort((rect1, rect2) => rect1.Y - rect2.Y);
        }

    }
}
