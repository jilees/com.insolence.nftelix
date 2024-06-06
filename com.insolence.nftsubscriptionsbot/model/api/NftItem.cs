
using Newtonsoft.Json;

namespace com.insolence.nftsubscriptionsbot.model.api;

public  class NftItem {


        [JsonProperty("address")]
        public string Address;

        [JsonProperty("collection_address")]
        public string CollectionAddress;

        [JsonProperty("index")]
        public string Index;

        [JsonProperty("content")]
        public NftItemContent Content;

    }
    