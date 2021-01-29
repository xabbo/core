using System;
using System.Threading.Tasks;

using Xabbo.Core.Events;
using Xabbo.Core.Messages;

namespace Xabbo.Core.Game
{
    public class ProfileManager : GameStateManager
    {
        private Task<IUserData> _taskUserData;
        private TaskCompletionSource<IUserData>? _tcsUserData;

        private bool _loadingCredits;

        public UserData? UserData { get; private set; }
        public int? HomeRoom { get; private set; }
        public Achievements? Achievements { get; private set; }

        public int? AchievementScore { get; private set; }
        public int? Credits { get; private set; }
        public ActivityPoints Points { get; private set; }
        public int? Diamonds => Points.TryGetValue(ActivityPointType.Diamond, out int value) ? (int?)value : null;
        public int? Duckets => Points.TryGetValue(ActivityPointType.Ducket, out int value) ? (int?)value : null;

        #region - Events -
        public event EventHandler? LoadedUserData;
        protected virtual void OnLoadedUserData() => LoadedUserData?.Invoke(this, EventArgs.Empty);

        public event EventHandler? UserDataUpdated;
        protected virtual void OnUserDataUpdated() => UserDataUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler? HomeRoomUpdated;
        protected virtual void OnHomeRoomUpdated() => HomeRoomUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler? LoadedAchievements;
        protected virtual void OnLoadedAchievements() => LoadedAchievements?.Invoke(this, EventArgs.Empty);

        public event EventHandler? AchievementUpdated;
        protected virtual void OnAchievementUpdated(IAchievement achievement)
            => AchievementUpdated?.Invoke(this, EventArgs.Empty); // TODO AchievementEventArgs

        public event EventHandler? CreditsUpdated;
        protected virtual void OnCreditsUpdated() => CreditsUpdated?.Invoke(this, EventArgs.Empty);

        public event EventHandler? LoadedPoints;
        protected virtual void OnLoadedPoints() => LoadedPoints?.Invoke(this, EventArgs.Empty);

        public event EventHandler<PointsUpdatedEventArgs>? PointsUpdated;
        protected virtual void OnPointsUpdated(ActivityPointType type, int amount, int change)
            => PointsUpdated?.Invoke(this, new PointsUpdatedEventArgs(type, amount, change));

        // LoadedFriends, FriendAdded, FriendRemoved, FriendUpdated
        #endregion

        public ProfileManager(IInterceptor interceptor)
            : base(interceptor)
        {
            _tcsUserData = new TaskCompletionSource<IUserData>();
            _taskUserData = _tcsUserData.Task;

            Points = new ActivityPoints();
        }

        /// <summary>
        /// Waits for the user data load, or returns the user's data instantly if it has already loaded.
        /// </summary>
        public Task<IUserData> GetUserDataAsync() => _taskUserData;

        [InterceptIn(nameof(Incoming.ClientLatencyPingResponse))]
        private async void HandleLatencyResponse(InterceptArgs e)
        {
            if (UserData is null) await SendAsync(Out.InfoRetrieve);
            if (Achievements is null) await SendAsync(Out.GetUserAchievements);

            if (Credits is null && !_loadingCredits)
            {
                _loadingCredits = true;
                await SendAsync(Out.GetCredits);
            }
        }

        [InterceptIn(nameof(Incoming.UserObject))]
        private void HandleUserData(InterceptArgs e)
        {
            UserData = UserData.Parse(e.Packet);

            _tcsUserData?.TrySetResult(UserData);
            _tcsUserData = null;

            _taskUserData = Task.FromResult<IUserData>(UserData);

            OnLoadedUserData();
        }

        [InterceptIn(nameof(Incoming.UpdateFigure))]
        private void HandleUpdateFigure(InterceptArgs e)
        {
            if (UserData is null) return;

            UserData.Figure = e.Packet.ReadString();
            UserData.Gender = H.ToGender(e.Packet.ReadString());

            OnUserDataUpdated();
        }

        [InterceptIn(nameof(Incoming.UpdateAvatar))]
        private void HandleUpdateAvatar(InterceptArgs e)
        {
            if (UserData is null) return;

            int index = e.Packet.ReadInt();
            if (index == -1)
            {
                UserData.Figure = e.Packet.ReadString();
                UserData.Gender = H.ToGender(e.Packet.ReadString());
                UserData.Motto = e.Packet.ReadString();
                AchievementScore = e.Packet.ReadInt();

                OnUserDataUpdated();
            }
        }

        // @Update [InterceptIn(nameof(Incoming.?)]
        private void HandleUserHomeRoom(InterceptArgs e)
        {
            HomeRoom = e.Packet.ReadInt();
        }

        // @Update [InterceptIn(nameof(Incoming.?)]
        private void HandleUserCredits(InterceptArgs e)
        {
            if (_loadingCredits)
            {
                _loadingCredits = false;
                e.Block();
            }

            Credits = (int)e.Packet.ReadFloat();

            OnCreditsUpdated();
        }

        // @Update [InterceptIn(nameof(Incoming.?))]
        private void HandleUserPoints(InterceptArgs e)
        {
            Points = ActivityPoints.Parse(e.Packet);

            OnLoadedPoints();
        }

        // @Update [Group(Features.Points), InterceptIn("UserPoints")]
        private void HandleUpdateUserPoints(InterceptArgs e)
        {
            int amount = e.Packet.ReadInt();
            int change = e.Packet.ReadInt();
            var type = (ActivityPointType)e.Packet.ReadInt();

            Points[type] = amount;

            OnPointsUpdated(type, amount, change);
        }

        // @Update [Group(Features.Achievements), InterceptIn("AchievementList"), RequiredOut("RequestAchievements")]
        private void HandleAchievementList(InterceptArgs e)
        {
            Achievements = Achievements.Parse(e.Packet);

            OnLoadedAchievements();
        }

        // @Update [Group(Features.Achievements), InterceptIn("AchievementProgress")]
        private void HandleAchievementProgress(InterceptArgs e)
        {
            var achievement = Achievement.Parse(e.Packet);
            Achievements?.Update(achievement);

            OnAchievementUpdated(achievement);
        }
    }
}
