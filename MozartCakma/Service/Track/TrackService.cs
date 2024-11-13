using DSharpPlus.Entities;
using MozartCakma.Dto;

namespace MozartCakma.Service.Track;

public class TrackService : ITrackService
{
    public List<TrackDto> GuildTracks = new();

    public Program Main = Program.Instance;

    public async Task<MusicInfo?> PlayAsync(DiscordGuild guild, DiscordChannel channel, string url)
    {
        // Guild'teki mevcut track'i bul
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);

        // URL'den video bilgilerini asenkron olarak al
        var videoInfo = await Main.Container.YoutubeService.GetVideoInfoAsync(url);
        if (videoInfo == null) return null; // Video bilgisi alınamadıysa null dönebilir

        // Yeni bir MusicInfo nesnesi oluştur
        var music = new MusicInfo
        {
            Looped = false,
            Title = videoInfo.Title,
            Url = url,
            FormatedLenght = videoInfo.FormatedLenght,
            Skipping = false,
            Playing = false // Yeni eklenen müzik başlangıçta oynatılmıyor
        };

        // Eğer guildTrack bulunmazsa, yani guild için hiç müzik yoksa
        if (guildTrack == null)
        {
            // Yeni track oluşturup ilk müziği çalmaya başla
            music.Playing = true; // İlk müzik oynatılacak, sadece buna true ver
            Main.Container.VoiceService.PlayAudio(guild, channel, url); // Asenkron çağrı

            // Yeni track'i listeye ekle
            GuildTracks.Add(new TrackDto
            {
                GuildId = guild.Id,
                MusicsInfo = new List<MusicInfo?> { music }
            });
        }
        else
        {
            // Eğer guildTrack mevcutsa ve içinde hiç müzik yoksa, ilk müzik oynatılır
            if (guildTrack.MusicsInfo.Count < 1)
            {
                // İlk müzik ekleniyor, o zaman bu müzik çalınsın
                music.Playing = true; // İlk müzik oynatılacak, sadece buna true ver
                Main.Container.VoiceService.PlayAudio(guild, channel, url); // Asenkron çağrı
                guildTrack.MusicsInfo.Add(music); // Müzik ekleniyor
            }
            else
            {
                // Eğer başka müzikler eklenmişse, yeni müzik sadece listeye eklenir
                guildTrack.MusicsInfo.Add(music);
            }
        }

        // Eklenen müzik nesnesini döndür
        return music;
    }


    public Task<bool> ClearTrackAsync(DiscordGuild guild)
    {
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);

        guildTrack.MusicsInfo.Clear();
        return Task.FromResult(true);

        return Task.FromResult(false);
    }

    public async Task<MusicInfo?> SkipTrackAsync(DiscordGuild guild, DiscordChannel channel)
    {
        var container = Main.Container;
        var guildTrack = GuildTracks.Find(x => x.GuildId == guild.Id);

        // Eğer guildTrack mevcutsa, müzik listesini kontrol et
        if (guildTrack != null && guildTrack.MusicsInfo.Count > 0)
        {
            // İlk müziği kaldır (skip işlemi)
            var oldMUsic = guildTrack.MusicsInfo[0]; // İlk müziği çıkarıyoruz

            // Eğer listede hala müzik varsa, onu çal
            if (guildTrack.MusicsInfo.Count > 0)
            {
                oldMUsic.Skipping = true;
                var nextTrack = guildTrack.MusicsInfo[1]; // Yeni çalınacak müzik
                container.VoiceService.PlayAudio(guild, channel, nextTrack.Url); // Asenkron çağrı
                await Task.Delay(TimeSpan.FromSeconds(1));
                guildTrack.MusicsInfo.RemoveAt(0);
                return nextTrack;
            }

            // Eğer track listesi boşsa, sesli çalma durur
            // Burada herhangi bir şey yapmaya gerek yok çünkü müzik bitmiş olur
            ClearTrackAsync(guild);
            return null;
        }

        // Eğer guildTrack yoksa ya da müzik listesi boşsa, hiçbir şey yapma
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