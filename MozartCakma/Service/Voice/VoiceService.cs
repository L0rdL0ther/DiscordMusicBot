using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.VoiceNext;
using MozartCakma.Service.Voice;

namespace MozartCakma.Service.Bot;

public class VoiceService : IVoiceService
{
    private readonly Dictionary<ulong, VoiceNextConnection>
        _connections = new();

    public Dictionary<ulong, Process> DownloadProccess = new();

    public Dictionary<ulong, Process> FfmpegProcesses = new();


    public async Task JoinChannel(DiscordMember member, DiscordGuild guild, DiscordClient client)
    {
        var bot = guild.Members[client.CurrentUser.Id];
        if (!_connections.TryGetValue(guild.Id, out var connection) || connection == null)
        {
            connection = await member.VoiceState.Channel.ConnectAsync();
            _connections[guild.Id] = connection;
            return;
        }

        if (bot.VoiceState == null || bot.VoiceState.Channel.Id == null)
            _connections[guild.Id] = await member.VoiceState.Channel.ConnectAsync();
        if (member.VoiceState.Channel.Id == bot.VoiceState.Channel.Id) return;
        connection.Disconnect();
        _connections[guild.Id] = await member.VoiceState.Channel.ConnectAsync();
    }

    public async Task PlayAudio(DiscordGuild guild, DiscordChannel channel, string link)
    {
        var connection = _connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.GetTransmitSink();
            await transmit.FlushAsync();
            var pcm = await ConvertAudioToPcm(guild.Id, channel, link);
            await pcm.CopyToAsync(transmit);
            await pcm.DisposeAsync();
            Program.Instance.Container.Instance.MusicEvents.InvokeOnMusicFinished(channel, guild);
        }
    }

    public void StopAudio(DiscordGuild guild)
    {
        var connection = _connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.GetTransmitSink();
            transmit.FlushAsync();
            connection.Disconnect();
            _connections.Remove(guild.Id);
            if (FfmpegProcesses.ContainsKey(guild.Id))
                FfmpegProcesses[guild.Id].Kill();
            if (DownloadProccess.ContainsKey(guild.Id))
                DownloadProccess[guild.Id].Kill();
            Program.Instance.Container.TrackService.ClearTrackAsync(guild);
        }
    }

    public void ResumeAudio(DiscordGuild guild)
    {
        var connection = _connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.GetTransmitSink();
            transmit.ResumeAsync();
        }
    }

    public void PauseAudio(DiscordGuild guild)
    {
        var connection = _connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.GetTransmitSink();
            transmit.Pause();
        }
    }

    public void DisconnectChannel(DiscordGuild guild)
    {
        var connection = _connections[guild.Id];
        if (connection != null)
        {
            connection.Disconnect();
            _connections.Remove(guild.Id);
        }
    }

    #region Events

    public async Task HandleVoiceStateUpdated(DiscordClient client, VoiceStateUpdatedEventArgs e)
    {
        if (!_connections.ContainsKey(e.Guild.Id) || _connections[e.Guild.Id] == null)
            return;
        if (e.User.Id != client.CurrentUser.Id) return;
        var connection = _connections[e.Guild.Id];
        var guildId = e.Guild.Id;
        if (e.After.Channel != null && e.Before.Channel != null && e.After.Channel.Id != e.Before.Channel.Id)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            StopAudio(e.Guild);
        }
        else if (e.After.Channel == null)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            StopAudio(e.Guild);
        }
    }

    #endregion

    #region FileFormat

    public async Task<Stream> ConvertAudioToPcm(ulong guildId, DiscordChannel channel, string url)
    {
        var downloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
        Directory.CreateDirectory(downloadsDirectory);
        var randomFileName = Path.GetRandomFileName() + ".mp3";
        var outputFilePath = Path.Combine(downloadsDirectory, randomFileName);

        if (FfmpegProcesses.ContainsKey(guildId))
            FfmpegProcesses[guildId].Kill();

        if (DownloadProccess.ContainsKey(guildId))
            DownloadProccess[guildId].Kill();

        Console.WriteLine("Trying to install");
        var ytDlp = Process.Start(new ProcessStartInfo
        {
            FileName = "yt-dlp",
            Arguments = $"-f bestaudio --extract-audio --audio-format mp3 --output \"{outputFilePath}\" {url}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = false // Pencere açmasın
        });

        DownloadProccess[guildId] = ytDlp;
        await Task.Run(() => ytDlp?.WaitForExit());
        var ffmpeg = Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-i \"{outputFilePath}\" -ac 2 -b:a 192k -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        FfmpegProcesses[guildId] = ffmpeg;

        await Task.Delay(1000);
        if (File.Exists(outputFilePath))
            File.Delete(outputFilePath);
        else
            await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(new DiscordEmbedBuilder
            {
                Title = "Error",
                Description = "Invalid Link",
                Color = DiscordColor.Red
            }));

        //AudioProcesses[guildId] = ffmpeg.StandardOutput.BaseStream;

        return ffmpeg.StandardOutput.BaseStream;
    }

    #endregion
}