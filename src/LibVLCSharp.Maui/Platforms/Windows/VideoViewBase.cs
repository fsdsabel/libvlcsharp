using System.Runtime.InteropServices;
using LibVLCSharp.Shared;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using DirectN;
using DesignMode = Windows.ApplicationModel.DesignMode;
using Constants = DirectN.Constants;
using Window = Microsoft.UI.Xaml.Window;
using Application = Microsoft.Maui.Controls.Application;
using Microsoft.Maui.LifecycleEvents;

namespace LibVLCSharp.Platforms.Windows
{
    static class ComExtensions
    {
        public static unsafe IntPtr NativePointer<T>(this T obj)
        {
            if (obj == null)
                return IntPtr.Zero;

            return Marshal.GetComInterfaceForObject<T, T>(obj);
        }
    }

    /// <summary>
    /// VideoView base class for the UWP platform
    /// </summary>
    public abstract class VideoViewBase : ContentControl, IVideoView
    {
        [ComImport, Guid("63aad0b8-7c24-40ff-85a8-640d944cc325"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface ISwapChainPanelNativeWinUI
        {
            [PreserveSig]
            HRESULT SetSwapChain(IDXGISwapChain swapChain);
        }

        SwapChainPanel? _panel;        
        IComObject<ID3D11Device>? _d3D11Device;
        IComObject<IDXGIDevice1>? _device;
        IComObject<IDXGIDevice3>? _device3;
        IComObject<IDXGISwapChain2>? _swapChain2;
        IComObject<IDXGISwapChain>? _swapChain;
        bool _loaded;

        /// <summary>
        /// The constructor
        /// </summary>
        public VideoViewBase()
        {
            DefaultStyleKey = typeof(VideoViewBase);

            Content = CreateSwapChainPanel();
            

            if (!DesignMode.DesignModeEnabled)
            {
                Unloaded += (s, e) => DestroySwapChain();

                // TODO where can we do this in MAUI?
                //Application.Current.Suspending += (s, e) => { Trim(); };
            }
        }

        private SwapChainPanel CreateSwapChainPanel()
        {
            _panel = new SwapChainPanel();

            if (DesignMode.DesignModeEnabled)
                return _panel;

            DestroySwapChain();

            _panel.SizeChanged += (s, eventArgs) =>
            {
                if (_loaded)
                {
                    UpdateSize();
                }
                else
                {
                    CreateSwapChain();
                }
            };

            _panel.CompositionScaleChanged += (s, eventArgs) =>
            {
                if (_loaded)
                {
                    UpdateScale();
                }
            };
            return _panel;
        }

        /// <summary>
        /// Gets the swapchain parameters to pass to the <see cref="LibVLC"/> constructor.
        /// If you don't pass them to the <see cref="LibVLC"/> constructor, the video won't
        /// be displayed in your application.
        /// Calling this property will throw an <see cref="InvalidOperationException"/> if the VideoView is not yet full Loaded.
        /// </summary>
        /// <returns>The list of arguments to be given to the <see cref="LibVLC"/> constructor.</returns>
        public string[] SwapChainOptions
        {
            get
            {
                if (!_loaded)
                {
                    throw new InvalidOperationException("You must wait for the VideoView to be loaded before calling GetSwapChainOptions()");
                }

                return new string[]
                {
                    $"--winrt-d3dcontext=0x{_d3D11Device!.GetImmediateContext().Object.NativePointer().ToString("x")}",
                    $"--winrt-swapchain=0x{_swapChain!.Object.NativePointer().ToString("x")}"
                };
            }
        }

        /// <summary>
        /// Called when the video view is fully loaded
        /// </summary>
        protected abstract void OnInitialized();

        /// <summary>
        /// Initializes the SwapChain for use with LibVLC
        /// </summary>
        void CreateSwapChain()
        {
            // Do not create the swapchain when the VideoView is collapsed.
            if (_panel == null || _panel.ActualHeight == 0)
                return;


            IComObject<IDXGIFactory2>? dxgiFactory = null;
            try
            {
                var deviceCreationFlags =
                    D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_BGRA_SUPPORT | D3D11_CREATE_DEVICE_FLAG.D3D11_CREATE_DEVICE_VIDEO_SUPPORT;

#if DEBUG
                try
                {
                    dxgiFactory = DXGIFunctions.CreateDXGIFactory2(DXGI_CREATE_FACTORY_FLAGS.DXGI_CREATE_FACTORY_DEBUG);
                }
                catch (Exception)
                {
                    dxgiFactory = DXGIFunctions.CreateDXGIFactory2();
                }
#else
                dxgiFactory = DXGIFunctions.CreateDXGIFactory2();
#endif
                _d3D11Device = null;
                foreach (var adapter in dxgiFactory.EnumAdapters())
                {
                    try
                    {
                        
                        _d3D11Device = D3D11Functions.D3D11CreateDevice(adapter.Object, 
                            D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_UNKNOWN, 
                            deviceCreationFlags);
                        break;
                    }
                    catch (Exception)
                    {
                    }
                }

                if (_d3D11Device is null)
                {
                    throw new VLCException("Could not create Direct3D11 device : No compatible adapter found.");
                }

                _device = new ComObject<IDXGIDevice1>(_d3D11Device.As<IDXGIDevice1>());

                //Create the swapchain


                var swapChainDescription = new DXGI_SWAP_CHAIN_DESC1
                {
                    Width = (uint)(_panel.ActualWidth * _panel.CompositionScaleX),
                    Height = (uint)(_panel.ActualHeight * _panel.CompositionScaleY),
                    Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
                    Stereo = false,                    
                    SampleDesc =
                    {
                        Count = 1,
                        Quality = 0
                    },
                    BufferUsage = Constants.DXGI_USAGE_RENDER_TARGET_OUTPUT,
                    BufferCount = 2,
                    SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_SEQUENTIAL,
                    Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH,
                    Flags = 0,
                    AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_PREMULTIPLIED
                };
                                
                _swapChain = dxgiFactory.CreateSwapChainForComposition<IDXGISwapChain1>(_device, swapChainDescription);
         
                _device.Object.SetMaximumFrameLatency(1);
                
                var guidNativePanel = typeof(ISwapChainPanelNativeWinUI).GUID;
                var swapChainPanelPtr = Marshal.GetIUnknownForObject(_panel);
                Marshal.QueryInterface(swapChainPanelPtr, ref guidNativePanel, out var pNativePanel);
                Marshal.Release(swapChainPanelPtr);
                var nativePanel = (ISwapChainPanelNativeWinUI)Marshal.GetObjectForIUnknown(pNativePanel);
                
                nativePanel.SetSwapChain(_swapChain.Object);

                
                Marshal.ReleaseComObject(nativePanel);
                
                

                // This is necessary so we can call Trim() on suspend
                _device3 = new ComObject<IDXGIDevice3>(_device.As<IDXGIDevice3>());
                

                _swapChain2 = new ComObject<IDXGISwapChain2>(_swapChain.As<IDXGISwapChain2>());
                
                UpdateScale();
                UpdateSize();
                _loaded = true;

                // for vlc 4.x we need to change this to the libvlc_video_set_output_callbacks calls
                OnInitialized();
            }
            catch (Exception ex)
            {
                DestroySwapChain();
                if (ex is Exception)
                {
                    throw new VLCException("DirectN operation failed, see InnerException for details", ex);
                }

                throw;
            }
            finally
            {
                dxgiFactory?.Dispose();
            }
        }

        /// <summary>
        /// Destroys the SwapChain and all related instances.
        /// </summary>
        void DestroySwapChain()
        {
            _swapChain2?.Dispose();
            _swapChain2 = null;

            _device3?.Dispose();
            _device3 = null;

            _swapChain?.Dispose();
            _swapChain = null;

            _device?.Dispose();
            _device = null;

            _d3D11Device?.Dispose();
            _d3D11Device = null;

            _loaded = false;
        }

        readonly Guid SWAPCHAIN_WIDTH = new Guid(0xf1b59347, 0x1643, 0x411a, 0xad, 0x6b, 0xc7, 0x80, 0x17, 0x7a, 0x6, 0xb6);
        readonly Guid SWAPCHAIN_HEIGHT = new Guid(0x6ea976a0, 0x9d60, 0x4bb7, 0xa5, 0xa9, 0x7d, 0xd1, 0x18, 0x7f, 0xc9, 0xbd);

        /// <summary>
        /// Associates width/height private data into the SwapChain, so that VLC knows at which size to render its video.
        /// </summary>
        void UpdateSize()
        {
            if (_panel is null || _swapChain is null || _swapChain.IsDisposed)
                return;

            var width = IntPtr.Zero;
            var height = IntPtr.Zero;

            try
            {
                width = Marshal.AllocHGlobal(sizeof(int));
                height = Marshal.AllocHGlobal(sizeof(int));
                
                var w = (int)(_panel.ActualWidth * _panel.CompositionScaleX);
                var h = (int)(_panel.ActualHeight * _panel.CompositionScaleY);

                Marshal.WriteInt32(width, w);
                Marshal.WriteInt32(height, h);

                _swapChain.Object.SetPrivateData(SWAPCHAIN_WIDTH, sizeof(int), width);
                _swapChain.Object.SetPrivateData(SWAPCHAIN_HEIGHT, sizeof(int), height);
            }
            finally
            {
                Marshal.FreeHGlobal(width);
                Marshal.FreeHGlobal(height);
            }
        }

        /// <summary>
        /// Updates the MatrixTransform of the SwapChain.
        /// </summary>
        void UpdateScale()
        {
            if (_panel is null) return;
            var matrix = new DXGI_MATRIX_3X2_F
            {                
                _11 = 1.0f / _panel.CompositionScaleX,
                _22 = 1.0f / _panel.CompositionScaleY
            };
            _swapChain2!.Object.SetMatrixTransform(ref matrix);
        }

        /// <summary>
        /// When the app is suspended, UWP apps should call Trim so that the DirectX data is cleaned.
        /// </summary>
        void Trim()
        {
            _device3?.Object.Trim();
        }

        /// <summary>
        /// When the media player is attached to the view.
        /// </summary>
        void Attach()
        {
        }

        /// <summary>
        /// When the media player is detached from the view.
        /// </summary>
        void Detach()
        {
        }


        /// <summary>
        /// Identifies the <see cref="MediaPlayer"/> dependency property.
        /// </summary>
        public static DependencyProperty MediaPlayerProperty { get; } = DependencyProperty.Register(nameof(MediaPlayer), typeof(MediaPlayer),
            typeof(VideoViewBase), new PropertyMetadata(null, OnMediaPlayerChanged));
        /// <summary>
        /// MediaPlayer object connected to the view
        /// </summary>
        public MediaPlayer? MediaPlayer
        {
            get => (MediaPlayer?)GetValue(MediaPlayerProperty);
            set => SetValue(MediaPlayerProperty, value);
        }

        private static void OnMediaPlayerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var videoView = (VideoViewBase)d;
            videoView.Detach();
            if (e.NewValue != null)
            {
                videoView.Attach();
            }
        }
    }
}
