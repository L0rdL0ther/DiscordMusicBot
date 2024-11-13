using MozartCakma.EventService;
using MozartCakma.Service.Bot;
using MozartCakma.Service.Track;
using MozartCakma.Service.youtube;

namespace MozartCakma.Service;

public class Container
{
    public MusicEvents MusicEvents { get; private set; }
    public TrackService TrackService { get; private set; }
    public VoiceService VoiceService { get; private set; }
    public YoutubeService YoutubeService { get; private set; }

    // Instance'ı this olarak ayarlıyoruz
    public Container Instance { get; private set; }

    // Servislerin başlatılması için kullanılacak metod
    public void Initialize()
    {
        Instance = this;
        MusicEvents = new MusicEvents();
        TrackService = new TrackService(); // Servisi burada başlatıyoruz
        VoiceService = new VoiceService(); // Servisi burada başlatıyoruz
        YoutubeService = new YoutubeService(); // Servisi burada başlatıyoruz

        // Diğer gerekli başlangıç işlemleri burada yapılabilir
    }
}