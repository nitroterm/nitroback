﻿using System.ComponentModel.DataAnnotations;

namespace Nitroterm.Backend.Dto;

public class LoginDto
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public string? ReCaptchaChallenge { get; set; }
    public string? FirebaseToken { get; set; }
}