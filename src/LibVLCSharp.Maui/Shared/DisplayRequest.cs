﻿using LibVLCSharp.Shared.MediaPlayerElement;

namespace LibVLCSharp.Maui.Shared
{
    /// <summary>
    /// Represents a display request
    /// </summary>
    internal class DisplayRequest : IDisplayRequest
    {
        private IPowerManager PowerManager => DependencyService.Get<IPowerManager>();

        /// <summary>
        /// Activates a display request
        /// </summary>
        public void RequestActive()
        {
            PowerManager.KeepScreenOn = true;
        }

        /// <summary>
        /// Deactivates a display request
        /// </summary>
        public void RequestRelease()
        {
            PowerManager.KeepScreenOn = false;
        }
    }
}
