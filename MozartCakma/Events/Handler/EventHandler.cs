using DSharpPlus.Entities;

namespace MozartCakma.Events.Handler;

public class EventHandler
{
    public delegate void MusicFinishedEventHandler(object sender, DiscordChannel c,DiscordGuild e);
}