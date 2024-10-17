namespace MozartCakma.Dto;

public class YtVideoInfo(string title, int lenght, string thumbnail, string fomatedLenght, string videoId)
{
    public string FomatedLenght = fomatedLenght;
    public int Lenght = lenght;
    public string Thumbnail = thumbnail;
    public string Title = title;
    public string VideoId = videoId;
}