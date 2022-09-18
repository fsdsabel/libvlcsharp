using LibVLCSharp.Maui.Handlers;
using LibVLCSharp.Maui.Shared;

namespace Microsoft.Maui.Hosting
{
    /// <summary>
    /// App Builder extensions
    /// </summary>
    public static class LibVLCSharpHostBuilderExtensions
    {
        /// <summary>
        /// Add VLCSharp components to a MAUI app.
        /// </summary>
        public static MauiAppBuilder UseVLCSharp(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddVLCSharpHandlers();
            });
            return builder;
        }

        /// <summary>
        /// Add VLC VideoViewHandler to MAUI handlers.
        /// </summary>
        public static IMauiHandlersCollection AddVLCSharpHandlers(this IMauiHandlersCollection handlers)
        {
            handlers.AddHandler<VideoView, VideoViewHandler>();
            return handlers;
        }
    }
}
