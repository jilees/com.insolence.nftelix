using Telegram.Bot.Types;

namespace com.insolence.nftsubscriptionsbot.bot.commands;

public interface ICommand{
    public Task Execute(Chat chat);

    internal class DelegateCommand(Func<Chat, Task> execute) : ICommand
    {
        public async Task Execute(Chat chat)
        {
            await execute(chat);
        }
    }
}