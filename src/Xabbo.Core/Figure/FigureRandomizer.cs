using System;
using System.Collections.Generic;
using System.Linq;

using Xabbo.Core.GameData;
using static Xabbo.Core.GameData.FigureData;

namespace Xabbo.Core;

public class FigureRandomizer
{
    private readonly Random random = new();

    public FigureData FigureData { get; }

    public Dictionary<FigurePartType, double> Probabilities { get; }

    public bool AllowHC { get; set; }

    public FigureRandomizer(FigureData figureData)
    {
        FigureData = figureData;
        Probabilities = new Dictionary<FigurePartType, double>() {
            { FigurePartType.Hair, 1 }, // hr
            { FigurePartType.Head, 1 }, // hd
            { FigurePartType.Chest, 1 }, // ch
            { FigurePartType.Legs, 1 }, // lg
            { FigurePartType.Shoes, 0.8 }, // sh
            { FigurePartType.Hat, 0.2 }, // ha
            { FigurePartType.HeadAccessory, 0.1 }, // he
            { FigurePartType.EyeAccessory, 0.1 }, // ea
            { FigurePartType.FaceAccessory, 0.1 }, // fa
            { FigurePartType.ChestAccessory, 0.1 }, // ca
            { FigurePartType.WaistAccessory, 0.1 }, // wa
            { FigurePartType.Coat, 0.1 }, // cc
            { FigurePartType.ChestPrint, 0.1 }  // cp
        };
    }

    private static int GetColorCount(FigurePartSet partSet)
        => partSet.Parts.Select(x => x.ColorIndex).Where(x => x > 0).Distinct().Count();

    private FigurePartColor[] GetColors(FigurePartSetCollection setCollection) =>
        FigureData.GetPalette(setCollection).Colors.Values
        .Where(x =>
            x.IsSelectable &&
            (AllowHC || !x.IsClubRequired)
        ).ToArray();

    private FigurePartSet[] GetPartSets(FigurePartType partType, Gender gender)
        => FigureData.SetCollections[partType].PartSets.Values
        .Where(set =>
            set.IsSelectable && !set.IsSellable &&
            (AllowHC || !set.IsClubRequired) &&
            ((set.Gender & gender) > 0)
        ).ToArray();

    private FigurePartSet? GetRandomPartSet(FigurePartType partType, Gender gender)
    {
        var selection = GetPartSets(partType, gender);
        if (selection.Length == 0) return null;
        return selection[random.Next(selection.Length)];
    }

    public void RandomizeColors(Figure.Part part)
    {
        var partSetCollection = FigureData.SetCollections[part.Type];
        if (partSetCollection == null) return;
        var partSet = partSetCollection.PartSets[part.Id];
        if (partSet == null) return;

        part.Colors.Clear();
        if (partSet.IsColorable)
        {
            int colorCount = GetColorCount(partSet);
            var colorSelection = FigureData.GetPalette(partSetCollection).Colors.Values.Where(x =>
                (AllowHC || !x.IsClubRequired) &&
                x.IsSelectable
            ).ToArray();

            if (colorSelection.Length > 0)
            {
                for (int i = 0; i < colorCount; i++)
                {
                    var color = colorSelection[random.Next(colorSelection.Length)];
                    part.Colors.Add(color.Id);
                }
            }
        }
    }

    public FigurePartColor GetRandomColor(FigurePartType figurePartType)
    {
        var set = FigureData.SetCollections[figurePartType];
        // if (set is null) throw new Exception($"Unknown part type: {figurePartType}.");
        var selection = FigureData.GetPalette(set).Colors.Values.Where(x => x.IsSelectable && (AllowHC || !x.IsClubRequired)).ToArray();
        return selection[random.Next(selection.Length)];
    }

    public Figure Generate() => Generate(random.NextDouble() < 0.5 ? Gender.Male : Gender.Female);

    public Figure Generate(Gender gender)
    {
        if (gender != Gender.Male && gender != Gender.Female)
            throw new ArgumentException($"Invalid gender for figure generator: {gender}");

        var figure = new Figure(gender);
        foreach (var setCollection in FigureData.SetCollections.Values)
        {
            if (!Probabilities.TryGetValue(setCollection.Type, out double probability))
                continue;

            var partType = setCollection.Type;

            if (probability >= 1.0 || random.NextDouble() < probability)
            {
                var partSet = GetRandomPartSet(partType, gender);
                if (partSet == null) continue;

                var figurePart = new Figure.Part(partType, partSet.Id);
                if (partSet.IsColorable)
                {
                    int colorCount = GetColorCount(partSet);
                    var colors = GetColors(setCollection);

                    for (int i = 0; i < colorCount; i++)
                    {
                        var color = colors[random.Next(colors.Length)];
                        figurePart.Colors.Add(color.Id);
                    }
                }

                figure.Add(figurePart);
            }
        }

        return figure;
    }
}
