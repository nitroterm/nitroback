using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Nitroterm.Backend.Database.Models;

public class UserToUserInteraction
{
    public int Id { get; set; }
    public User User { get; set; }
    public UserToUserInteractionType Type { get; set; }
}

public enum UserToUserInteractionType
{
    None,
    Follow,
    Block
}