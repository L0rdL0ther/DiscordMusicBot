namespace MozartCakma.Dto;

public class YtVideoDto(string title, int lenght, string thumbnail, string formatedLenght, string videoId)
{
    public string FormatedLenght = formatedLenght;
    public int Lenght = lenght;
    public string Thumbnail = thumbnail;
    public string Title = title;
    public string VideoId = videoId;
}