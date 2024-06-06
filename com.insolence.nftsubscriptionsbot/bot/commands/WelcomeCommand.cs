using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class WelcomeCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager
) : ICommand {
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        session.ResetSessionState();

        await botClient.SendPhotoAsync(
            chat.Id,
            photo: InputFile.FromStream(
                new FileStream(
                    Constants.WELCOME_IMAGE_PATH, 
                    FileMode.Open, 
                    FileAccess.Read, 
                    FileShare.Read
                )
            ),
            caption: $"Привет! Этот бот умеет управлять NFT подписками, находить их в кошельке сети TON и привязывать к сервису.\nДля начала давай привяжем кошелек TON.",
            replyMarkup: new InlineKeyboardMarkup(
                new List<InlineKeyboardButton>() {
                    InlineKeyboardButton.WithCallbackData(
                        "Привязать кошелек",
                        Constants.SELECT_WALLET_APP_CALLBACK
                    )
                }
            )
        );
    }
}