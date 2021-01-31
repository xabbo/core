using System;

namespace Xabbo.Core.GameData
{
    public class GameDataHash
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Hash { get; set; }

        public GameDataHash()
        {
            Name = string.Empty;
            Url = string.Empty;
            Hash = string.Empty;
        }
    }
}
