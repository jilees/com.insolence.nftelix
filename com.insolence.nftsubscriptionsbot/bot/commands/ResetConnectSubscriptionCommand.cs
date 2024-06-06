using com.insolence.nftsubscriptionsbot.session;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class ResetConnectSubscriptionCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager
) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        session.ResetSessionState();
        await botClient.SendTextMessageAsync(
            chat.Id,
            "Ок, мы отменили подключение подписки к аккаунту. Можно начать заново."
        );
    }
}