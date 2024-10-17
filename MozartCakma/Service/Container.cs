using MozartCakma.Service.Bot;
using MozartCakma.Service.youtube;

namespace MozartCakma.Service;

public class Container
{
    public VoiceChannelService VoiceChannelService = new();
    public YoutubeService YoutubeService = new();
    public static Container Instance { get; } = new();
}