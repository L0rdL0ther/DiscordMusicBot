using DSharpPlus.Entities;
using MozartCakma.Dto;
using MozartCakma.enums;
using MozartCakma.Service.youtube;

namespace MozartCakma.Service.Track;

public class ATrackService : ITrackService
{
    public List<TrackDto> GuildTracks = new();

    public Program Main = Program.Instance;

    public MusicInfo Play(DiscordGuild guild, DiscordChannel channel, string url)
    {
        Console.WriteLine("ney niga ney niga niga");
        Console.WriteLine("ney");
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        Console.WriteLine(url);
        var youtubeService = new YoutubeService();
        var videoInfo = youtubeService.GetVideoInfo(url);
        Console.WriteLine("NIGGGERS");
        Console.WriteLine(videoInfo.Title);
        if (videoInfo == null) return null; // Video bilgisi alınamadıysa null dönebilir
        Console.WriteLine("sen null ney");
        var music = new MusicInfo
        {
            Looped = false,
            Title = videoInfo.Title,
            Url = url, // "FormatedLenght"
            FormatedLenght = videoInfo.FormatedLenght, // "FomatedLenght" yerine "FormatedLenght"
            Playing = guildTrack == null // Yeni parça oynatılıyor mu?
        };
        Console.WriteLine("NIGGGERS 2");
        if (guildTrack == null)
        {
            Console.WriteLine("Null check");
            Console.WriteLine(Main == null ? "Main" : "Main is null");
            Console.WriteLine(Main?.Container == null
                ? "Container"
                : "Container is null"); // Null kontrolü için ? kullanımı
            Console.WriteLine(Main?.Container.VoiceService == null ? "VoiceService" : "VoiceService is null");
            Main?.Container?.VoiceService?.PlayAudio(guild, channel, url);
            Console.WriteLine("NIGGGERS 222");
            GuildTracks.Add(new TrackDto
            {
                GuildId = guild.Id,
                MusicsInfo = new List<MusicInfo> { music }
            });
        }
        else
        {
            Console.WriteLine("NIGGGERS 3");
            guildTrack.MusicsInfo.Add(music);
        }

        Console.WriteLine(music.Looped);
        return music;
    }


    public TrackEnum ClearTrack(DiscordGuild guild)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null) guildTrack.MusicsInfo.Clear();
        return TrackEnum.Error;
    }

    public MusicInfo SkipTrack(DiscordGuild guild, DiscordChannel channel)
    {
        var container = Main.Container;
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            guildTrack.MusicsInfo.Remove(guildTrack.MusicsInfo.First());
            container.VoiceService.PlayAudio(guild, channel, guildTrack.MusicsInfo.First().Url);
            return guildTrack.MusicsInfo.First();
        }

        return null;
    }

    public MusicInfo GetTrack(DiscordGuild guild)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);
        if (guildTrack != null)
        {
            var playingTrack = guildTrack.MusicsInfo.First(x => x.Playing);
            return playingTrack;
        }

        return null;
    }
}