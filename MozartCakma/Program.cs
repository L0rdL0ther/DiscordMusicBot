using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using MozartCakma;
using MozartCakma.Events;
using MozartCakma.Service;
using Newtonsoft.Json;
using Command = MozartCakma.Command.Command;

public class Program
{
    public Container Container = new();
    public static Program Instance { get; private set; }

    private static async Task Main(string[] args)
    {
        var Settings = new Settings();
        Instance = new Program();
        var messageEvents = new MessageEvents();
        var interactionEvents = new InteractionEvents();

        var jsonConfig = JsonConvert.SerializeObject(Settings);

        var builder = DiscordClientBuilder.CreateDefault(Settings.BotToken, DiscordIntents.All)
            .UseCommandsNext(ex => { ex.RegisterCommands<Command>(); }, new CommandsNextConfiguration
            {
                StringPrefixes = new[] { Settings.Prefix }
            })
            .UseInteractivity(new InteractivityConfiguration
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            })
            .ConfigureEventHandlers(eventType =>
            {
                eventType
                    .HandleMessageCreated(messageEvents.MessageCreatedHandler)
                    .HandleComponentInteractionCreated(interactionEvents.ComponentInteractionCreated)
                    .HandleVoiceStateUpdated(Instance.Container.VoiceChannelService.HandleVoiceStateUpdated);
            });

        builder.UseVoiceNext(new VoiceNextConfiguration
        {
            AudioFormat = AudioFormat.Default
        });
        var client = builder.Build();

        try
        {
            await client.ConnectAsync();
            await Task.Delay(-1);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}