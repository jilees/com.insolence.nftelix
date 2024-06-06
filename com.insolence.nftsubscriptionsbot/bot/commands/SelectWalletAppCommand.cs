using com.insolence.nftsubscriptionsbot.model;
using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.tonconnect;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class SelectWalletAppCommand(
    ITelegramBotClient botClient,
    TonConnectManager tonConnectManager,
    SessionManager sessionManager) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);

        if (session.IsWalletConnected())
        {
            var wallet = session.GetCurrentWallet();
            var address = wallet.Account.Address;

            await botClient.SendTextMessageAsync(
                chat.Id,
                $"У вас уже привязан кошелек {address}, чтобы подключить новый, сначала нужно отвязать этот."
            );
        }
        else
        {
            var wallets = tonConnectManager.WalletConfigList.Select(w =>
            {
                return new WalletButtonData(w);
            });

            var buttons = wallets.Select(c =>
                InlineKeyboardButton.WithCallbackData(
                    c.DisplayName,
                    c.CallbackData
                )
            ).ToList().Chunk(2);

            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            await botClient.SendTextMessageAsync(
                chat.Id,
                "Через какой клиент будем привязывать кошелек?",
                replyMarkup: inlineKeyboard);
        }
    }
}