﻿namespace BlazorERP.Core.Extensions;

public static class StringExtensions
{
    /// <summary>
    /// Generates a random string
    /// </summary>
    /// <param name="length">The length of the random string</param>
    /// <returns></returns>
    public static string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new string(stringChars);
        return finalString;
    }

    public static string ParseUrl(string? url)
    {
        if (url is null)
        {
            return string.Empty;
        }

        url = url.Replace("https://", "");
        url = url.Replace("http://", "");
        url = url.Replace("www.", "");
        return $"//{url}";
    }
}
