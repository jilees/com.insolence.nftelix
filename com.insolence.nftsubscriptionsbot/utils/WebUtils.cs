namespace com.insolence.nftsubscriptionsbot.utils;

public static class WebUtils{

    public static async Task<Stream> GetAsStream(this string url)
    {
        using var client = new HttpClient();
        return await client.GetStreamAsync(url);
    }

    public static async Task<string> GetAsString(this string url){
        using var client = new HttpClient();
        return await client.GetStringAsync(url);

    }

}