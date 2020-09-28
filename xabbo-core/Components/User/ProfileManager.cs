using System;
using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Components
{
    public class ProfileManager : XabboComponent
    {
        public enum Features { Autoload, UserData, HomeRoom, Credits, Points, Achievements, AchievementScore }

        public UserData UserData { get; private set; }
        public int? HomeRoom { get; private set; }
        public Achievements Achievements { get; private set; }

        public int? AchievementScore { get; private set; }
        public int? Credits { get; private set; }
        public ActivityPoints Points { get; private set; }
        public int? Diamonds => Points.TryGetValue(ActivityPointType.Diamond, out int value) ? (int?)value : null;
        public int? Duckets => Points.TryGetValue(ActivityPointType.Ducket, out int value) ? (int?)value : null;

        #region - Events -
        public event EventHandler LoadedUserData;
        protected virtual void OnLoadedUserData() => LoadedUserData?.Invoke(this, EventArgs.Empty);

        public event EventHandler HomeRoomUpdated;
        protected virtual void OnHomeRoomUpdated() => HomeRoomUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler LoadedAchievements;
        protected virtual void OnLoadedAchievements() => LoadedAchievements?.Invoke(this, EventArgs.Empty);

        public event EventHandler AchievementUpdated;
        protected virtual void OnAchievementUpdated(Achievement previousAchievement)
            => AchievementUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler CreditsUpdated;
        protected virtual void OnCreditsUpdated() => CreditsUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler LoadedPoints;
        protected virtual void OnLoadedPoints() => LoadedPoints?.Invoke(this, EventArgs.Empty);

        public event EventHandler<PointsUpdatedEventArgs> PointsUpdated;
        protected virtual void OnPointsUpdated(ActivityPointType type, int amount, int change)
            => PointsUpdated?.Invoke(this, new PointsUpdatedEventArgs(type, amount, change));

        // LoadedAchievements, AchievementUpdated
        // LoadedFriends, FriendAdded, FriendRemoved, FriendUpdated
        #endregion

        public ProfileManager()
        {
            Points = new ActivityPoints();
        }

        protected override void OnInitialize()
        {

        }

        [Group(Features.Autoload), InterceptIn("LatencyResponse")]
        private async void HandleLatencyResponse(InterceptEventArgs e)
        {
            if (UserData == null && Dispatcher.IsAttached(this, Features.UserData))
            {
                await SendAsync(Out.RequestUserData);
            }

            if (!Credits.HasValue && Dispatcher.IsAttached(this, Features.Credits))
            {
                await SendAsync(Out.RequestUserCredits);
            }

            if (Achievements == null && Dispatcher.IsAttached(this, Features.Achievements))
            {
                await SendAsync(Out.RequestAchievements);
            }
        }

        [Group(Features.UserData), InterceptIn("UserData"), RequiredOut("RequestUserData")]
        private void HandleUserData(InterceptEventArgs e)
        {
            UserData = UserData.Parse(e.Packet);
            OnLoadedUserData();
        }

        [Group(Features.HomeRoom), InterceptIn("UserHomeRoom")]
        private void HandleUserHomeRoom(InterceptEventArgs e)
        {
            HomeRoom = e.Packet.ReadInt();


        }

        [Group(Features.Credits), InterceptIn("UserCredits"), RequiredOut("RequestUserCredits")]
        private void HandleUserCredits(InterceptEventArgs e)
        {
            Credits = (int)e.Packet.ReadDouble();

            OnCreditsUpdated();
        }

        [Group(Features.Points), InterceptIn("UserCurrency")]
        private void HandleUserPoints(InterceptEventArgs e)
        {
            Points = ActivityPoints.Parse(e.Packet);

            OnLoadedPoints();
        }

        [Group(Features.Points), InterceptIn("UserPoints")]
        private void HandleUpdateUserPoints(InterceptEventArgs e)
        {
            int amount = e.Packet.ReadInt();
            int change = e.Packet.ReadInt();
            var type = (ActivityPointType)e.Packet.ReadInt();

            Points[type] = amount;

            OnPointsUpdated(type, amount, change);
        }

        [Group(Features.Achievements), InterceptIn("AchievementList"), RequiredOut("RequestAchievements")]
        private void HandleAchievementList(InterceptEventArgs e)
        {
            Achievements = Achievements.Parse(e.Packet);

            OnLoadedAchievements();
        }

        [Group(Features.Achievements), InterceptIn("AchievementProgress")]
        private void HandleAchievementProgress(InterceptEventArgs e)
        {
            var achievement = Achievement.Parse(e.Packet);
            Achievements?.Update(achievement);
        }
    }
}
