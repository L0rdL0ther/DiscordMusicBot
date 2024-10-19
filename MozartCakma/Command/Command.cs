using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MozartCakma.Service.Bot;

namespace MozartCakma.Command;

public class Command : BaseCommandModule
{
    public Program Main = Program.Instance;

    public VoiceService
        VoiceService = Program.Instance.Container.VoiceService;

    [Command("search")]
    public async Task Search(CommandContext ctx, [RemainingText] string queryT)
    {
        await VoiceService.JoinChannel(ctx.Member, ctx.Guild, ctx.Client);

        if (ctx.Member?.VoiceState == null || ctx.Member.VoiceState.Channel! == null)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var videoList = await Main.Container.YoutubeService.Search(queryT);

        if (videoList.Count > 0)
        {
            var options = new List<DiscordSelectComponentOption>();

            foreach (var video in videoList)
            {
                options.Add(new DiscordSelectComponentOption(
                    video.Title,
                    video.VideoId,
                    "Video Lenght: " + video.Lenght
                ));
                if (options.Count > 4) break;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = "Search",
                Description = "Select a video",
                Color = DiscordColor.Azure
            };

            var dropdown = new DiscordSelectComponent("dropdown", null, options);
            var builder = new DiscordMessageBuilder()
                .AddEmbed(embed)
                .AddComponents(dropdown);

            await ctx.Channel.SendMessageAsync(builder);
        }
    }

    [Command("play")]
    public async Task Play(CommandContext ctx, [RemainingText] string queryT)
    {
        if (ctx.Member?.VoiceState?.Channel == null)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        var container = Main.Container.Instance;
        string videoUrl = null;

        await container.VoiceService.JoinChannel(ctx.Member, ctx.Guild, ctx.Client);
        Console.WriteLine("here is problem");
        if (queryT.StartsWith("https://www.youtube.com/") ||
            queryT.StartsWith("https://www.instagram.com/") ||
            queryT.StartsWith("https://www.tiktok.com"))
        {
            videoUrl = queryT;
        }
        else if (queryT.StartsWith("https://") || queryT.StartsWith("http://"))
        {
            await ctx.RespondAsync(new DiscordEmbedBuilder
            {
                Title = "Error",
                Description = "Unknown Link",
                Color = DiscordColor.Red
            });
            return;
        }
        else
        {
            var video = (await container.YoutubeService.Search(queryT)).FirstOrDefault();
            if (video != null) videoUrl = $"https://www.youtube.com/watch?v={video.VideoId}";
        }
        Console.WriteLine("here is problem2");
        Console.WriteLine(videoUrl);
        if (videoUrl != null)
        {
            Console.WriteLine("haydeee");
            var result = await container.TrackService.PlayAsync(ctx.Guild, ctx.Channel, videoUrl);
            Console.WriteLine(result.Playing);
            var videoInfo = await container.YoutubeService.GetVideoInfoAsync(videoUrl);
            var embed = new DiscordEmbedBuilder
            {
                Title = result.Playing ? $"Title: {videoInfo.Title}" : "Successfully added track",
                Url = result.Playing ? $"https://www.youtube.com/watch?v={videoInfo.VideoId}" : null,
                Description = result.Playing
                    ? $"Video Length: {videoInfo.FormatedLenght}"
                    : $"Video Name: {result.Title} Track",
                ImageUrl = result.Playing ? videoInfo.Thumbnail : null,
                Color = result.Playing ? DiscordColor.Cyan : DiscordColor.Gray
            };

            await ctx.RespondAsync(embed);
        }
    }


    /*
     * await Main.Container.VoiceService.JoinChannel(ctx.Member, ctx.Guild, ctx.Client);
       await Task.Delay(1000);

       if (queryT.StartsWith("https://www.youtube.com/watch?v=")
           || queryT.StartsWith("https://www.instagram.com/")
           || queryT.StartsWith("https://www.tiktok.com"))
       {
           var embed1 = new DiscordEmbedBuilder
           {
               Title = "Playing",
               Url = queryT,
               Color = DiscordColor.Green
           };
           ctx.Message.RespondAsync(embed1);
           await Main.Container.VoiceService.PlayAudio(ctx.Guild, ctx.Channel, queryT);
       }
       else if (queryT.StartsWith("https://") || queryT.StartsWith("http://"))
       {
           var embed1 = new DiscordEmbedBuilder
           {
               Title = "Error",
               Description = "Unknow Link",
               Color = DiscordColor.Red
           };
           ctx.Message.RespondAsync(embed1);
       }
       else
       {
           var video = Main.Container.YoutubeService.Search(queryT).Result.First();
           var embed = new DiscordEmbedBuilder
           {
               Title = $"Title: {video.Title}",
               Url = $"https://www.youtube.com/watch?v={video.VideoId}",
               Description = $"Video Length: {video.FomatedLenght}",
               ImageUrl = video.Thumbnail,
               Color = DiscordColor.Green
           };
           ctx.Message.RespondAsync(embed);
           await Main.Container.VoiceService.PlayAudio(ctx.Guild, ctx.Channel,
               "https://www.youtube.com/watch?v=" + video.VideoId);
       }
     */

    [Command("stop")]
    public async Task Stop(CommandContext ctx, [RemainingText] string queryT)
    {
        if (ctx.Member?.VoiceState == null || ctx.Member.VoiceState.Channel! == null)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }

        Main.Container.VoiceService.StopAudio(ctx.Guild);
        var embed1 = new DiscordEmbedBuilder
        {
            Title = "Stopped",
            Color = DiscordColor.Red
        };
        ctx.Message.RespondAsync(embed1);
    }
}