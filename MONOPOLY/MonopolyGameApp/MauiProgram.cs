using Microsoft.Extensions.Logging;
using Monopoly.BusinessLogic.Services.Interfaces;
using Monopoly.BusinessLogic.Services.Logic;

namespace MonopolyGameApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>() // App получает IServiceProvider автоматически
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // ⬇️ Регистрация твоих библиотек
        builder.Services.AddSingleton<IPlayerService, PlayerService>();
        builder.Services.AddSingleton<IPropertyService, PropertyService>();
        builder.Services.AddSingleton<PropertyRepository>(); // если без интерфейса
        builder.Services.AddSingleton<GameService>();

        return builder.Build();
    }
}

