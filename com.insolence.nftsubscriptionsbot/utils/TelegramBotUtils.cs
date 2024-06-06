using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

static class TelegramBotUtils
{

    public static Chat GetChat(this Update update)
    {
        switch (update.Type)
        {

            case UpdateType.CallbackQuery:
                {
                    return update.CallbackQuery.Message.Chat;
                }
            case UpdateType.Message:
                {
                    return update.Message.Chat;
                }
            default: {
                    throw new ArgumentException("Update");
                }
        }
    }

}