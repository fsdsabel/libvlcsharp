using LibVLCSharp.Maui.Platforms.iOS;
using LibVLCSharp.Maui.Shared;
using UIKit;

[assembly: Dependency(typeof(PowerManager))]
namespace LibVLCSharp.Maui.Platforms.iOS
{
    /// <summary>
    /// Power manager.
    /// </summary>
    internal class PowerManager : IPowerManager
    {
        /// <summary>
        /// Gets or sets a value indicating whether the screen should be kept on.
        /// </summary>
        public bool KeepScreenOn
        {
            get => UIApplication.SharedApplication.IdleTimerDisabled;
            set => UIApplication.SharedApplication.IdleTimerDisabled = value;
        }
    }
}
