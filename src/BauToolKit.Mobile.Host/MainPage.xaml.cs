#nullable enable

#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Controls;

namespace BauToolKit.Mobile.Host;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}
#endif
