using LibVLCSharp.Shared.MediaPlayerElement;

namespace LibVLCSharp.Maui.Shared
{
    /// <summary>
    /// Monitors display-related information for an application view.
    /// </summary>
    internal class DisplayInformation : IDisplayInformation
    {
        /// <summary>
        /// Gets the scale factor
        /// </summary>
        public double ScalingFactor => DeviceDisplay.Current.MainDisplayInfo.Density;
    }
}
