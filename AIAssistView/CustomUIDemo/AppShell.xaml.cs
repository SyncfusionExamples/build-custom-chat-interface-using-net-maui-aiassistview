namespace CustomUIDemo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
#if WINDOWS || MACCATALYST
            var mainPageShellContent = new ShellContent
            {
                Title = "Home",
                ContentTemplate = new DataTemplate(typeof(DesktopViewPage)),
                Route = "MainPage"
            };

            Items.Add(mainPageShellContent);
#else
			var mainPageShellContent = new ShellContent
			{
				Title = "Home",
				ContentTemplate = new DataTemplate(typeof(MobileViewPage)),
				Route = "MobileViewPage"
			};

			Items.Add(mainPageShellContent);
#endif
        }
    }
}
