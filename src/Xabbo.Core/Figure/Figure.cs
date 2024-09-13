using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Xabbo.Core;

public class Figure
{
    private static readonly char[] ItemSeparator = ['.'];
    private static readonly Dictionary<int, Gender> GenderMap;
    private const string FIGUREPART_GENDERS_RESOURCE_PATH = "Xabbo.Core.Resources.figure_part_genders";

    static Figure()
    {
        var dictionary = new Dictionary<int, Gender>();

        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FIGUREPART_GENDERS_RESOURCE_PATH);
        if (stream is not null)
        {
            using (stream)
            using (StreamReader reader = new(stream))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Gender gender;

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] split = line.Split(['/']);
                    if (split.Length != 2) continue;
                    if (!int.TryParse(split[0], out int partId)) continue;

                    switch (split[1])
                    {
                        case "M": gender = Gender.Male; break;
                        case "F": gender = Gender.Female; break;
                        case "U": gender = Gender.Unisex; break;
                        default: continue;
                    }

                    dictionary[partId] = gender;
                }
            }
        }

        GenderMap = dictionary;
    }

    public static bool TryGetGender(int partId, out Gender gender)
        => GenderMap.TryGetValue(partId, out gender);

    public static bool TryGetGender(Figure figure, out Gender gender)
    {
        foreach (var part in figure.Parts)
        {
            if (TryGetGender(part.Id, out gender) &&
                gender != Gender.Unisex)
            {
                return true;
            }
        }
        gender = Gender.Unisex;
        return false;
    }

    public static bool TryGetGender(string figureString, out Gender gender)
    {
        string[] parts = figureString.Split(ItemSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (string part in parts)
        {
            int start = part.IndexOf('-');
            if (start < 0) continue;
            int end = part.IndexOf('-', start + 1);

            string partIdString;
            if (end < 0)
                partIdString = part[(start + 1)..];
            else
                partIdString = part[(start + 1)..end];

            if (!int.TryParse(partIdString, out int partId)) continue;
            if (TryGetGender(partId, out gender) && gender != Gender.Unisex) return true;
        }

        gender = Gender.Unisex;
        return false;
    }

    private Gender gender = Gender.Unisex;
    public Gender Gender
    {
        get => gender;
        set
        {
            gender = value switch
            {
                Gender.Male or Gender.Female or Gender.Unisex => value,
                _ => throw new ArgumentException($"Invalid gender: {value}."),
            };
        }
    }

    private readonly List<FigurePart> parts = new();

    public IReadOnlyList<FigurePart> Parts { get; }

    public FigurePart? this[FigurePartType type] => parts.FirstOrDefault(part => part.Type == type);


    public Figure()
    {
        Parts = parts.AsReadOnly();
    }

    public Figure(Gender gender)
        : this()
    {
        Gender = gender;
    }

    public bool ContainsPart(FigurePartType type) => parts.Any(part => part.Type == type);

    public void AddPart(FigurePart part)
    {
        var existingPart = this[part.Type];
        if (existingPart != null)
            parts.Remove(existingPart);
        parts.Add(part);
    }

    public bool RemovePart(FigurePartType partType)
    {
        var part = this[partType];
        if (part == null) return false;
        return parts.Remove(part);
    }

    public void RemovePart(string partType) => RemovePart(H.GetFigurePartType(partType));

    public string GetGenderString() => gender.ToClientString();

    public string GetFigureString() => ToString();

    public static Figure ParseString(string figureString)
    {
        var figure = new Figure();
        var figurePartStrings = figureString.Split(ItemSeparator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var figurePartString in figurePartStrings)
        {
            var figurePart = FigurePart.ParseString(figurePartString);
            if (figure[figurePart.Type] != null)
                throw new FormatException($"Duplicate figure part type '{figurePart.Type.ToShortString()}'");
            figure.AddPart(figurePart);
        }

        if (TryGetGender(figure, out Gender gender))
            figure.Gender = gender;

        return figure;
    }

    public static bool TryParse(string figureString, [NotNullWhen(true)] out Figure? figure)
    {
        figure = null;
        var tempFigure = new Figure();
        var figurePartStrings = figureString.Split(new char[] { '.' });
        foreach (var figurePartString in figurePartStrings)
        {
            if (!FigurePart.TryParseString(figurePartString, out var figurePart))
                return false;
            if (tempFigure[figurePart.Type] != null)
                return false;
            tempFigure.AddPart(figurePart);
        }

        if (TryGetGender(tempFigure, out Gender gender))
            tempFigure.Gender = gender;

        figure = tempFigure;
        return true;
    }

    public override string ToString() => string.Join(".", parts.Select(x => x.ToString()));

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 647;

            hash = (hash * 47) + (int)Gender;
            foreach (var part in Parts.OrderBy(x => x.Type))
                hash = (hash * 47) + part.GetHashCode();

            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Figure other ||
            other.Gender != Gender ||
            other.Parts.Count != Parts.Count)
            return false;

        return
            other.Gender == Gender &&
            other.Parts.Count == Parts.Count &&
            other.Parts.All(part => part.Equals(this[part.Type]));
    }
}
