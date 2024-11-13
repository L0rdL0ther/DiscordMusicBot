using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace MozartCakma.Events;

public class InteractionEvents
{
    public Program main = Program.Instance;

    public async Task ComponentInteractionCreated(DiscordClient discordClient, ComponentInteractionCreatedEventArgs e)
    {
        // If the interaction is not of type Component, exit the function
        if (e.Interaction.Type != DiscordInteractionType.Component) return;

        // If the user who triggered the interaction is not the same as the one interacting, exit the function
        if (e.Interaction.User.Id != e.User.Id) return;

        // Get the videoId from the interaction data
        var videoId = e.Interaction.Data?.Values?.FirstOrDefault();

        // Check if the interaction was created more than 30 seconds ago
        if (DateTimeOffset.Now - e.Interaction.CreationTimestamp > TimeSpan.FromSeconds(30))
        {
            // If interaction time expired (30 seconds), send an expiration message and return
            await e.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                    .WithContent("You did not interact in time, the action has been canceled."));
            return; // Exit the method
        }

        // If no video ID is selected, notify the user
        if (string.IsNullOrEmpty(videoId))
        {
            await e.Interaction.CreateResponseAsync(
                DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("No selection made."));
            return;
        }

        // Respond to the user to indicate that the video might take some time to load
        await e.Interaction.CreateResponseAsync(DiscordInteractionResponseType.UpdateMessage,
            new DiscordInteractionResponseBuilder().WithContent(
                "It can take a minute to fetch the video details. Please wait."));

        try
        {
            // Fetch video details asynchronously
            var videoDetails = await main.Container.YoutubeService.GetVideoInfoAsync(videoId);

            // If no video details were found, inform the user
            if (videoDetails == null)
            {
                await e.Interaction.EditOriginalResponseAsync(
                    new DiscordWebhookBuilder().WithContent("Video details not found."));
                return;
            }

            // Play the video asynchronously and get the result
            var playAsyncTask = main.Container.TrackService.PlayAsync(e.Guild, e.Channel,
                $"https://www.youtube.com/watch?v={Uri.EscapeUriString(videoDetails.VideoId)}");
            var result = playAsyncTask.Result;

            // Prepare an embed to display video details
            var embed = new DiscordEmbedBuilder
            {
                Title = result.Playing ? $"Title: {videoDetails.Title}" : "Successfully added track",
                Url = $"https://www.youtube.com/watch?v={videoDetails.VideoId}",
                Description = result.Playing
                    ? $"Video Length: {videoDetails.FormatedLenght}"
                    : $"Video Name: {result.Title} Track",
                ImageUrl = videoDetails.Thumbnail,
                Color = result.Playing ? DiscordColor.Cyan : DiscordColor.Gray
            };

            // Edit the original response with the video details
            await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));
        }
        catch (Exception ex)
        {
            // Log and send error response if something went wrong
            Console.WriteLine($"An unexpected error occurred: {ex}");
            await e.Interaction.EditOriginalResponseAsync(
                new DiscordWebhookBuilder().WithContent("An error occurred while processing your request."));
        }
    }
}