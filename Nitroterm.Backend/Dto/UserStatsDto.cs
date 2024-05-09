namespace Nitroterm.Backend.Dto;

public class UserStatsDto
{
    public int FollowCount { get; set; }
}

public class PostStatsDto
{
    public int NitroCount { get; set; }
    public int DynamiteCount { get; set; }
    public int ForkCount { get; set; }
}