using DSharpPlus;
using DSharpPlus.Entities;

namespace MozartCakma.Service.Bot;

public interface IVoiceChannelService
{
    public Task JoinChannel(DiscordMember member, DiscordGuild guild, DiscordClient client);
    public void DisconnectChannel(DiscordGuild guild);
    public Task PlayAudio(DiscordGuild guild, DiscordChannel channel, string link);
    public void StopAudio(DiscordGuild guild);
    public void ResumeAudio(DiscordGuild guild);
    public void PauseAudio(DiscordGuild guild);
}