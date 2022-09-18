using LibVLCSharp.Maui.Shared;
using Microsoft.Maui.Handlers;

namespace LibVLCSharp.Maui.Handlers
{
    public partial class VideoViewHandler : ViewHandler<IVideoView, LibVLCSharp.Platforms.Android.VideoView>
    {
        protected override LibVLCSharp.Platforms.Android.VideoView CreatePlatformView() 
            => new LibVLCSharp.Platforms.Android.VideoView(Context);

        public static void MapMediaPlayer(VideoViewHandler handler, IVideoView videoView)
        {
            if (handler.PlatformView != null)
            {
                handler.PlatformView.MediaPlayer = videoView.MediaPlayer;
                handler.PlatformView.TriggerLayoutChangeListener();
                if (!videoView.IsInitialized)
                {
                    videoView.OnInitialized(new InitializedEventArgs(new string[0]));
                }
            }
        }
    }
}
