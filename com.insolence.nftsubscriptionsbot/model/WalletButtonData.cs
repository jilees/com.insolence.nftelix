

using System.Globalization;
using TonSdk.Connect;

namespace com.insolence.nftsubscriptionsbot.model;

class WalletButtonData(WalletConfig wallet)
{

    public string AppName = wallet.AppName;
    public string DisplayName = Capitalize(wallet.AppName);
    public string CallbackData = $"{Constants.CONNECT_WALLET_CALLBACK} {wallet.AppName}";

    public static string Capitalize(string word)
    {
        return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(word);
    }
}
