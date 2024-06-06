using Newtonsoft.Json;

namespace com.insolence.nftsubscriptionsbot.model.api;

public class NftItemContentAttribute
{
    [JsonProperty("trait_type")]
    public string TraitType;

    [JsonProperty("value")]
    public string? Value;

}