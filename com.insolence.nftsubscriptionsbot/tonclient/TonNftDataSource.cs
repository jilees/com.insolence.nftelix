
using com.insolence.nftsubscriptionsbot.model.api;
using com.insolence.nftsubscriptionsbot.utils;
using Newtonsoft.Json;
using TonSdk.Client;
using TonSdk.Core;

namespace com.insolence.nftsubscriptionsbot.tonclient;

public class TonNftDataSource
{

    private readonly string _collectionAddress;

    public TonNftDataSource(string? apiKey, string collectionAddress)
    {
        _httpClient = BuildHttpClient(apiKey);
        _collectionAddress = collectionAddress;
    }


    private HttpClient _httpClient;

    private static HttpClient BuildHttpClient(string? apiKey)
    {
        var httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromMilliseconds(Convert.ToDouble(30000)),
            BaseAddress = new Uri("https://toncenter.com/api/v3/")
        };
        httpClient.DefaultRequestHeaders.Accept.Clear();
        if (apiKey != null)
        {
            httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
        }
        return httpClient;
    }

    public string GetBuyCollectionUrl(){
        return $"https://getgems.io/collection/{_collectionAddress}";
    }

    public async Task<List<NftItem>> GetNftItems(Address walletAddress)
    {
        string result = await new TonRequestV3(
            BuildRequestParameters(walletAddress),
            _httpClient
        ).CallGet();
        var response = JsonConvert.DeserializeObject<NftItemsResponse>(result);
        return response.NftItems;
    }

    public async Task<NftItemContent> GetNftItemContent(NftItem item)
    {
        var internalContent = item.Content;
        var externalContentUri = internalContent.Uri;
        if (externalContentUri == null) {
            return internalContent;
        }
        var result = await externalContentUri.GetAsString();
        return JsonConvert.DeserializeObject<NftItemContent>(result);
    }

    private RequestParametersV3 BuildRequestParameters(Address walletAddress){
        Dictionary<string, object> requestParams = new(){
                { OWNER_ADDRESS_PARAMETER_NAME, walletAddress.ToString() },
                { COLLECTION_ADDRESS_PARAMETER_NAME, _collectionAddress }
        };
        return new RequestParametersV3(GET_NFT_METHOD_NAME, requestParams);
    }

    private const string GET_NFT_METHOD_NAME = "nft/items";
    private const string OWNER_ADDRESS_PARAMETER_NAME = "owner_address";
    private const string COLLECTION_ADDRESS_PARAMETER_NAME = "collection_address";

}