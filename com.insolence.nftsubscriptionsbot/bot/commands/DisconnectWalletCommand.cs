
using com.insolence.nftsubscriptionsbot.session;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class DisconnectWalletCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        if (session.IsWalletConnected()) {
            var address = session.GetCurrentWallet().Account.Address;
            session.DisconnectWallet();
            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Кошелек {address} отвязан. Можно привязать другой."
            );
        }
        else
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Вы пока не привязали кошелек."
            );
        }
    }
}