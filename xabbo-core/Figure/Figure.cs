using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Xabbo.Core
{
    public class Figure
    {
        private static readonly IReadOnlyDictionary<int, Gender> _genderMap;

        static Figure()
        {
            var dictionary = new Dictionary<int, Gender>();

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Xabbo.Core.Resources.figure_part_genders"))
            using (var reader = new StreamReader(stream))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int partId; Gender gender;

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    string[] split = line.Split(new char[] { '/' });
                    if (split.Length != 2) continue;
                    if (!int.TryParse(split[0], out partId)) continue;

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

            _genderMap = dictionary;
        }

        public static bool TryGetGender(int partId, out Gender gender)
            => _genderMap.TryGetValue(partId, out gender);

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
            string[] parts = figureString.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string part in parts)
            {
                int start = part.IndexOf('-');
                if (start < 0) continue;
                int end = part.IndexOf('-', start + 1);

                string partIdString;
                if (end < 0)
                    partIdString = part.Substring(start + 1);
                else
                    partIdString = part.Substring(start + 1, end - start - 1);

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
                switch (value)
                {
                    case Gender.Male:
                    case Gender.Female:
                    case Gender.Unisex:
                        gender = value;
                        break;
                    default: throw new ArgumentException($"Invalid gender: {value}");
                }
            }
        }

        private readonly List<FigurePart> parts = new List<FigurePart>();

        public IReadOnlyList<FigurePart> Parts { get; }

        public FigurePart this[FigurePartType type] => parts.FirstOrDefault(part => part.Type == type);

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

        public string GetGenderString() => gender.ToShortString();

        public string GetFigureString() => ToString();

        public static Figure Parse(string figureString)
        {
            var figure = new Figure();
            var figurePartStrings = figureString.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var figurePartString in figurePartStrings)
            {
                var figurePart = FigurePart.Parse(figurePartString);
                if (figure[figurePart.Type] != null)
                    throw new FormatException($"Duplicate figure part type '{figurePart.Type.ToShortString()}'");
                figure.AddPart(figurePart);
            }

            if (TryGetGender(figure, out Gender gender))
                figure.Gender = gender;

            return figure;
        }

        public static bool TryParse(string figureString, out Figure figure)
        {
            figure = null;
            var tempFigure = new Figure();
            var figurePartStrings = figureString.Split(new char[] { '.' });
            foreach (var figurePartString in figurePartStrings)
            {
                if (!FigurePart.TryParse(figurePartString, out var figurePart))
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

        public override bool Equals(object obj)
        {
            var other = obj as Figure;

            if (other is null ||
                other.Gender != Gender ||
                other.Parts.Count != Parts.Count)
                return false;

            return
                !ReferenceEquals(other, null) &&
                other.Gender == Gender &&
                other.Parts.Count == Parts.Count &&
                other.Parts.All(part => part.Equals(this[part.Type]));
        }
    }
}
