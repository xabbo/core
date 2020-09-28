using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Components
{
    internal class InventoryManager : XabboComponent
    {
        public bool IsInitialized { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool NeedsRefresh { get; private set; }

        protected override void OnInitialize()
        {
            throw new NotImplementedException();
        }
    }
}
