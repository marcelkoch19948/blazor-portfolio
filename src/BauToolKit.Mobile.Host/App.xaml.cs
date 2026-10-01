#nullable enable

#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Controls;

namespace BauToolKit.Mobile.Host;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new MainPage();
    }
}
#endif
