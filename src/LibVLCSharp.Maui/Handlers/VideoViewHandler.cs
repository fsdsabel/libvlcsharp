using LibVLCSharp.Maui.Shared;
using Microsoft.Maui.Handlers;

namespace LibVLCSharp.Maui.Handlers
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VideoViewHandler
    {
        /// <summary>
        /// 
        /// </summary>
        public static PropertyMapper<IVideoView, VideoViewHandler> VideoViewMapper = new PropertyMapper<IVideoView, VideoViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(IVideoView.MediaPlayer)] = MapMediaPlayer
        };

        /// <summary>
        /// 
        /// </summary>
        public VideoViewHandler() : base(VideoViewMapper)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        public VideoViewHandler(PropertyMapper? mapper = null) : base(mapper ?? VideoViewMapper)
        {

        }
    }
}
