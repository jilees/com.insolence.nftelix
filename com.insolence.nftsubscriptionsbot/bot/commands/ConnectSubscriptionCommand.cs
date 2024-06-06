using com.insolence.nftsubscriptionsbot.session;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class ConnectSubscriptionCommandCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        session.WaitForAccountInput = true;
        await botClient.SendTextMessageAsync(
            chat.Id,
            $"Введите идентификатор Вашего аккаунта в сервисе. Как правило, это адрес электронной почты."
        );
    }
}