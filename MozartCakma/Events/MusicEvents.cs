using DSharpPlus.Entities;
using MozartCakma.Service;

namespace MozartCakma.Events;

public class MusicEvents
{
    public Program Main = Program.Instance;
    public Container Container = Program.Instance.Container.Instance;
    public void OnOnMusicFinished(object sender,DiscordChannel channel , DiscordGuild e)
    {
        var voiceManager = Container.VoiceService;
        var trackManager = Container.TrackService;
        var track = trackManager.GetTrackAsync(e).Result;
        var musicInfo = track.MusicsInfo.First();

        if (musicInfo != null && musicInfo.Playing)
        {
            track.MusicsInfo.Remove(musicInfo);
            var music = track.MusicsInfo.First();
            if (music != null)
            {
                Console.WriteLine("test2");
                music.Playing = true;
                voiceManager.PlayAudio(e,channel,music.Url);
                trackManager.UpdateTrack(e, track.MusicsInfo);
            }
            else
            {
                Console.WriteLine("test");
                channel.SendMessageAsync("Track ended");
            }
        }
        
    }
}