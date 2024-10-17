using MozartCakma.Dto;

namespace MozartCakma.Service.youtube;

public interface IYoutubeService
{
    public Task<List<YtVideoInfo>> Search(string queryT);
    public Task<YtVideoInfo?> GetVideoInfo(string videoId);
}