using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class Achievements : List<Achievement>
    {
        public static Achievements Parse(Packet packet) => new Achievements(packet);

        public string StringA { get; set; }

        public Achievements() { }

        internal Achievements(Packet packet)
            : this()
        {
            int n = packet.ReadInteger();
            for (int i = 0; i < n; i++)
                Add(Achievement.Parse(packet));
            StringA = packet.ReadString();
        }

        public Achievement GetAchievement(int id) => this.FirstOrDefault(x => x.Id.Equals(id));
    }
}
