using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.VoiceNext;
using MozartCakma.Events;
using MozartCakma.Service;
using Newtonsoft.Json;

namespace MozartCakma;

public class Program
{
    public Program(Container container)
    {
        Container = container;
    }

    public Container Container { get; }
    public static Program Instance { get; private set; }

    private static async Task Main(string[] args)
    {
        Instance = new Program(new Container()); // Burada Instance atanıyor
        Instance.Container.Initialize();
        var settings = new Settings();
        var messageEvents = new MessageEvents();
        var interactionEvents = new InteractionEvents();
        var MusicEvents = new MusicEvents();
        var jsonConfig = JsonConvert.SerializeObject(settings);

        var builder = DiscordClientBuilder.CreateDefault(settings.BotToken, DiscordIntents.All)
            .UseCommandsNext(ex => { ex.RegisterCommands<Command.Command>(); }, new CommandsNextConfiguration
            {
                StringPrefixes = new[] { settings.Prefix }
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
                .HandleVoiceStateUpdated(Instance.Container.VoiceService.HandleVoiceStateUpdated);
            });

            Instance.Container.MusicEvents.OnMusicFinished += MusicEvents.OnOnMusicFinished;
                
                
        
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