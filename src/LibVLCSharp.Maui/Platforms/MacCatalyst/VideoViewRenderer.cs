﻿/*using LibVLCSharp.Shared;
using LibVLCSharp.Maui.Platforms.Mac;
using Foundation;

[assembly: ExportRenderer(typeof(LibVLCSharp.Forms.Shared.VideoView), typeof(VideoViewRenderer))]
namespace LibVLCSharp.Maui.Platforms.Mac
{
    /// <summary>
    /// Xamarin.Forms custom renderer for the Apple VideoView
    /// </summary>
    [Preserve(AllMembers = true)]
    public class VideoViewRenderer : ViewRenderer<LibVLCSharp.Maui.Shared.VideoView, LibVLCSharp.Platforms.Mac.VideoView>
    {
        /// <summary>
        /// Native control management during lifecycle events
        /// </summary>
        /// <param name="e">lifecycle event</param>
        protected override void OnElementChanged(ElementChangedEventArgs<LibVLCSharp.Maui.Shared.VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    SetNativeControl(new LibVLCSharp.Platforms.Mac.VideoView());
                    e.NewElement.MediaPlayerChanging += OnMediaPlayerChanging;
                    if (Control!.MediaPlayer != e.NewElement.MediaPlayer)
                    {
                        OnMediaPlayerChanging(this, new MediaPlayerChangingEventArgs(Control!.MediaPlayer, e.NewElement.MediaPlayer));
                    }
                }
            }

            if (e.OldElement != null)
            {
                e.OldElement.MediaPlayerChanging -= OnMediaPlayerChanging;
            }
        }

        private void OnMediaPlayerChanging(object? sender, MediaPlayerChangingEventArgs e)
        {
            Control.MediaPlayer = e.NewMediaPlayer;
        }
    }
}
*/
