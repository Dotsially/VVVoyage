using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using VVVoyage.Database;
using VVVoyage.ViewModels;
using VVVoyage.Views;

namespace VVVoyage
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<IAppDatabase>(new SQLAppDatabase(false));
            builder.Services.AddSingleton<MainMenuViewModel>();
            builder.Services.AddSingleton<MainMenuPage>();

            return builder.Build();
        }
    }
}
