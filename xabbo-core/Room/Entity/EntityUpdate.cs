using System;
using System.Globalization;

using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class EntityUpdate : IEntityUpdate
    {
        public int Index { get; set; }
        public Tile Location { get; set; }
        ITile IEntityUpdate.Location => Location;
        public int HeadDirection { get; set; }
        public int Direction { get; set; }
        public string Status { get; set; }

        // sit, lay
        public Stances Stance { get; set; } = Stances.Stand;

        // flatctrl
        public bool IsController { get; set; }
        public int ControlLevel { get; set; }

        // mv
        public Tile MovingTo { get; set; }
        ITile IEntityUpdate.MovingTo => MovingTo;

        // sit
        public bool SittingOnFloor { get; set; }

        // sit, lay
        public double ActionHeight { get; set; }

        // sign
        public Signs Sign { get; set; } = Signs.None;

        public static EntityUpdate Parse(Packet packet) => new EntityUpdate(packet);

        public EntityUpdate()
        {
            Location = new Tile();
            HeadDirection = 0;
            Direction = 0;
            Status = "//";
        }

        private EntityUpdate(Packet packet)
        {
            Index = packet.ReadInt();
            Location = Tile.Parse(packet);
            HeadDirection = packet.ReadInt();
            Direction = packet.ReadInt();
            Status = packet.ReadString();

            Stance = Stances.Stand;

            string[] actionData = Status.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string actionInfo in actionData)
            {
                string[] actionValues = actionInfo.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (actionValues.Length < 1)
                    continue;

                switch (actionValues[0])
                {
                    case "flatctrl":
                        {
                            IsController = true;
                            if (actionValues.Length > 1 &&
                                int.TryParse(actionValues[1], out int controlLevel))
                                ControlLevel = controlLevel;
                        }
                        break;
                    case "mv":
                        {
                            MovingTo = Tile.Parse(actionValues[1]);
                        }
                        break;
                    case "sit":
                        {
                            Stance = Stances.Sit;
                            if (actionValues.Length > 1
                                && double.TryParse(actionValues[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double actionHeight))
                                ActionHeight = actionHeight;

                            if (actionValues.Length > 2)
                                SittingOnFloor = actionValues[2] == "1";
                        }
                        break;
                    case "lay":
                        {
                            Stance = Stances.Lay;
                            if (actionValues.Length > 1
                                && double.TryParse(actionValues[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double actionHeight))
                                ActionHeight = actionHeight;
                        }
                        break;
                    case "sign":
                        {
                            if (actionValues.Length > 1 &&
                                int.TryParse(actionValues[1], out int sign))
                                Sign = (Signs)sign;
                        }
                        break;
                }
            }
        }

        public void Write(Packet packet)
        {
            packet.WriteInt(Index);
            Location.Write(packet);
            packet.WriteInt((int)HeadDirection);
            packet.WriteInt((int)Direction);
            packet.WriteString(Status);
        }
    }
}
