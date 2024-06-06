using System.Net.Mail;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using static Google.Apis.Sheets.v4.SpreadsheetsResource.ValuesResource.UpdateRequest;

namespace com.insolence.nftsubscriptionsbot.subscription;

public class GoogleSheetsTestConnector : ISubscriptionConnector
{
    
    private static readonly string ServiceAccountEmail = "test-push-project-162211@appspot.gserviceaccount.com";
    private static readonly string[] _scopes = { SheetsService.Scope.Spreadsheets };
    private static readonly string ApplicationName = "Test NFT Subscriptions App";
    private static readonly string SpreadSheetId = "1-w9mu7hw73XDtqwik5dlBI-j3eyl6InLYlGgcBndviY";
    private static readonly string PageName = "Subscriptions";
    private const int ACCOUNT_ID_COLUMN_INDEX = 1;

    private readonly SheetsService _service;

    public GoogleSheetsTestConnector(){

        using Stream stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read, FileShare.Read);
        var credential = (ServiceAccountCredential) GoogleCredential.FromStream(stream).UnderlyingCredential;

        var initializer = new ServiceAccountCredential.Initializer(credential.Id)
        {
            User = ServiceAccountEmail,
            Key = credential.Key,
            Scopes = _scopes
        };
        var serviceAccountCredential = new ServiceAccountCredential(initializer);

        _service = new SheetsService(new BaseClientService.Initializer() {
                ApplicationName = ApplicationName,
                HttpClientInitializer = serviceAccountCredential
            }
        );

    }

    public async Task<CheckResult> CheckAccount(string account){
        if (MailAddress.TryCreate(account, out MailAddress? result)){
            return new CheckResult(){
                Result = true
            };
        } else {
            return new CheckResult(){
                Result = false,
                Message = "Укажите корректный адрес электронной почты в качестве аккаунта."
            };
        }
    }

    public async Task<CheckResult> CheckSubscription(string subscriptionCode)
    {
        var subscriptions = await GetSubscriptions();
        var subscription = subscriptions.FirstOrDefault( s => {
            return s.SubscriptionCode == subscriptionCode;
        }, null);

        if (subscription == null){
            return new CheckResult(){
                Result = false,
                Message = "Код подписки не найден в сервисе. Свяжитесь с поддержкой, вам обязательно помогут."
            };
        } else {
            return new CheckResult(){
                Result = true,
                AccountId = subscription.AccountId
            };
        }
    }

    public async Task<bool> ConnectSubscription(string subscriptionCode, string accountId)
    {
        var subscriptions = await GetSubscriptions();
        var subscription = subscriptions.FirstOrDefault( s => {
            return s.SubscriptionCode == subscriptionCode;
        }, null);
        if (subscription == null) return false;
        subscription.AccountId = accountId;
        await UpdateAccountId(subscription);
        return true;
    }

    private async Task UpdateAccountId(SubscriptionRow subscription) {
        await UpdateCell(
            subscription.Index, 
            ACCOUNT_ID_COLUMN_INDEX, 
            subscription.AccountId ?? string.Empty
        );
    }

    private async Task UpdateCell(int rowIndex, int columnIndex, string value)
    {
        var updateRequest = _service.Spreadsheets.Values.Update(
            new ValueRange {
                MajorDimension = "ROWS",
                Values = [[value]],

            },
            SpreadSheetId,
            BuildRange(rowIndex, columnIndex)
        );
        updateRequest.ValueInputOption = ValueInputOptionEnum.USERENTERED;
        await Task.Run(() => {
            updateRequest.Execute();
        });
    }

    private static string BuildRange(int rowIndex, int columnIndex){
        return $"{PageName}!R{rowIndex + 1}C{columnIndex + 1}";
    }

    private async Task<List<SubscriptionRow>> GetSubscriptions()
    {
        var sheetData = await Task.Run(() => {
            return _service.Spreadsheets.Values.Get(SpreadSheetId, PageName).Execute();
        });
        var sheetValues = sheetData.Values;
        return sheetValues.Select((row, index) =>
        {
            var subscription = new SubscriptionRow()
            {
                Index = index,
                SubscriptionCode = row[0].ToString(),
            };
            if (row.Count > 1){
                var accountId = row[1].ToString();
                if (!string.IsNullOrEmpty(accountId))
                {
                    subscription.AccountId = accountId;
                }
            }
            return subscription;
        }).ToList();
    }

    
    private class SubscriptionRow
    {
        public int Index;
        public string SubscriptionCode;
        public string? AccountId;
    }
}
