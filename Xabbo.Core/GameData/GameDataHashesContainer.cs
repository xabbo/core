using System;
using System.Collections.Generic;

namespace Xabbo.Core.GameData
{
    public class GameDataHashesContainer
    {
        public List<GameDataHash> Hashes { get; set; }

        public GameDataHashesContainer()
        {
            Hashes = new List<GameDataHash>();
        }
    }
}
