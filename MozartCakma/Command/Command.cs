using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        
        if (videoUrl != null)
        {
            var result = await container.TrackService.PlayAsync(ctx.Guild, ctx.Channel, videoUrl);
            Console.WriteLine(result.Playing);
            var videoInfo = await container.YoutubeService.GetVideoInfoAsync(videoUrl);
            Console.WriteLine(videoInfo.Title);
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