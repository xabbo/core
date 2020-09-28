using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    internal class AchievementManager : XabboComponent
    {
        private bool isLoadingAchievements = true;

        private Achievements achievements;

        protected override void OnInitialize()
        {
            achievements = new Achievements();
        }

        [InterceptIn("AchievementList")]
        private void HandleAchievementList(InterceptEventArgs e)
        {

        }
    }
}
