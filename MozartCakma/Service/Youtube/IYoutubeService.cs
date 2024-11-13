using MozartCakma.Dto;

namespace MozartCakma.Service.youtube;

public interface IYoutubeService
{
    public Task<List<YtVideoDto>> Search(string queryT);
    public Task<YtVideoDto?> GetVideoInfoAsync(string videoId);
}