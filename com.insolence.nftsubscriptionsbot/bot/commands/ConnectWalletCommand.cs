using com.insolence.nftsubscriptionsbot.model;
using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.tonconnect;
using com.insolence.nftsubscriptionsbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class ConnectWalletCommand(
    ITelegramBotClient botClient,
    TonConnectManager tonConnectManager,
    SessionManager sessionManager) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        var appName = session.SelectedWalletAppId;
        var connectLink = await session.ConnectWallet(appName, async (wallet) =>
            {
                await botClient.SendPhotoAsync(
                    chat.Id,
                    photo: InputFile.FromStream(
                        new FileStream(
                            Constants.CHECK_NFT_IMAGE_PATH, 
                            FileMode.Open, 
                            FileAccess.Read, 
                            FileShare.Read
                        )
                    ),
                    caption: $"Кошелек {wallet.Account.Address} успешно привязан.\nТеперь проверим, есть ли у вас в кошельке NFT подписки. Не забудьте, NFT не должен быть выставлен на продажу!",
                    replyMarkup: new InlineKeyboardMarkup(
                        new List<InlineKeyboardButton>() {
                                InlineKeyboardButton.WithCallbackData(
                                    "Проверить NFT",
                                    Constants.CHECK_NFT_CALLBACK
                                )
                        }
                    )
                );
            });

        var selectedWallet = new WalletButtonData(tonConnectManager.WalletConfigList.First(w =>
        {
            return w.AppName == appName;
        }));

        await botClient.SendTextMessageAsync(
            chat.Id,
            $"Хорошо, давайте привяжем кошелек с помощью {selectedWallet.DisplayName}.",
            parseMode: ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(
                new List<InlineKeyboardButton>() {
                    InlineKeyboardButton.WithUrl(
                        $"Привязать через {selectedWallet.DisplayName}",
                        connectLink
                    )
                }
            )
        );
    }
}