using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xabbo.Core;

/// <summary>
/// Represents an avatar figure.
/// </summary>
public sealed class Figure : IEquatable<Figure>, IEnumerable<Figure.Part>
{
    private readonly Dictionary<FigurePartType, Part> _parts = [];
    /// <summary>
    /// Gets the parts contained in the figure.
    /// </summary>
    public IReadOnlyCollection<Part> Parts => _parts.Values;

    /// <summary>
    /// Gets or sets the gender of the figure.
    /// </summary>
    public Gender Gender { get; set; } = Gender.Unisex;

    /// <summary>
    /// Gets or sets the figure part of the specified type.
    /// </summary>
    /// <exception cref="ArgumentException">If the specified key differs from the part type when setting the part.</exception>
    public Part this[FigurePartType key]
    {
        get => _parts[key];
        set
        {
            if (value.Type != key)
            {
                throw new ArgumentException(
                    $"Attempt to set {nameof(FigurePartType)}.{key} when "
                    + $"{nameof(value)}.{nameof(Part.Type)} is {value.Type}",
                    nameof(value)
                );
            }
            _parts[key] = value;
        }
    }

    public Figure() { }

    public Figure(Gender gender)
    {
        Gender = gender;
    }

    public Figure(Gender gender, IEnumerable<Part> parts)
    {
        Gender = gender;
        foreach (var part in parts)
            Add(part);
    }

    /// <summary>
    /// Gets whether the figure has a part of the specified type.
    /// </summary>
    public bool Has(FigurePartType type) => _parts.ContainsKey(type);

    /// <summary>
    /// Adds the specified part to the figure.
    /// </summary>
    public void Add(Part part) => _parts.Add(part.Type, part);

    /// <summary>
    /// Adds the specified part type and ID to the figure.
    /// </summary>
    public void Add(FigurePartType type, int id) => _parts.Add(type, new Part(type, id));

    /// <summary>
    /// Adds the specified part type, ID, and colors to the figure.
    /// </summary>
    public void Add(FigurePartType type, int id, params int[] colors) => _parts.Add(type, new Part(type, id, colors));

    /// <summary>
    /// Attempts to get the part of the specified type in the figure.
    /// </summary>
    public bool TryGetPart(FigurePartType type, [NotNullWhen(true)] out Part? part) => _parts.TryGetValue(type, out part);

    /// <summary>
    /// Removes the specified part type from the figure.
    /// </summary>
    /// <param name="partType">The part type to remove.</param>
    /// <returns>Whether the part was removed.</returns>
    public bool Remove(FigurePartType partType) => _parts.Remove(partType);

    /// <summary>
    /// Composes the figure to a figure string.
    /// </summary>
    public override string ToString() => string.Join('.', _parts.Values.Select(x => x.ToString()));

    public override int GetHashCode()
    {
        int hashCode = Gender.GetHashCode();
        foreach (var part in Parts)
            hashCode = (hashCode, part).GetHashCode();
        return hashCode;
    }

    public override bool Equals(object? obj) => Equals(obj as Figure);

    public bool Equals(Figure? other) =>
        other is not null &&
        other.Gender == Gender &&
        other.Parts.SequenceEqual(Parts);

    /// <summary>
    /// Parses the specified figure string into a <see cref="Figure"/> .
    /// </summary>
    public static Figure Parse(string figureString)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(figureString);

        var figure = new Figure();
        var figurePartStrings = figureString.Split('.');
        foreach (var figurePartString in figurePartStrings)
            figure.Add(Part.Parse(figurePartString));

        if (TryGetGender(figure, out Gender gender))
            figure.Gender = gender;

