using Android.Views;
using LibVLCSharp.Maui.Platforms.Android;
using LibVLCSharp.Maui.Shared;

[assembly: Dependency(typeof(SystemUI))]
namespace LibVLCSharp.Maui.Platforms.Android
{
    internal class SystemUI : ISystemUI
    {
        public void ShowSystemUI()
        {
            var decorView = Platform.Activity?.Window?.DecorView;
            if (decorView == null)
                return;

#pragma warning disable CS0618 // Type or member is obsolete
            decorView.SystemUiVisibility =
                (StatusBarVisibility)(SystemUiFlags.LayoutStable |
                SystemUiFlags.LayoutFullscreen);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public void HideSystemUI()
        {
            var decorView = Platform.Activity?.Window?.DecorView;
            if (decorView == null)
                return;

#pragma warning disable CS0618 // Type or member is obsolete
            decorView.SystemUiVisibility = 
               decorView.SystemUiVisibility |
               (StatusBarVisibility)(SystemUiFlags.ImmersiveSticky | 
               SystemUiFlags.Fullscreen | 
               SystemUiFlags.HideNavigation |
               SystemUiFlags.LayoutStable | 
               SystemUiFlags.LayoutFullscreen | 
               SystemUiFlags.LayoutHideNavigation);
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}
