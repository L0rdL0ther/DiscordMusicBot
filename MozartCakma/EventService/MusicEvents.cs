using DSharpPlus.Entities;
using EventHandler = MozartCakma.Events.Handler.EventHandler;

namespace MozartCakma.EventService;

public class MusicEvents
{
    public event EventHandler.MusicFinishedEventHandler? OnMusicFinished;

    public void InvokeOnMusicFinished(DiscordChannel c, DiscordGuild e)
    {
        OnMusicFinished?.Invoke(this, c, e);
    }
}