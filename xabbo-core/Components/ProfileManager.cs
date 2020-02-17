using System;
using System.Threading.Tasks;

using Xabbo.Core.Messages;
using Xabbo.Core.Protocol;

namespace Xabbo.Core.Components
{
    public class ProfileManager : XabboComponent
    {
        public enum Features { UserData, Friends, Achievements }

        private bool loadedUserData;

        public UserData UserData { get; private set; }

        protected override void OnInitialize()
        {
            if (In.LatencyResponse >= 0)
            {
                Dispatcher.AddHandler(In.LatencyResponse, HandleLatencyResponse);
            }
        }

        private async void HandleLatencyResponse(object sender, Packet packet)
        {
            await Task.Yield();

            Dispatcher.RemoveHandler(In.LatencyResponse, HandleLatencyResponse);

            if (!loadedUserData) await Interceptor.SendToServerAsync(Out.RequestUserData);
        }

        [Group(Features.UserData), Receive("UserData")]
        private void HandleUserData(Packet packet)
        {
            UserData = UserData.Parse(packet);
            loadedUserData = true;
        }


    }
}
