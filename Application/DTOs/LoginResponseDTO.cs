﻿namespace DiscordCloneBackend.Application.DTOs
{
    public class LoginResponseDTO
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserResponseDTO User { get; set; }
    }
}
