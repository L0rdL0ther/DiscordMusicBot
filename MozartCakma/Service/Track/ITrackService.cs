using DSharpPlus.Entities;
using MozartCakma.Dto;

namespace MozartCakma.Service.Track;

public interface ITrackService
{
    Task<MusicInfo?> PlayAsync(DiscordGuild guild, DiscordChannel channel, string url);
    Task<bool> ClearTrackAsync(DiscordGuild guild);
    Task<MusicInfo?> SkipTrackAsync(DiscordGuild guild, DiscordChannel channel);
    Task<TrackDto> GetTrackAsync(DiscordGuild guild);
    
    Task<TrackDto> RemoveMusicFromTrack(DiscordGuild guild,MusicInfo musicInfo);
    
    Task<TrackDto> UpdateTrack(DiscordGuild guild,List<MusicInfo> musicInfo);
}