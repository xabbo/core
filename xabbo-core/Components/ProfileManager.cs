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

        private bool loadedUserData, isLoadingUserData;
        private bool hasLoadedFriends, isLoadingFriends;

        public UserData UserData { get; private set; }

        protected override void OnInitialize()
        {

        }

        [Group(Features.Autoload), Receive("LatencyResponse")]
        private async void HandleLatencyResponse(object sender, Packet packet)
        {
            Dispatcher.RemoveHandler(In.LatencyResponse, HandleLatencyResponse);

            await Task.Yield();

            try
            {

            }
            catch
            {
                Dispatcher.AddHandler(In.LatencyResponse, HandleLatencyResponse);
            }
        }

        [Group(Features.UserData), Receive("UserData")]
        private void HandleUserData(Packet packet)
        {
            UserData = UserData.Parse(packet);
            loadedUserData = true;
        }

        [Group(Features.Friends), Receive("Friends")]
        private void HandleFriends(object sender, Packet packet)
        {
            Dispatcher.RemoveHandler(packet.Header, HandleFriends);
        }
    }
}
