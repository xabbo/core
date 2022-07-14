using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

namespace Xabbo.Core;

/// <summary>
/// Defines basic information of a hotel.
/// </summary>
public class Hotel
{
    /// <summary>
    /// Represents an unknown hotel.
    /// </summary>
    public static readonly Hotel Unknown = new() { Name = "Unknown" };

    /// <summary>
    /// Contains the definitions of all hotels.
    /// </summary>
    public static readonly ImmutableArray<Hotel> All;

    static Hotel()
    {
        string fileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Environment.ExpandEnvironmentVariables(@"%APPDATA%\xabbo\hotels.json")
            : Environment.ExpandEnvironmentVariables(@"%HOME%/.xabbo/hotels.json");

        FileInfo file = new(fileName);

        List<Hotel>? hotels = null;

        if (file.Exists)
        {
            try
            {
                string json = File.ReadAllText(file.FullName);
                hotels = JsonSerializer.Deserialize<List<Hotel>>(json);
            }
            catch { }
        }

        if (hotels is null ||
            hotels.Count == 0)
        {
            hotels = new List<Hotel>()
            {
                new("Sandbox", "s2", subdomain: "sandbox"),
                new("US", "us"),
                new("Spain", domain: "es"),
                new("Finland", domain: "fi"),
                new("Italy", domain: "it"),
                new("Netherlands", domain: "nl"),
                new("Germany", domain: "de"),
                new("France", domain: "fr"),
                new("Brazil", identifier: "br", domain: "com.br"),
                new("Turkey", identifier: "tr", domain: "com.tr")
            };
        }

        All = hotels.ToImmutableArray();
    }

    /// <summary>
    /// Gets the name of this hotel.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the identifier of this hotel.
    /// </summary>
    public string Identifier { get; init; } = string.Empty;

    /// <summary>
    /// Gets the subdomain of this hotel.
    /// </summary>
    public string Subdomain { get; init; } = string.Empty;

    /// <summary>
    /// Gets the domain of this hotel.
    /// </summary>
    public string Domain { get; init; } = string.Empty;

    /// <summary>
    /// Gets the web host for this hotel.
    /// </summary>
    public string Host { get; init; } = string.Empty;

    /// <summary>
    /// Gets the game host for this hotel.
    /// </summary>
    public string GameHost { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new hotel instance.
    /// </summary>
    public Hotel() { }

    /// <summary>
    /// Creates a new hotel instance.
    /// </summary>
    public Hotel(
        string name, string? identifier = null,
        string subdomain = "www", string domain = "com",
        string? host = null, string? gameHost = null)
    {
        Name = name;
        Identifier = identifier ?? domain;
        Subdomain = subdomain;
        Domain = domain;
        Host = host ?? $"{subdomain}.habbo.{domain}";
        GameHost = gameHost ?? $"game-{identifier}.habbo.com";
    }

    /// <summary>
    /// Gets the hotel with the specified identifier. (ex. <c>us</c>)
    /// </summary>
    public static Hotel FromIdentifier(string identifier) => All.FirstOrDefault(x => x.Identifier == identifier)
        ?? throw new Exception($"Unknown hotel identifier: \"{identifier}\".");

    /// <summary>
    /// Gets the hotel with the specified domain. (ex. <c>com</c>)
    /// </summary>
    public static Hotel FromDomain(string domain) => All.FirstOrDefault(x => x.Domain == domain)
        ?? throw new Exception($"Unknown hotel domain: \"{domain}\".");

    /// <summary>
    /// Gets the hotel with the specified host. (ex. <c>www.habbo.com</c>)
    /// </summary>
    public static Hotel FromHost(string host)
    {
        if (host.StartsWith("habbo."))
            host = "www." + host;
        return All.FirstOrDefault(x => x.Host.Equals(host, StringComparison.OrdinalIgnoreCase))
            ?? throw new Exception($"Unknown hotel host: \"{host}\".");
    }

    /// <summary>
    /// Gets the hotel with the specified game host. (ex. <c>game-us.habbo.com</c>)
    /// </summary>
    public static Hotel FromGameHost(string gameHost) => All.FirstOrDefault(x => x.GameHost.Equals(gameHost, StringComparison.OrdinalIgnoreCase))
        ?? throw new Exception($"Unknown game host: \"{gameHost}\".");
}
