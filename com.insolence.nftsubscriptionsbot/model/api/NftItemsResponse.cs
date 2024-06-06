using Newtonsoft.Json;

namespace com.insolence.nftsubscriptionsbot.model.api;

public class NftItemsResponse
{

    [JsonProperty("nft_items")]
    public List<NftItem> NftItems;
}