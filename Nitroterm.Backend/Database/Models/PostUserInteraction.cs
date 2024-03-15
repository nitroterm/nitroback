using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Nitroterm.Backend.Database.Models;

public class PostUserInteraction
{
    public int Id { get; set; }
    public Post Post { get; set; }
    public PostUserInteractionType Type { get; set; }
}

public enum PostUserInteractionType
{
    None,
    Nitronize,
    Dynamite
}