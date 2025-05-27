namespace MonopolyGameApp
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }
        public App(IServiceProvider services)
        {
            InitializeComponent();
           // MainPage = new AppShell();

            Services = services;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new MainPage());

#if WINDOWS
    // Настройка окна после его создания
    window.HandlerChanged += (s, e) =>
    {
        if (window.Handler?.PlatformView is Microsoft.UI.Xaml.Window nativeWindow)
        {
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            var appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            
            if (appWindow.Presenter is Microsoft.UI.Windowing.OverlappedPresenter presenter)
            {
                presenter.SetBorderAndTitleBar(false, false); // Убираем рамку и заголовок
                presenter.Maximize(); // Разворачиваем на весь экран
            }
        }
    };
#endif

            return window;
        }
    }
}