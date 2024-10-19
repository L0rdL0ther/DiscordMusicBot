using DSharpPlus.Entities;

namespace MozartCakma.EventService;

public class MusicEvents
{
    public event Events.Handler.EventHandler.MusicFinishedEventHandler? OnMusicFinished;

    public void InvokeOnMusicFinished(DiscordChannel c,DiscordGuild e)
    {
        OnMusicFinished?.Invoke(this, c,e);
    }
    
}