
using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.subscription;
using com.insolence.nftsubscriptionsbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class ConfirmConnectSubscriptionCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager,
    ISubscriptionConnector subscriptionConnector
) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        var subscriptionCode = session.SelectedSubscriptionCode;
        var accountId = session.SelectedAccountId;

        var result = await subscriptionConnector.ConnectSubscription(subscriptionCode, accountId);

        if (result) {
            session.ResetSessionState();
            await botClient.SendPhotoAsync(
                chat.Id,
                photo: InputFile.FromStream(
                    new FileStream(
                        Constants.FINAL_IMAGE_PATH, 
                        FileMode.Open, 
                        FileAccess.Read, 
                        FileShare.Read
                    )
                ),
                caption: $"Подписка {subscriptionCode} успешно подключена к аккаунту {accountId}. Можно заходить в сервис и начинать пользоваться.\nВладелец NFT может в любой момент воспользоваться помощью бота повторно и подключить подписку к другому аккаунту.\nСпасибо, и приятного пользования сервисом!"
            );
        }
        else
        {
             //TODO: Обработка ошибки привязки
        }
    }
}