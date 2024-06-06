
using com.insolence.nftsubscriptionsbot.session;
using com.insolence.nftsubscriptionsbot.subscription;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

class CheckAccountInputCommand(
    ITelegramBotClient botClient,
    SessionManager sessionManager,
    ISubscriptionConnector subscriptionConnector) : ICommand
{
    public async Task Execute(Chat chat)
    {
        var session = sessionManager.GetSession(chat.Id);
        var accountInput = session.SelectedAccountId;

        var checkAccountResult = await subscriptionConnector.CheckAccount(accountInput);
        if (!checkAccountResult.Result)
        {
            session.SelectedAccountId = null;
            await botClient.SendTextMessageAsync(
                chat.Id,
                checkAccountResult.Message
            );
            return;
        }

        var subscriptionCode = session.SelectedSubscriptionCode;
        var checkSubscriptinResult = await subscriptionConnector.CheckSubscription(subscriptionCode);
        if (!checkSubscriptinResult.Result)
        {
            session.ResetSessionState();
            await botClient.SendTextMessageAsync(
                chat.Id,
                checkSubscriptinResult.Message
            );
            return;
        }

        var currentAccount = checkSubscriptinResult.AccountId;

        session.SelectedAccountId = accountInput;

        string userMessage =
            currentAccount == null ?
            $"Подключить подписку {subscriptionCode} к аккаунту {accountInput}? Владелец NFT может в любой момент подключить подписку к другому аккаунту." :
            $"Эта подписка уже подключена к аккаунту {currentAccount}. Отключить и подключить к {accountInput}?";

        await botClient.SendTextMessageAsync(
            chat.Id,
            userMessage,
            replyMarkup: new InlineKeyboardMarkup(
                new List<InlineKeyboardButton>() {
                            InlineKeyboardButton.WithCallbackData(
                                "Да",
                                Constants.CONFIRM_CONNECT_SUBSCRIPTION_CALLBACK
                            ),
                            InlineKeyboardButton.WithCallbackData(
                                "Нет",
                                Constants.RESET_CONNECT_SUBSCRIPTION_CALLBACK
                            )
                }
            )
        );
    }

}