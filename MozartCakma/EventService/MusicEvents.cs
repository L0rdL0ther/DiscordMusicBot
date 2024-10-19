using MozartCakma.Events.Handler;

namespace MozartCakma.Service;

public class MusicEvents
{
    public event Events.Handler.EventHandler.MusicFinishedEventHandler? OnMusicFinished;
    
}