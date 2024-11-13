using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;

namespace MozartCakma.Service.Voice;

public interface IVoiceService
{
    public Task JoinChannel(DiscordMember member, DiscordGuild guild, DiscordClient client);
    public Task PlayAudio(DiscordGuild guild, DiscordChannel channel, string link);
    public void StopAudio(DiscordGuild guild);
    public void ResumeAudio(DiscordGuild guild);
    public void PauseAudio(DiscordGuild guild);
}