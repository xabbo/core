using System;
using System.Globalization;
using Xabbo.Core.Protocol;

namespace Xabbo.Core
{
    public class EntityUpdate
    {
        public int Index { get; set; }
        public Tile Tile { get; set; }
        public Direction HeadDirection { get; set; }
        public Direction Direction { get; set; }
        public string Status { get; set; }

        // sit, lay
        public Stance Stance { get; set; } = Stance.Stand;

        // flatctrl
        public bool IsController { get; set; }
        public int ControlLevel { get; set; }

        // mv
        public Tile MovingTo { get; set; }

        // sit
        public bool SittingOnFloor { get; set; }

        // sit, lay
        public double ActionHeight { get; set; }

        // sign
        public Sign Sign { get; set; } = Sign.None;

        public static EntityUpdate Parse(Packet packet) => new EntityUpdate(packet);

        private EntityUpdate(Packet packet)
        {
            Index = packet.ReadInteger();
            Tile = Tile.Parse(packet);
            HeadDirection = (Direction)packet.ReadInteger();
            Direction = (Direction)packet.ReadInteger();
            Status = packet.ReadString();

            Stance = Stance.Stand;

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
                            Stance = Stance.Sit;
                            if (actionValues.Length > 1
                                && double.TryParse(actionValues[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double actionHeight))
                                ActionHeight = actionHeight;

                            if (actionValues.Length > 2)
                                SittingOnFloor = actionValues[2] == "1";
                        }
                        break;
                    case "lay":
                        {
                            Stance = Stance.Lay;
                            if (actionValues.Length > 1
                                && double.TryParse(actionValues[1], NumberStyles.Number, CultureInfo.InvariantCulture, out double actionHeight))
                                ActionHeight = actionHeight;
                        }
                        break;
                    case "sign":
                        {
                            if (actionValues.Length > 1 &&
                                int.TryParse(actionValues[1], out int sign))
                                Sign = (Sign)sign;
                        }
                        break;
                }
            }
        }

        public void Write(Packet packet)
        {
            packet.WriteInteger(Index);
            Tile.Write(packet);
            packet.WriteInteger((int)HeadDirection);
            packet.WriteInteger((int)Direction);
            packet.WriteString(Status);
        }
    }
}
