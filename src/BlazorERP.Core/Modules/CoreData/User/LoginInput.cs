﻿namespace BlazorERP.Core.Modules.CoreData;
public class LoginInput
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
