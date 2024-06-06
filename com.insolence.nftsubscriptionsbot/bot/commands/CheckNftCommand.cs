using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.tonclient;
using com.insolence.nftsubscriptionsbot.utils;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class CheckNftCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager, 
    TonNftDataSource tonNftDataSource,
    NftSubscriptionAttributeGetter attributeGetter) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        if (session.IsWalletConnected())
        {
            var wallet = session.GetCurrentWallet();
            var ownerAddress = wallet.Account.Address;
            var items = await tonNftDataSource.GetNftItems(ownerAddress);

            if (items.Count == 0)
            {
                await botClient.SendTextMessageAsync(
                    chat.Id,
                    $"У вас нет NFT из нужной коллекции. Но вы можете купить их на <a href='https://getgems.io/collection/{tonNftDataSource.GetBuyCollectionUrl()}'>getgems.io</a>",
                    parseMode: ParseMode.Html
                );
            }
            /*else if (items.Count > 1)
            {
                //TODO: Выбор из нескольких токенов
            }*/
            else
            {

                var itemOfColllection = items.First();
                
                var nftItemContent = await tonNftDataSource.GetNftItemContent(itemOfColllection);

                Console.Out.WriteLine("NFT Content description: " + nftItemContent.Description);
                Console.Out.WriteLine("NFT Content image: " + nftItemContent.Image);

                var paramValue = attributeGetter.GetAttribute(nftItemContent);

                session.SelectedSubscriptionCode = paramValue;

                await botClient.SendPhotoAsync(
                                chat.Id,
                                photo: InputFile.FromStream(
                                    new FileStream(
                                        Constants.CONNECT_SUBSCRIPTION_IMAGE_PATH, 
                                        FileMode.Open, 
                                        FileAccess.Read, 
                                        FileShare.Read
                                    )
                                ),
                                caption: $"Мы нашли у вас в кошельке подходящий NFT. Код подписки из вашего NFT: {paramValue}.\nДавайте теперь подключим ее к аккаунту в сервисе.",
                                replyMarkup: new InlineKeyboardMarkup(
                                    new List<InlineKeyboardButton>() {
                                        InlineKeyboardButton.WithCallbackData(
                                            "Подключить подписку к аккаунту",
                                             Constants.CONNECT_SUBSCRIPTION_CALLBACK
                                        )
                                    }
                                )
                );
            }

        }
        else
        {
            await botClient.SendTextMessageAsync(
                chat.Id,
                $"Вы пока не привязали кошелек."
            );
        }
        return;
    }
}