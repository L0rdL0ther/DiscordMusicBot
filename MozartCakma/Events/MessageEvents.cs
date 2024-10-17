using DSharpPlus;
using DSharpPlus.EventArgs;

namespace MozartCakma.Events;

public class MessageEvents
{
    public Task MessageCreatedHandler(DiscordClient s, MessageCreatedEventArgs e)
    {
        return Task.CompletedTask;
    }
}