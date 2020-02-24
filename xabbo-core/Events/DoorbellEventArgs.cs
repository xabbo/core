using System;

namespace Xabbo.Core.Events
{
    public class DoorbellEventArgs : EventArgs
    {
        public string Name { get; }
        public bool Accept { get; set; }
        public bool Reject { get; set; }

        public DoorbellEventArgs(string name)
        {
            Name = name;
        }
    }
}
