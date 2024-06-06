using com.insolence.nftsubscriptionsbot.model.api;

namespace com.insolence.nftsubscriptionsbot.tonclient;

public class NftSubscriptionAttributeGetter(string attributeName){

    public string? GetAttribute(NftItemContent content) {
        var attribute = content.Attributes?.FirstOrDefault(a => a.TraitType == attributeName);
        if (attribute == null){
            return null;
        } else {
            return attribute.Value;
        }
    }

}