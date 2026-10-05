#nullable enable

#if ANDROID || IOS || MACCATALYST || WINDOWS
namespace BauToolKit.Mobile.Host;

public partial class App : Microsoft.Maui.Controls.Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}
#endif
