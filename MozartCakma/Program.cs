using DSharpPlus;
using DSharpPlus.CommandsNext;
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
    private static readonly string ConfigFileName = "config.json";

    public Program(Container container, Settings setting)
    {
        Container = container;
        Settings = setting;
    }


    public Container Container { get; }
    public Settings Settings { get; }
    public static Program Instance { get; private set; }

    private static async Task Main(string[] args)
    {
        Instance = new Program(new Container(), new Settings()); // Burada Instance atanıyor
        Instance.Container.Initialize();

        var settings = Instance.Settings;
        File.Open(ConfigFileName, FileMode.OpenOrCreate).Close();

        try
        {
            var rawConfigJson = File.ReadAllText("config.json");
            var configJson = JsonConvert.DeserializeObject<Settings>(rawConfigJson);
            settings = configJson ?? new Settings();
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error accessing file: {ex.Message}");
            settings = new Settings();
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Error deserializing JSON: {ex.Message}");
            settings = new Settings();
        }

        var messageEvents = new MessageEvents();
        var interactionEvents = new InteractionEvents();
        var musicEvents = new MusicEvents();
        var jsonConfig = JsonConvert.SerializeObject(settings);

        File.Open(ConfigFileName, FileMode.OpenOrCreate).Close();
        File.WriteAllText(ConfigFileName, jsonConfig);

        Console.WriteLine(settings.BotToken);

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

        Instance.Container.MusicEvents.OnMusicFinished += musicEvents.OnOnMusicFinished;


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