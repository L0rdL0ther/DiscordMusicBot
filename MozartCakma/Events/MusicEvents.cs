using DSharpPlus.Entities;
using MozartCakma.Service;

namespace MozartCakma.Events;

public class MusicEvents
{
    public Container Container = Program.Instance.Container.Instance;
    public Program Main = Program.Instance;

    public void OnOnMusicFinished(object sender, DiscordChannel channel, DiscordGuild e)
    {
        try
        {
            var voiceManager = Container.VoiceService;
            var trackManager = Container.TrackService;

            // Guild için track bilgilerini alıyoruz
            var track = trackManager.GetTrackAsync(e).Result;
            if (track == null || track.MusicsInfo.Count == 0)
            {
                Console.WriteLine("No track found or music queue is empty.");
                channel.SendMessageAsync("No more music in the queue. Track ended.");
                trackManager.ClearTrackAsync(e);
                return; // Eğer track yoksa ya da müzik listesi boşsa, işlem durur
            }

            var musicInfo = track.MusicsInfo.First(); // İlk müzik bilgisi alınır (null check yapıldı)

            if (musicInfo == null)
            {
                Console.WriteLine("No music info available.");
                return; // Eğer müzik bilgisi yoksa, işlem sonlanır
            }

            // Eğer müzik çalmıyorsa veya looped değilse
            if (!musicInfo.Playing || !musicInfo.Looped && !musicInfo.Skipping)
            {
                var tryingToSkip = trackManager.SkipTrackAsync(e, channel).Result;

                if (tryingToSkip == null)
                {
                    // Müzik bitti, kuyruğa eklenen başka müzik yoksa
                    channel.SendMessageAsync("No more music in the queue. Track ended.");
                }
                else
                {
                    // Yeni eklenen müzik varsa, video bilgilerini al ve embed gönder
                    var videoInfo = Main.Container.YoutubeService.GetVideoInfoAsync(tryingToSkip.Url).Result;
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = $"Title: {videoInfo.Title}",
                        Url = $"https://www.youtube.com/watch?v={videoInfo.VideoId}",
                        Description = $"Video Length: {videoInfo.FormatedLenght}",
                        ImageUrl = videoInfo.Thumbnail,
                        Color = DiscordColor.Cyan
                    };

                    channel.SendMessageAsync(embed);
                }
            }
            // Eğer müzik looped (dönme) modundaysa, aynı müzik tekrar çalınsın
            else if (musicInfo.Looped && musicInfo.Playing)
            {
                voiceManager.PlayAudio(e, channel, musicInfo.Url); // Aynı müzik çalıyor
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred: {ex.Message}");
            channel.SendMessageAsync("An error occurred while processing the music event."); // Hata mesajı gönder
        }
    }
}

/*

Console.WriteLine("Invoked music event");

   var voiceManager = Container.VoiceService;
   var trackManager = Container.TrackService;

   // Synchronously retrieving track details
   var track = trackManager.GetTrackAsync(e).Result; // Making an async call synchronous
   var musicInfo = track.MusicsInfo.FirstOrDefault(); // Getting the first music info (with null check)

   if (musicInfo == null)
   {
       Console.WriteLine("No music info available.");
       return; // If no music info is available, terminate the process
   }

   // If the music is playing and it's not on loop, start playing the next music
   if (musicInfo.Playing && !musicInfo.Looped)
   {
        // Remove the current playing music from the list
       var music = track.MusicsInfo.FirstOrDefault(); // Check if there is another music in the list

       if (track.MusicsInfo.Count >1 && music != null)
       {
           Console.WriteLine("Starting next track");
           music.Playing = true;
           Thread.Sleep(500); // Wait for 500ms (to ensure synchronous processing)
           voiceManager.PlayAudio(e, channel, music.Url); // Start playing the next music
           trackManager.UpdateTrack(e, track.MusicsInfo); // Update the track details

           var videoInfo = Main.Container.YoutubeService.GetVideoInfoAsync(music.Url).Result;
           var embed = new DiscordEmbedBuilder
           {
               Title = $"Title: {videoInfo.Title}" ,
               Url = $"https://www.youtube.com/watch?v={videoInfo.VideoId}",
               Description =  $"Video Length: {videoInfo.FormatedLenght}",
               ImageUrl = videoInfo.Thumbnail ,
               Color = DiscordColor.Cyan,
           };


           channel.SendMessageAsync(embed: embed);

       }
       else
       {
           Console.WriteLine("No more music in the queue");
           channel.SendMessageAsync(
               "No more music in the queue. Track ended."); // Send message if no more music in the queue

           Main.Container.TrackService.ClearTrackAsync(e);

       }
   }
   // If the music is set to loop, it will play the same music again
   else if (musicInfo.Looped && musicInfo.Playing)
   {
       Console.WriteLine("Looping current track");
       voiceManager.PlayAudio(e, channel, musicInfo.Url); // Loop the current music
   }

   // If there are no more tracks in the list, send a "Track ended" message
   if (track.MusicsInfo.FirstOrDefault() == null)
   {
       Console.WriteLine("Track ended.");
       //channel.SendMessageAsync("Track ended."); // Notify that the track has ended
   }
   else
   {
       Console.WriteLine("No Null, tracks remaining."); // If there are still tracks left, log it
   }
*/