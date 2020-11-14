using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Xabbo.Core.Metadata
{
    public class FigureData
    {
        public static FigureData Load(Stream stream) => new FigureData(FigureDataXml.Load(stream));
        public static FigureData Load(string path)
        {
            using (var stream = File.OpenRead(path))
                return Load(stream);
        }

        public IReadOnlyList<Palette> Palettes { get; }
        public IReadOnlyList<PartSetCollection> SetCollections { get; }

        internal FigureData(FigureDataXml proxy)
        {
            Palettes = proxy.Palettes
                .Select(palette => new Palette(palette))
                .ToList().AsReadOnly();

            SetCollections = proxy.SetCollections
                .Select(setCollection => new PartSetCollection(setCollection))
                .ToList().AsReadOnly();
        }

        public Palette GetPalette(int id) => Palettes.FirstOrDefault(x => x.Id == id);

        public Palette GetPalette(FigurePartType partType) => GetPalette(GetSetCollection(partType).PaletteId);

        public PartSetCollection GetSetCollection(FigurePartType figurePartType)
            => SetCollections.FirstOrDefault(x => x.Type == figurePartType);

        public class Palette
        {
            public int Id { get; }
            public IReadOnlyList<Color> Colors { get; }

            internal Palette(FigureDataXml.Palette proxy)
            {
                Id = proxy.Id;
                Colors = proxy.Colors
                    .Select(color => new Color(color))
                    .ToList().AsReadOnly();
            }

            public Color GetColor(int id) => Colors.FirstOrDefault(x => x.Id == id);
        }

        public class Color
        {
            public int Id { get; }
            public int Index { get; }
            public int RequiredClubLevel { get; }
            public bool IsSelectable { get; }
            public string Value { get; }

            public bool IsClubRequired => RequiredClubLevel > 0;

            internal Color(FigureDataXml.Color proxy)
            {
                Id = proxy.Id;
                Index = proxy.Index;
                RequiredClubLevel = proxy.RequiredClubLevel;
                IsSelectable = proxy.IsSelectable;
                Value = proxy.Value;
            }
        }

        public class PartSetCollection
        {
            /// <summary>
            /// The figure part type of this part set collection.
            /// </summary>
            public FigurePartType Type { get; }
            /// <summary>
            /// The color palette that this part set collection uses.
            /// </summary>
            public int PaletteId { get; }
            public int mand_m_0 { get; }
            public int mand_f_0 { get; }
            public int mand_m_1 { get; }
            public int mand_f_1 { get; }
            public IReadOnlyList<PartSet> Sets { get; }

            internal PartSetCollection(FigureDataXml.PartSetCollection proxy)
            {
                Type = H.GetFigurePartType(proxy.Type);
                PaletteId = proxy.PaletteId;
                mand_m_0 = proxy.mand_m_0;
                mand_f_0 = proxy.mand_f_0;
                mand_m_1 = proxy.mand_m_1;
                mand_f_1 = proxy.mand_f_1;
                Sets = proxy.Sets
                    .Select(partSet => new PartSet(partSet))
                    .ToList().AsReadOnly();
            }

            public PartSet GetSet(int id) => Sets.FirstOrDefault(x => x.Id == id);
        }

        public class PartSet 
        {
            public int Id { get; }
            public Gender Gender { get; }
            public int RequiredClubLevel { get; }
            public bool IsColorable { get; }
            public bool IsSelectable { get; }
            public bool IsPreSelectable { get; }
            public bool IsSellable { get; }
            public IReadOnlyList<Part> Parts { get; }

            public bool IsClubRequired => RequiredClubLevel > 0;

            internal PartSet(FigureDataXml.PartSet proxy)
            {
                Id = proxy.Id;
                Gender = H.ToGender(proxy.Gender);
                RequiredClubLevel = proxy.RequiredClubLevel;
                IsColorable = proxy.IsColorable;
                IsSelectable = proxy.IsSelectable;
                IsPreSelectable = proxy.IsPreSelectable;
                IsSellable = proxy.IsSellable;
                Parts = proxy.Parts
                    .Select(part => new Part(part))
                    .ToList().AsReadOnly();
            }

            public Part GetPart(int id) => Parts.FirstOrDefault(x => x.Id == id);
        }

        public class Part
        {
            public int Id { get; }
            public string Type { get; }
            public bool IsColorable { get; }
            public int Index { get; }
            public int ColorIndex { get; }

            internal Part(FigureDataXml.Part proxy)
            {
                Id = proxy.Id;
                Type = proxy.Type;
                IsColorable = proxy.IsColorable;
                Index = proxy.Index;
                ColorIndex = proxy.ColorIndex;
            }
        }

        public bool TryGetGender(Figure figure, out Gender gender)
        {
            gender = Gender.Unisex;

            foreach (var part in figure.Parts)
            {
                var partSetCollection = GetSetCollection(part.Type);
                if (partSetCollection == null) continue;
                var partSet = partSetCollection.GetSet(part.Id);
                if (partSet == null) continue;
                if (partSet.Gender != Gender.Unisex)
                    break;
            }

            return gender != Gender.Unisex;
        }

        public bool TryGetGender(string figureString, out Gender gender)
        {
            gender = Gender.Unisex;
            if (!Figure.TryParse(figureString, out Figure figure))
                return false;
            return TryGetGender(figure, out gender);
        }
    }
}
