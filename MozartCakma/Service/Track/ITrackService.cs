using DSharpPlus.Entities;
using MozartCakma.Dto;
using MozartCakma.enums;

namespace MozartCakma.Service.Track;

public interface ITrackService
{
    public MusicInfo Play(DiscordGuild guild, DiscordChannel channel, string url);
    public TrackEnum ClearTrack(DiscordGuild guild);
    public MusicInfo SkipTrack(DiscordGuild guild, DiscordChannel channel);
    public MusicInfo GetTrack(DiscordGuild guild);
}