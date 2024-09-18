using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;

namespace Xabbo.Core;

internal static class FigurePartGenders
{
    private const string FigurePartGendersResourcePath = "Xabbo.Core.Resources.figure_part_genders";
    internal static readonly ImmutableDictionary<int, Gender> GenderMap;

    static FigurePartGenders()
    {
        var dictionary = new Dictionary<int, Gender>();

        Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(FigurePartGendersResourcePath);
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
                    if (line.Split('/') is not [string partIdStr, string genderStr])
                        continue;

                    if (!int.TryParse(partIdStr, out int partId)) continue;
                    switch (genderStr)
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

        GenderMap = dictionary.ToImmutableDictionary();
    }

}