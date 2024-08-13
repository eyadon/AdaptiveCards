using Microsoft.Extensions.Logging;
using Font = Microsoft.Maui.Font;

namespace AdaptiveCards.Sample.MAUIVisualizer
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Oswald-Bold.ttf");
                    fonts.AddFont("Oswald-Regular.ttf");
                    fonts.AddFont("Oswald-ExtraLight.ttf", "Oswald-Regular_ExtraLight");
                    fonts.AddFont("Roboto-Regular.ttf");
                    fonts.AddFont("Roboto-Light.ttf");
                    fonts.AddFont("Roboto-Medium.ttf");
                    fonts.AddFont("Roboto-Thin.ttf");

                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
