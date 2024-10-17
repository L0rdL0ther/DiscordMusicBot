using System.Diagnostics;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace MozartCakma.Service.Bot;

public class VoiceChannelService : IVoiceChannelService
{
    private readonly Dictionary<ulong, Task<VoiceNextConnection>>
        connections = new();

    public Dictionary<ulong, Process> FfmpegProcesses = new();
    public Dictionary<ulong, Process> DownloadProccess = new();
    public async Task JoinChannel(DiscordMember member, DiscordGuild guild,DiscordClient client)
    {
        var bot = guild.Members[client.CurrentUser.Id];
        if (!connections.TryGetValue(guild.Id, out var connection) || connection == null)
        {
            connection = member.VoiceState.Channel.ConnectAsync();
            connections[guild.Id] = connection;
            return;
        }
        if(bot.VoiceState == null || bot.VoiceState.Channel.Id == null)
            connections[guild.Id] = member.VoiceState.Channel.ConnectAsync();
        if (member.VoiceState.Channel.Id == bot.VoiceState.Channel.Id)
        {
            return;
        }
        connection.Result.Disconnect();
        connections[guild.Id] = member.VoiceState.Channel.ConnectAsync();
        
    }

    public void DisconnectChannel()
    {
        throw new NotImplementedException();
    }

    public async Task PlayAudio(ulong guildId,DiscordChannel channel, string link)
    {
        var connection = connections[guildId];
        if (connection != null)
        {
            var transmit = connection.Result.GetTransmitSink();
            await transmit.FlushAsync();
            var pcm = await ConvertAudioToPcm(guildId,channel, link);
            await pcm.CopyToAsync(transmit);
            await pcm.DisposeAsync();
            await transmit.FlushAsync();
        }
    }

    public void StopAudio(DiscordGuild guild)
    {
        var connection = connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.Result.GetTransmitSink();
            transmit.FlushAsync();
            
            if (FfmpegProcesses.ContainsKey(guild.Id))
                FfmpegProcesses[guild.Id].Kill();
            if (DownloadProccess.ContainsKey(guild.Id))
                DownloadProccess[guild.Id].Kill();
        }
    }

    public void ResumeAudio(DiscordGuild guild)
    {
        var connection = connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.Result.GetTransmitSink();
            transmit.ResumeAsync();
        }
    }

    public void PauseAudio(DiscordGuild guild)
    {
        var connection = connections[guild.Id];
        if (connection != null)
        {
            var transmit = connection.Result.GetTransmitSink();
            transmit.Pause();
        }
    }

    public async Task<Stream> ConvertAudioToPcm(ulong guildId,DiscordChannel channel, string url)
    {
        
        string downloadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
        Directory.CreateDirectory(downloadsDirectory);
        string randomFileName = Path.GetRandomFileName() + ".mp3";
        string outputFilePath = Path.Combine(downloadsDirectory, randomFileName);
        
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
            Arguments = $"-i \"{outputFilePath}\" -ac 2 -f s16le -ar 48000 pipe:1",
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
        
        return ffmpeg.StandardOutput.BaseStream;
    }

}

/*
private Dictionary<ulong, Task<VoiceNextConnection>>
       connections = new Dictionary<ulong, Task<VoiceNextConnection>>();

   public Dictionary<ulong, Process> ffmpegProcesses = new Dictionary<ulong, Process>();

   public Task JoinChannel(CommandContext ctx)
   {

   }

   public void DisconnectChannel()
   {
       throw new NotImplementedException();
   }

   public Task PlayAudio(CommandContext ctx, string link)
   {
       throw new NotImplementedException();
   }

   public void StopAudio()
   {
       throw new NotImplementedException();
   }

   public void ResumeAudio()
   {
       throw new NotImplementedException();
   }

   public void PauseAudio()
   {
       throw new NotImplementedException();
   }


   private Dictionary<ulong, Task<VoiceNextConnection>>
       connection = new Dictionary<ulong, Task<VoiceNextConnection>>();
   public Process ffmpegProcess;

   public async Task JoinChannel(CommandContext ctx)
   {
       var bot = ctx.Guild.Members[ctx.Client.CurrentUser.Id];

       if (connection == null) await FirstConnect(ctx);

       if (connection.Result.TargetChannel.Id == bot.VoiceState.Channel.Id)
           return;



   }


   public async Task PlayAudio(CommandContext ctx,string link)
   {

       await JoinChannel(ctx);

       if(!CheckBotInVc(ctx))
           return;

       await connection.Result.ResumeAsync();
       var transmit = connection.Result.GetTransmitSink();
       var pcm = ConvertAudioToPcm("dayum.mp3");
       await pcm.CopyToAsync(transmit);
       await pcm.DisposeAsync();
   }

   public void StopAudio()
   {
       connection.Result.Pause();
       DisconnectChannel();
   }

   public void ResumeAudio()
   {
       connection.Result.ResumeAsync();
   }

   public void PauseAudio()
   {
       connection.Result.Pause();
   }

   public void DisconnectChannel()
   {
       ffmpegProcess.Kill();
       connection?.Result?.Disconnect();
   }

   #region Converter
   private Stream ConvertAudioToPcm(string filePath)
   {
       var ffmpeg = Process.Start(new ProcessStartInfo
       {
           FileName = "ffmpeg",
           Arguments = $@"-i ""{filePath}"" -ac 2 -f s16le -ar 48000 pipe:1",
           RedirectStandardOutput = true,
           UseShellExecute = false
       });
       ffmpegProcess = ffmpeg;
       return ffmpeg.StandardOutput.BaseStream;
   }

   #endregion

   #region EventsFunc

   private async Task FirstConnect(CommandContext ctx)
   {
       connection = ctx.Member.VoiceState.Channel.ConnectAsync();
       await connection;

       connection.Result.UserLeft += ResultOnUserLeft;
       connection.Result.VoiceSocketErrored += ResultOnVoiceSocketErrored;
   }

   private bool CheckBotInVc(CommandContext ctx)
   {
       var bot = ctx.Guild.Members[ctx.Client.CurrentUser.Id];
       if (connection == null)
           return false;

       if (connection.Result.TargetChannel.Id == bot.VoiceState.Channel.Id)
           return true;

       return false;
   }

   private Task ResultOnVoiceSocketErrored(VoiceNextConnection sender, SocketErrorEventArgs args)
   {
       DisconnectChannel();
       return Task.CompletedTask;
   }

   private Task ResultOnUserLeft(VoiceNextConnection sender, VoiceUserLeaveEventArgs args)
   {
       DisconnectChannel();
       return Task.CompletedTask;
   }

   #endregion


*/