        return figure;
    }

    /// <summary>
    /// Attempts to parse the specified figure string into a <see cref="Figure"/> .
    /// </summary>
    public static bool TryParse(string figureString, [NotNullWhen(true)] out Figure? figure)
    {
        figure = null;
        var tempFigure = new Figure();
        var figurePartStrings = figureString.Split('.');
        foreach (var figurePartString in figurePartStrings)
        {
            if (!Part.TryParseString(figurePartString, out var figurePart))
                return false;
            if (tempFigure[figurePart.Type] != null)
                return false;
            tempFigure.Add(figurePart);
        }

        if (TryGetGender(tempFigure, out Gender gender))
            tempFigure.Gender = gender;

        figure = tempFigure;
        return true;
    }

    /// <summary>
    /// Attempts to get the gender from the specified figure.
    /// </summary>
    /// <remarks>
    /// This works by detecting certain parts in the figure that belong to a specific gender.
    /// </remarks>
    public static bool TryGetGender(Figure figure, out Gender gender)
    {
        foreach (var part in figure.Parts)
        {
            if (FigurePartGenders.GenderMap.TryGetValue(part.Id, out gender) &&
                gender != Gender.Unisex)
            {
                return true;
            }
        }
        gender = Gender.Unisex;
        return false;
    }

    IEnumerator<Part> IEnumerable<Part>.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public sealed class Part : IEquatable<Part>
    {
        private static readonly char[] _partSeparator = ['-'];

        public FigurePartType Type { get; }

        public int Id { get; set; }
        public List<int> Colors { get; } = [];

        public Part(FigurePartType type)
        {
            Type = type;
        }

        public Part(FigurePartType type, int id)
        {
            Type = type;
            Id = id;
        }

        public Part(FigurePartType type, int id, params int[] colors)
        {
            Type = type;
            Id = id;
            Colors = [.. colors];
        }

        public override string ToString() =>
            $"{Type.ToShortString()}-{Id}"
            + (Colors.Count > 0 ? $"-{string.Join('-', Colors)}" : "");

        public static Part Parse(string figurePartString)
        {
            string[] split = figurePartString.Split(_partSeparator);
            if (split.Length < 1) throw new FormatException("Empty figure part");
            if (split.Length < 2) throw new FormatException($"Figure part '{split[0]}' has no ID");

            var partType = H.GetFigurePartType(split[0]);

            if (!int.TryParse(split[1], out int partId))
                throw new FormatException($"Couldn't parse ID '{split[1]}' for figure part '{split[0]}'");

            var figurePart = new Part(partType, partId);

            for (int i = 2; i < split.Length; i++)
            {
                if (!int.TryParse(split[i], out int color))
                    throw new FormatException($"Couldn't parse color '{split[i]}' for figure part '{split[0]}'");
                figurePart.Colors.Add(color);
            }

            return figurePart;
        }

        public static bool TryParseString(string figurePartString, [NotNullWhen(true)] out Part? figurePart)
        {
            figurePart = null;

            string[] split = figurePartString.Split(_partSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
                return false;

            if (!H.TryGetFigurePartType(split[0], out FigurePartType partType))
                return false;
            if (!int.TryParse(split[1], out int partId))
                return false;

            var figurePartTemp = new Part(partType, partId);

            for (int i = 2; i < split.Length; i++)
            {
                if (!int.TryParse(split[i], out int color))
                    return false;
                figurePartTemp.Colors.Add(color);
            }

            figurePart = figurePartTemp;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = (Type, Id).GetHashCode();
            foreach (var color in Colors)
                hash = (hash, color).GetHashCode();
            return hash;
        }

        public override bool Equals(object? obj) => Equals(obj as Part);

        public bool Equals(Part? other) =>
            other is not null &&
            other.Type == Type &&
            other.Id == Id &&
            other.Colors.SequenceEqual(Colors);

        public static implicit operator Part((FigurePartType type, int id) x) => new(x.type, x.id);
        public static implicit operator Part((FigurePartType type, int id, int color1) x)
            => new(x.type, x.id, x.color1);
        public static implicit operator Part((FigurePartType type, int id, int color1, int color2) x)
            => new(x.type, x.id, x.color1);
    }
}
