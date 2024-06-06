using TonSdk.Connect;

namespace com.insolence.nftsubscriptionsbot.tonconnect;

class TonConnectManager
{

    private TonConnectOptions options;

    public List<WalletConfig> WalletConfigList
    {
        get; private set;
    }

    public TonConnectManager(string manifestUrl)
    {
        options = new TonConnectOptions()
        {
            ManifestUrl = manifestUrl,
            WalletsListSource = "https://raw.githubusercontent.com/ton-blockchain/wallets-list/main/wallets-v2.json"
        };
        WalletConfigList = new TonConnect(options).GetWallets().ToList();
    }

    public TonConnect Create(RemoteStorage storage)
    {
        return new TonConnect(options, storage);
    }
}