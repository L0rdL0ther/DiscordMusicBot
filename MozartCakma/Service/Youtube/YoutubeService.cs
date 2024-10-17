using MozartCakma.Dto;
using Newtonsoft.Json.Linq;

namespace MozartCakma.Service.youtube;

public class YoutubeService : IYoutubeService
{
    public Program Main = Program.Instance;

    public async Task<List<YtVideoInfo>> Search(string queryT)
    {
        var config = new Settings();
        using var client = new HttpClient();
        var requestUrl =
            $"https://youtube-search-and-download.p.rapidapi.com/search?query={Uri.EscapeDataString(queryT)}&type=v";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Add("x-rapidapi-host", "youtube-search-and-download.p.rapidapi.com");
        request.Headers.Add("x-rapidapi-key", config.RapidKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var contents = JObject.Parse(await response.Content.ReadAsStringAsync())["contents"];
        return contents?.Select(content => content["video"])
            .Where(video => video != null)
            .Select(video => new YtVideoInfo(
                video["title"]?.ToString(),
                0,
                video["thumbnails"]?[0]?["url"]?.ToString(),
                video["lengthText"]?.ToString(),
                video["videoId"]?.ToString()))
            .ToList() ?? new List<YtVideoInfo>();
    }

    public async Task<YtVideoInfo?> GetVideoInfo(string videoId)
    {
        YtVideoInfo? videoInfo = null;
        var config = new Settings();
        if (!string.IsNullOrEmpty(videoId))
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://www.searchapi.io/api/v1/search?api_key=62ZorMYyrob81z4PkmE1MQMe&engine=youtube_video&video_id=" +
                Uri.EscapeDataString(videoId));
            request.Headers.Add("Authorization", "Bearer " + config.VideoInfoApiKey);
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            var videoDetails = responseJson["video"];

            if (videoDetails != null)
            {
                var title = videoDetails["title"]?.ToString() ?? "Unknown Title";
                var lengthSeconds = videoDetails["length_seconds"]?.ToString();
                var length = lengthSeconds != null
                    ? TimeSpan.FromSeconds(int.Parse(lengthSeconds)).ToString(@"hh\:mm\:ss")
                    : "Unknown Length";
                var thumbnailUrl = videoDetails["thumbnail"]?.ToString() ?? "No Thumbnail";
                videoInfo = new YtVideoInfo(title, Convert.ToInt32(lengthSeconds), thumbnailUrl, length, videoId);
            }
        }

        return videoInfo;
    }
}