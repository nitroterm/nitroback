using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nitroterm.Backend.Database.Models;

public class UserToPostInteraction
{
    public int Id { get; set; }
    public User SourceUser { get; set; }
    public Post Post { get; set; }
    public UserToPostInteractionType Type { get; set; }
}

public enum UserToPostInteractionType
{
    None,
    Nitro,
    Dynamite
}