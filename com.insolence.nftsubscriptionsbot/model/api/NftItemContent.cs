using Newtonsoft.Json;

namespace com.insolence.nftsubscriptionsbot.model.api;

public class NftItemContent
{

    [JsonProperty("uri")]
    public string? Uri;

    [JsonProperty("name")]
    public string? Name;

    [JsonProperty("image")]
    public string? Image;

    [JsonProperty("description")]
    public string? Description;

    [JsonProperty("attributes")]
    public List<NftItemContentAttribute>? Attributes;

}