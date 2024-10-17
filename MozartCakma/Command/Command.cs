using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using MozartCakma.Service.Bot;

namespace MozartCakma.Command;

public class Command : BaseCommandModule
{
    public Program Main = Program.Instance;

    public VoiceChannelService
        VoiceChannelService = Program.Instance.Container.VoiceChannelService;
    
    [Command("search")]
    public async Task Search(CommandContext ctx, [RemainingText] string queryT)
    {
        await VoiceChannelService.JoinChannel(ctx.Member,ctx.Guild,ctx.Client);

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
        if (ctx.Member?.VoiceState == null || ctx.Member.VoiceState.Channel! == null)
        {
            await ctx.RespondAsync("You are not in a voice channel.");
            return;
        }
        await Main.Container.VoiceChannelService.JoinChannel(ctx.Member,ctx.Guild,ctx.Client);
        await Task.Delay(1000);
        
        if (queryT.StartsWith("https://www.youtube.com/watch?v="))
        {
            var embed1 = new DiscordEmbedBuilder
            {
                Title = $"Playing",
                Url = queryT,
                Color = DiscordColor.Green
            };
            ctx.Message.RespondAsync(embed1);
            await Main.Container.VoiceChannelService.PlayAudio(ctx.Guild.Id, ctx.Channel, queryT);
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
            await Main.Container.VoiceChannelService.PlayAudio(ctx.Guild.Id, ctx.Channel, "https://www.youtube.com/watch?v="+video.VideoId);
            
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
        Main.Container.VoiceChannelService.StopAudio(ctx.Guild);
        var embed1 = new DiscordEmbedBuilder
        {
            Title = $"Stopped",
            Color = DiscordColor.Red
        };
        ctx.Message.RespondAsync(embed1);
        
    }
    
}