using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MozartCakma.Dto;
using Newtonsoft.Json.Linq;

namespace MozartCakma.Service.youtube;

public class YoutubeService : IYoutubeService
{
    public Settings Config = Program.Instance.Settings;
    public Program Main = Program.Instance;


    public async Task<List<YtVideoDto>> Search(string queryT)
    {
        using var client = new HttpClient();
        var requestUrl =
            $"https://youtube-search-and-download.p.rapidapi.com/search?query={Uri.EscapeDataString(queryT)}&type=v";

        var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
        request.Headers.Add("x-rapidapi-host", "youtube-search-and-download.p.rapidapi.com");
        request.Headers.Add("x-rapidapi-key", Config.RapidKey);
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        var contents = JObject.Parse(await response.Content.ReadAsStringAsync())["contents"];
        return contents?.Select(content => content["video"])
            .Where(video => video != null)
            .Select(video => new YtVideoDto(
                video["title"]?.ToString(),
                0,
                video["thumbnails"]?[0]?["url"]?.ToString(),
                video["lengthText"]?.ToString(),
                video["videoId"]?.ToString()))
            .ToList() ?? new List<YtVideoDto>();
    }

    public async Task<YtVideoDto?> GetVideoInfoAsync(string videoIdOrUrl)
    {
        if (string.IsNullOrEmpty(videoIdOrUrl))
            return null;

        YtVideoDto? videoInfo = null;
        string videoId;
        var regex = new Regex(
            @"(?:https?:\/\/)?(?:www\.)?(?:youtube\.com\/(?:[^\/\n\s]+\/\S+\/|(?:v|e(?:mbed)?|watch(?:\?.*)?)\/|.*[?&]v=)|youtu\.be\/)([a-zA-Z0-9_-]{11})");
        var match = regex.Match(videoIdOrUrl);
        videoId = match.Success ? match.Groups[1].Value : videoIdOrUrl;

        if (string.IsNullOrEmpty(videoId))
            return null;
        var process = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = $"-j {videoIdOrUrl}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        try
        {
            var ytDlp = Process.Start(process);
            var output = ytDlp.StandardOutput.ReadToEnd();
            ytDlp.WaitForExit();
            var videoDetails = JObject.Parse(output);
            var title = videoDetails["title"]?.ToString() ?? "Unknown Title";
            var lengthSeconds = videoDetails["duration"]?.ToObject<int>() ?? 0;
            var lengthFormatted = TimeSpan.FromSeconds(lengthSeconds).ToString(@"hh\:mm\:ss");
            var thumbnailUrl = videoDetails["thumbnail"]?.ToString() ?? "No Thumbnail";
            videoInfo = new YtVideoDto(title, lengthSeconds, thumbnailUrl, lengthFormatted, videoId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            return null;
        }

        return videoInfo;
    }
}