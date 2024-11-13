using System;
using System.IO;
using System.Threading.Tasks;
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
        Settings settings = null;
        
        try
        {
            var rawConfigJson = File.ReadAllText(ConfigFileName);
            Settings configJson = JsonConvert.DeserializeObject<Settings>(rawConfigJson);
            settings = configJson == null ? new Settings() : configJson;
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
        
        Instance = new Program(new Container(), settings); // Burada Instance atanıyor
        Instance.Container.Initialize();
        
        File.Open(ConfigFileName, FileMode.OpenOrCreate).Close();
        var jsonConfig = JsonConvert.SerializeObject(settings);
        File.WriteAllText(ConfigFileName, jsonConfig);
        
        settings.BotToken = settings.BotToken == "YOUR_TOKEN_HERE" 
            ? System.Environment.GetEnvironmentVariable("BOT_TOKEN") ?? settings.BotToken 
            : settings.BotToken;

        settings.RapidKey = settings.RapidKey == "YOUR_API_KEY_HERE" 
            ? System.Environment.GetEnvironmentVariable("RAPID_KEY") ?? settings.RapidKey 
            : settings.RapidKey;
        
        Console.WriteLine("Your bot token: "+settings.BotToken);
        Console.WriteLine("Your api key: "+settings.RapidKey);
        
        var messageEvents = new MessageEvents();
        var interactionEvents = new InteractionEvents();
        var musicEvents = new MusicEvents();
        
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