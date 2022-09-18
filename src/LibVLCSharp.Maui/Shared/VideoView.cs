using System;
using System.Diagnostics;
using LibVLCSharp.Shared;

namespace LibVLCSharp.Maui.Shared
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVideoView : IView
    {
        /// <summary>
        /// MediaPlayer
        /// </summary>
        LibVLCSharp.Shared.MediaPlayer? MediaPlayer { get; }
        /// <summary>
        /// Called when initialized
        /// </summary>
        void OnInitialized(InitializedEventArgs e);
    }

    /// <summary>
    /// Generic Maui VideoView
    /// </summary>
    public class VideoView : View, IVideoControl, IElementConfiguration<VideoView>, IVideoView
    {
        /// <summary>
        /// Raised when a new MediaPlayer is set and will be attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangingEventArgs>? MediaPlayerChanging;

        /// <summary>
        /// Raised when a new MediaPlayer is set and attached to the view
        /// </summary>
        public event EventHandler<MediaPlayerChangedEventArgs>? MediaPlayerChanged;

        /// <summary>
        /// Raised when the MediaPlayer has been initialized
        /// </summary>
        public event EventHandler<InitializedEventArgs>? MediaPlayerInitialized;

        /// <summary>
        /// Maui MediaPlayer databinded property
        /// </summary>
        public static readonly BindableProperty MediaPlayerProperty = BindableProperty.Create(nameof(MediaPlayer),
                typeof(LibVLCSharp.Shared.MediaPlayer),
                typeof(VideoView),
                propertyChanging: OnMediaPlayerChanging,
                propertyChanged: OnMediaPlayerChanged);

        /// <summary>
        /// The MediaPlayer object attached to this view
        /// </summary>
        public LibVLCSharp.Shared.MediaPlayer? MediaPlayer
        {
            get { return GetValue(MediaPlayerProperty) as LibVLCSharp.Shared.MediaPlayer; }
            set { SetValue(MediaPlayerProperty, value); }
        }

        private static void OnMediaPlayerChanging(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanging");
            videoView.MediaPlayerChanging?.Invoke(videoView, new MediaPlayerChangingEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
        }

        private static void OnMediaPlayerChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var videoView = (VideoView)bindable;
            Debug.WriteLine("OnMediaPlayerChanged");
            videoView.MediaPlayerChanged?.Invoke(videoView, new MediaPlayerChangedEventArgs(oldValue as LibVLCSharp.Shared.MediaPlayer, newValue as LibVLCSharp.Shared.MediaPlayer));
        }

        private readonly Lazy<PlatformConfigurationRegistry<VideoView>> _platformConfigurationRegistry;

        /// <summary>
        /// Constructor
        /// </summary>
        public VideoView()
        {
            _platformConfigurationRegistry = new Lazy<PlatformConfigurationRegistry<VideoView>>(() => new PlatformConfigurationRegistry<VideoView>(this));
        }
        
        /// <inheritdoc />
        public IPlatformElementConfiguration<T, VideoView> On<T>() where T : IConfigPlatform
        {
            return _platformConfigurationRegistry.Value.On<T>();
        }

        /// <inheritdoc />
        public void OnInitialized(InitializedEventArgs e) => MediaPlayerInitialized?.Invoke(this, e);
    }
}
