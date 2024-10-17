using MozartCakma.Service.Bot;
using MozartCakma.Service.youtube;

namespace MozartCakma.Service;

public class Container
{
    public VoiceChannelManagerService VoiceChannelManagerService = new();
    public YoutubeService YoutubeService = new();
    public static Container Instance { get; } = new();
}