
namespace com.insolence.nftsubscriptionsbot.subscription;

interface ISubscriptionConnector
{
    public Task<bool> ConnectSubscription(string subscriptionCode, string account);

    public Task<CheckResult> CheckSubscription(string subscriptionCode);

    public Task<CheckResult> CheckAccount(string account);

}