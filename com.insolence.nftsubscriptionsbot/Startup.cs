using com.insolence.nftsubscriptionsbot.bot.commands;
using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.subscription;
using com.insolence.nftsubscriptionsbot.tonclient;
using com.insolence.nftsubscriptionsbot.tonconnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

public static class Startup
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        return services.AddSingleton<ITelegramBotClient>(
                new TelegramBotClient(config["telegramBotApiKey"])
            )
            .AddSingleton(
                new TonConnectManager(config["tonAppManifest"])
            )
            .AddSingleton<SessionManager>()
            .AddSingleton(
                new TonNftDataSource(
                    config["tonCenterApiKey"],
                    config["nftCollectionAddress"]
                )
            )
            .AddSingleton(
                new NftSubscriptionAttributeGetter(config["nftCustomAttributeName"])
            )
            .AddSingleton<ISubscriptionConnector, GoogleSheetsTestConnector>()

            .AddSingleton<WelcomeCommand>()
            .AddSingleton<SelectWalletAppCommand>()
            .AddSingleton<ConnectWalletCommand>()
            .AddSingleton<DisconnectWalletCommand>()
            .AddSingleton<CheckNftCommand>()
            .AddSingleton<CheckAccountInputCommand>()
            .AddSingleton<ConnectSubscriptionCommandCommand>()
            .AddSingleton<ConfirmConnectSubscriptionCommand>()
            .AddSingleton<ResetConnectSubscriptionCommand>();
    }
}