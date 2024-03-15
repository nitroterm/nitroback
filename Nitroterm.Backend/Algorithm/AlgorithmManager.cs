using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Algorithm;

public static class AlgorithmManager
{
    public static int DeduceInitialNitroLevelForPost(Post post, User sender)
    {
        return sender.NitroLevel;
    }
}