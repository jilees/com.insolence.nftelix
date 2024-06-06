using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.model;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Microsoft.Extensions.DependencyInjection;
using com.insolence.nftsubscriptionsbot.bot.commands;
using com.insolence.nftsubscriptionsbot;
using com.insolence.nftsubscriptionsbot.subscription;

class Program
{
    private static ServiceProvider _serviceProvider;

    private static async Task ExecuteCommand<T>(Chat chat) where T : ICommand
    {
        var command = _serviceProvider.GetService<T>();
        if (command != null)
        {
            await command.Execute(chat);
        }
    }

    static async Task Main()
    {

        _serviceProvider = new ServiceCollection()
           .ConfigureServices()
           .BuildServiceProvider();

        var botClient = _serviceProvider.GetService<ITelegramBotClient>();

        botClient.StartReceiving(
            UpdateHandler,
            ErrorHandler,
            new ReceiverOptions
            {
                AllowedUpdates = [
                    UpdateType.Message,
                    UpdateType.CallbackQuery
                ],
                ThrowPendingUpdates = true,
            },
            new CancellationTokenSource().Token
        );

        var me = await botClient.GetMeAsync();
        Console.WriteLine($"{me.FirstName} запущен!");

        await Task.Delay(-1);
    }

    private static async Task UpdateHandler(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {

        try
        {
            var sessionManager = _serviceProvider.GetService<SessionManager>();
            var chat = update.GetChat();
            var session = sessionManager.GetSession(chat.Id);

            switch (update.Type)
            {

                case UpdateType.CallbackQuery:
                    {
                        var callbackQuery = update.CallbackQuery;
                        var callbackPrefix = callbackQuery?.Data?.Split(' ')[0];
                        switch (callbackPrefix)
                        {
                            case Constants.SELECT_WALLET_APP_CALLBACK:
                                {
                                    await ExecuteCommand<SelectWalletAppCommand>(chat);
                                    return;
                                }
                            case Constants.CHECK_NFT_CALLBACK:
                                {
                                    await ExecuteCommand<CheckNftCommand>(chat);
                                    return;
                                }
                            case Constants.CONNECT_WALLET_CALLBACK:
                                {
                                    //TODO: Использовать URL форматирование 
                                    session.SelectedWalletAppId = callbackQuery?.Data?.Split(' ')[1];
                                    await ExecuteCommand<ConnectWalletCommand>(chat);
                                    return;
                                }
                            case Constants.CONNECT_SUBSCRIPTION_CALLBACK:
                                {
                                    await ExecuteCommand<ConnectSubscriptionCommandCommand>(chat);
                                    return;
                                }
                            case Constants.RESET_CONNECT_SUBSCRIPTION_CALLBACK:
                                {
                                    await ExecuteCommand<ResetConnectSubscriptionCommand>(chat);
                                    return;
                                }
                            case Constants.CONFIRM_CONNECT_SUBSCRIPTION_CALLBACK:
                                {
                                    await ExecuteCommand<ConfirmConnectSubscriptionCommand>(chat);
                                    return;
                                }
                        }
                        return;
                    }
                case UpdateType.Message:
                    {
                        var message = update.Message;
                        var user = message.From;

                        // Выводим на экран то, что пишут нашему боту, а также небольшую информацию об отправителе
                        Console.WriteLine($"{user.FirstName} ({user.Id}) написал сообщение: {message.Text}");

                        switch (message.Text)
                        {
                            case "/start":
                                {
                                    await ExecuteCommand<WelcomeCommand>(chat);
                                    return;
                                }
                            case "/disconnect":
                                {
                                    await ExecuteCommand<DisconnectWalletCommand>(chat);
                                    return;
                                }
                            case "/connect":
                                {
                                    await ExecuteCommand<SelectWalletAppCommand>(chat);
                                    return;
                                }
                            case "/nft":
                                {
                                    await ExecuteCommand<CheckNftCommand>(chat);
                                    return;
                                }
                            default:
                                {
                                    if (session.WaitForAccountInput)
                                    {
                                        session.SelectedAccountId = message.Text;
                                        await ExecuteCommand<CheckAccountInputCommand>(chat);
                                        return;
                                    }

                                    // Chat - содержит всю информацию о чате
                                    await botClient.SendTextMessageAsync(
                                        chat.Id,
                                        message.Text, // отправляем то, что написал пользователь
                                        replyToMessageId: message.MessageId // по желанию можем поставить этот параметр, отвечающий за "ответ" на сообщение
                                        );

                                    return;
                                }
                        }
                    }

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }


    private static Task ErrorHandler(ITelegramBotClient botClient, Exception error, CancellationToken cancellationToken)
    {
        // Тут создадим переменную, в которую поместим код ошибки и её сообщение 
        var ErrorMessage = error switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => error.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}
