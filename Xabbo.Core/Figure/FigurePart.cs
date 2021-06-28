using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Xabbo.Core
{
    public class FigurePart
    {
        public FigurePartType Type { get; }

        public int Id { get; set; }

        public List<int> Colors { get; }

        public FigurePart(FigurePartType type)
        {
            Type = type;
            Colors = new List<int>();
        }

        public FigurePart(FigurePartType type, int id)
            : this(type)
        {
            Id = id;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Type.ToShortString());
            sb.Append("-");
            sb.Append(Id);
            foreach (var color in Colors)
            {
                sb.Append("-");
                sb.Append(color);
            }
            return sb.ToString();
        }

        public static FigurePart Parse(string figurePartString)
        {
            string[] split = figurePartString.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 1) throw new FormatException("Empty figure part");
            if (split.Length < 2) throw new FormatException($"Figure part '{split[0]}' has no ID");

            var partType = H.GetFigurePartType(split[0]);

            if (!int.TryParse(split[1], out int partId))
                throw new FormatException($"Couldn't parse ID '{split[1]}' for figure part '{split[0]}'");

            var figurePart = new FigurePart(partType, partId);

            for (int i = 2; i < split.Length; i++)
            {
                if (!int.TryParse(split[i], out int color))
                    throw new FormatException($"Couldn't parse color '{split[i]}' for figure part '{split[0]}'");
                figurePart.Colors.Add(color);
            }

            return figurePart;
        }

        public static bool TryParse(string figurePartString, [NotNullWhen(true)] out FigurePart? figurePart)
        {
            figurePart = null;

            string[] split = figurePartString.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length < 2)
                return false;

            if (!H.TryGetFigurePartType(split[0], out FigurePartType partType))
                return false;
            if (!int.TryParse(split[1], out int partId))
                return false;

            var figurePartTemp = new FigurePart(partType, partId);

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
            unchecked
            {
                int hash = 647;

                hash = (hash * 47) + Type.GetHashCode();
                hash = (hash * 47) + Id;
                foreach (var color in Colors)
                {
                    hash = (hash * 47) + color;
                }

                return hash;
            }
        }

        public override bool Equals(object? obj)
        {
            return (
                obj is FigurePart other &&
                other.Type == Type &&
                other.Id == Id &&
                other.Colors.SequenceEqual(Colors)
            );
        }
    }
}
