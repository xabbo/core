using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    public class AchievementManager : XabboComponent
    {
        private bool isLoadingAchievements = true;

        protected override void OnInitialize()
        {
            
        }

        [InterceptIn("AchievementList")]
        private void HandleAchievementList(InterceptEventArgs e)
        {

        }
    }
}
