using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class ProfileManager : XabboComponent
    {
        public enum Features { Autoload, UserData, Friends, Achievements }

        private Task<List<FriendInfo>> getFriendsTask;

        private bool isInitialized;

        private bool
            hasLoadedUserData, forceLoadingUserData,
            hasLoadedFriends, forceLoadingFriends,
            hasLoadedAchievements;

        private bool isForceLoading;

        private SemaphoreSlim workingSemaphore = new SemaphoreSlim(1, 1);

        private Task<UserData> userDataTask;
        private Task<List<FriendInfo>> friendsTask;
        private Task<Achievements> achievementsTask;

        public UserData UserData { get; private set; }
        public IReadOnlyList<FriendInfo> Friends { get; private set; }
        public IReadOnlyList<Achievement> Achievements { get; private set; }

        #region - Events -
        public event EventHandler LoadedUserData;
        protected virtual void OnLoadedUserData() => LoadedUserData?.Invoke(this, EventArgs.Empty);


        // LoadedFriends, FriendAdded, FriendRemoved, FriendUpdated
        // LoadedAchievements, AchievementUpdated
        #endregion

        protected override void OnInitialize()
        {

        }

        [Group(Features.Autoload), Receive("LatencyResponse")]
        private async void HandleLatencyResponse(object sender, Packet packet)
        {
            if (!await workingSemaphore.WaitAsync(0)) return;

            await Task.Yield();

            try
            {
                if (!hasLoadedUserData && Dispatcher.IsAttached(this, Features.UserData))
                {
                    await new CaptureMessageTask(Interceptor, Destination.Client, In.UserData).ExecuteAsync(10000);
                }

                /*if (!hasLoadedFriends && Dispatcher.IsAttached(this, Features.Friends))
                {
                    var friends = await new GetFriendsTask(Interceptor).ExecuteAsync(10000);
                    await Task.Delay(1000);
                }

                if (!hasLoadedAchievements && Dispatcher.IsAttached(this, Features.Achievements))
                {
                }*/

                isInitialized = true;
            }
            finally { workingSemaphore.Release(); }
        }

        [Group(Features.UserData), InterceptIn("UserData"), RequiredOut("RequestUserData")]
        private void HandleUserData(InterceptEventArgs e)
        {
            UserData = UserData.Parse(e.Packet);
            hasLoadedUserData = true;

            OnLoadedUserData();
        }
    }
}
