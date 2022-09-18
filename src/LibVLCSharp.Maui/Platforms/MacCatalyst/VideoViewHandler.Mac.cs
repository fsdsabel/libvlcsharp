using LibVLCSharp.Maui.Shared;
using Microsoft.Maui.Handlers;

namespace LibVLCSharp.Maui.Handlers
{
    public partial class VideoViewHandler : ViewHandler<IVideoView, LibVLCSharp.Platforms.MacCatalyst.VideoView>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override LibVLCSharp.Platforms.MacCatalyst.VideoView CreatePlatformView() => new LibVLCSharp.Platforms.MacCatalyst.VideoView();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
        /// <param name="videoView"></param>
        public static void MapMediaPlayer(VideoViewHandler handler, IVideoView videoView)
        {
            if (handler.PlatformView != null)
            {
                handler.PlatformView.MediaPlayer = videoView.MediaPlayer;
                videoView.OnInitialized(new InitializedEventArgs(new string[0]));
            }
        }
    }
}
