using com.insolence.nftsubscriptionsbot.tonconnect;
using Telegram.Bot;

namespace com.insolence.nftsubscriptionsbot.session;


class SessionManager(TonConnectManager tonConnectManager){

    private readonly Dictionary<long, Session> _sessions = [];
    
    public Session GetSession(long chatId){

        if (_sessions.TryGetValue(chatId, out Session? value))
            return value;
        
        var newSession = new Session(tonConnectManager);
        _sessions.Add(chatId, newSession);
        return newSession;
    }

}