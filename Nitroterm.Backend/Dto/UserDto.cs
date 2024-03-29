﻿using Nitroterm.Backend.Database.Models;

namespace Nitroterm.Backend.Dto;

public class UserDto(User user)
{
    public int Id { get; } = user.Id;
    public string Username { get; } = user.Username;
    public ProductDto? Product { get; } = user.Product == null ? null : new ProductDto(user.Product);
}