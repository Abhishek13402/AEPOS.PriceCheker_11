﻿using Microsoft.Extensions.Logging;
using AEPOS.PriceChecker_11.SQLiteClasses;
using CommunityToolkit.Maui;

using Telerik.Maui.Controls.Compatibility;
using AEPOS.PriceCheker_11;


namespace AEPOS.PriceChecker_11
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

			builder
				.UseTelerik()
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
				.ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
            .ConfigureMauiHandlers(handlers => {

                handlers.AddHandler<Microsoft.Maui.Controls.Entry, Microsoft.Maui.Handlers.EntryHandler>();

                //For TimePicker
                Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
                {
#if ANDROID
                    if (handler.PlatformView is Android.Widget.EditText nativeEntry)
                    {
                        nativeEntry.Background = null;
                    }
#endif
                });

            });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<CacheDataRepo>(s => ActivatorUtilities.CreateInstance<CacheDataRepo>(s));


            return builder.Build();
        }
    }
}
