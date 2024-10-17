using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace MozartCakma.Events;

public class InteractionEvents
{
    public Program main = Program.Instance;

    public async Task ComponentInteractionCreated(DiscordClient discordClient, ComponentInteractionCreatedEventArgs e)
    {
        if (e.Interaction.Type != DiscordInteractionType.Component) return;

        var videoId = e.Interaction.Data?.Values?.FirstOrDefault();

        if (string.IsNullOrEmpty(videoId))
        {
            await e.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("No selection made."));
            return;
        }
        
        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
            new DiscordInteractionResponseBuilder().WithContent("Its can take a while minute"));

        try
        {
            var videoDetails = await main.Container.YoutubeService.GetVideoInfo(videoId);
            if (videoDetails == null)
            {
                await e.Interaction.EditOriginalResponseAsync(
                    new DiscordWebhookBuilder().WithContent("Video details not found."));
                return;
            }

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Title: {videoDetails.Title}",
                Url = $"https://www.youtube.com/watch?v={videoId}",
                Description = $"Video Length: {videoDetails.FomatedLenght}",
                ImageUrl = videoDetails.Thumbnail,
                Color = DiscordColor.Green
            };

            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
            
            main.Container.VoiceChannelService.PlayAudio(e.Guild.Id,e.Channel,$"https://www.youtube.com/watch?v={Uri.EscapeUriString(videoDetails.VideoId)}");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex}");
            await e.Interaction.EditOriginalResponseAsync(
                new DiscordWebhookBuilder().WithContent("An error occurred while processing your request."));
        }
    }
}