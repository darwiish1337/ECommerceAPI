﻿namespace Accounts.Application.DTOs.Requests.Auth;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
