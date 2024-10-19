using DSharpPlus.Entities;
using MozartCakma.Dto;

namespace MozartCakma.Service.Track;

public class TrackService : ITrackService
{
    public List<TrackDto> GuildTracks = new();

    public Program Main = Program.Instance;

    public async Task<MusicInfo?> PlayAsync(DiscordGuild guild, DiscordChannel channel,string url)
    {
        Console.WriteLine("çalma");
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        Console.WriteLine("asenkron");
        //var videoInfo = await Main.Container.YoutubeService.GetVideoInfoAsync(url); // Asenkron çağrı
        Console.WriteLine("asenkron2222");
        //Console.WriteLine(videoInfo.VideoId);
        //if (videoInfo == null) return null; // Video bilgisi alınamadıysa null dönebilir
        Console.WriteLine("çal");
        var music = new MusicInfo
        {
            Looped = false,
            Title = "",
            Url = url,
            //FormatedLenght = videoInfo.FormatedLenght,
            Playing = guildTrack == null
        };

        if (guildTrack == null)
        {
            Console.WriteLine("sorun bumu hocam");
            Main.Container.VoiceService.PlayAudio(guild, channel, url); // Asenkron çağrı
            Console.WriteLine("çal2222");
            GuildTracks.Add(new TrackDto
            {
                GuildId = guild.Id,
                MusicsInfo = new List<MusicInfo?> { music }
            });
        }
        else
        {
            Console.WriteLine("ase");
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