using com.insolence.nftsubscriptionsbot.tonconnect;
using TonSdk.Connect;

namespace com.insolence.nftsubscriptionsbot.session;

class Session {

    public Session(TonConnectManager tonConnectManager){
        _tonConnectManager = tonConnectManager;
        _tonConnect = _tonConnectManager.Create(_storage);
    }

    public string? SelectedWalletAppId = null;
    public string? SelectedSubscriptionCode = null;

    public string? SelectedAccountId = null;

    public bool WaitForAccountInput = false;

    public void ResetSessionState(){
        SelectedAccountId = null;
        SelectedSubscriptionCode = null;
        WaitForAccountInput = false;
    }

    private TonConnectManager _tonConnectManager;

    private InMemoryRemoteStorage _storage = InMemoryRemoteStorage.Build();

    private TonConnect _tonConnect;

    public bool IsWalletConnected() {
        return _tonConnect.IsConnected;
    }

    public Wallet GetCurrentWallet(){
        return _tonConnect.Wallet;
    }

    public void DisconnectWallet(){
        _storage.ClearStorage();
        _tonConnect = _tonConnectManager.Create(_storage);
        
    }

    public async Task<string> ConnectWallet(string appName, Func<Wallet, Task> onConnected){
        _tonConnect.OnStatusChange(async (wallet) => {
            await onConnected(wallet);
        });
        var walletConfig = _tonConnectManager.WalletConfigList.First(w => w.AppName == appName);
        var rawLink = await _tonConnect.Connect(walletConfig);
        return "https://ton-connect.github.io/open-tc?connect=" + Uri.EscapeDataString(rawLink).Replace("[", "%5B").Replace("]", "%5D");
    }

}