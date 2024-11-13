using DSharpPlus.Entities;
using MozartCakma.Dto;

namespace MozartCakma.Service.Track;

public class TrackService : ITrackService
{
    public List<TrackDto> GuildTracks = new();

    public Program Main = Program.Instance;

    public async Task<MusicInfo?> PlayAsync(DiscordGuild guild, DiscordChannel channel, string url)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        var videoInfo = await Main.Container.YoutubeService.GetVideoInfoAsync(url); // Asenkron çağrı
        if (videoInfo == null) return null; // Video bilgisi alınamadıysa null dönebilir
        var music = new MusicInfo
        {
            Looped = false,
            Title = "",
            Url = url,
            FormatedLenght = videoInfo.FormatedLenght,
            Playing = guildTrack == null
        };

        if (guildTrack == null)
        {
            Main.Container.VoiceService.PlayAudio(guild, channel, url); // Asenkron çağrı
            GuildTracks.Add(new TrackDto
            {
                GuildId = guild.Id,
                MusicsInfo = new List<MusicInfo?> { music }
            });
        }
        else
        {
            guildTrack.MusicsInfo.Add(music);
        }

        return music;
    }


    public Task<bool> ClearTrackAsync(DiscordGuild guild)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            guildTrack.MusicsInfo.Clear();
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public async Task<MusicInfo?> SkipTrackAsync(DiscordGuild guild, DiscordChannel channel)
    {
        var container = Main.Container;
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            guildTrack.MusicsInfo.Remove(guildTrack.MusicsInfo.First());
            await container.VoiceService.PlayAudio(guild, channel, guildTrack.MusicsInfo.First().Url); // Asenkron çağrı
            return guildTrack.MusicsInfo.First();
        }

        return null;
    }

    public Task<TrackDto> GetTrackAsync(DiscordGuild guild)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null) return Task.FromResult(guildTrack);

        return Task.FromResult<TrackDto>(null);
    }

    public Task<TrackDto> RemoveMusicFromTrack(DiscordGuild guild, MusicInfo musicInfo)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            guildTrack.MusicsInfo.Remove(musicInfo);
            return Task.FromResult(guildTrack);
        }

        return Task.FromResult<TrackDto>(null);
    }

    public Task<TrackDto> UpdateTrack(DiscordGuild guild, List<MusicInfo> musicInfo)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            guildTrack.MusicsInfo = musicInfo;
            return Task.FromResult(guildTrack);
        }

        return Task.FromResult<TrackDto>(null);
    }
}