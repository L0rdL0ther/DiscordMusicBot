using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;

namespace MozartCakma.Service.Bot;

public abstract class IVoiceChannelService
{
    private Dictionary<ulong, VoiceNextConnection>
        _connections = new();
    public abstract Task JoinChannel(DiscordMember member, DiscordGuild guild, DiscordClient client);
    public abstract void DisconnectChannel(DiscordGuild guild);
    public abstract Task PlayAudio(DiscordGuild guild, DiscordChannel channel, string link);
    public abstract void StopAudio(DiscordGuild guild);
    public abstract void ResumeAudio(DiscordGuild guild);
    public abstract void PauseAudio(DiscordGuild guild);
}