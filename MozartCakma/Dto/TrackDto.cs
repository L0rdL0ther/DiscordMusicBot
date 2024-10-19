namespace MozartCakma.Dto;

public class TrackDto
{
    public ulong GuildId { get; set; }
    public List<MusicInfo?> MusicsInfo { get; set; }
}

public class MusicInfo
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string FormatedLenght { get; set; }
    public bool Looped { get; set; }
    public bool Playing { get; set; }
}