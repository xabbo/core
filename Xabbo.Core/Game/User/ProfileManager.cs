using System;
using System.Threading.Tasks;

using Xabbo.Messages;
using Xabbo.Interceptor;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

public class ProfileManager : GameStateManager
{
    private Task<IUserData> _taskUserData;
    private TaskCompletionSource<IUserData>? _tcsUserData;

    private bool _isLoadingProfile, _isLoadingCredits;

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
    #endregion

#pragma warning disable CS8618
    public ProfileManager(IInterceptor interceptor)
        : base(interceptor)
    {
        Reset();
    }
#pragma warning restore CS8618

    private void Reset()
    {
        _tcsUserData = new TaskCompletionSource<IUserData>();
        _taskUserData = _tcsUserData.Task;

        UserData = null;
        Points = new ActivityPoints();
        Achievements = null;

        _isLoadingProfile = false;
        _isLoadingCredits = false;
    }

    protected override void OnDisconnected(object? sender, EventArgs e)
    {
        base.OnDisconnected(sender, e);

        Reset();
    }

    /// <summary>
    /// Waits for the user data to load, or returns the user's data immediately if it has already loaded.
    /// </summary>
    public Task<IUserData> GetUserDataAsync() => _taskUserData;

    [InterceptIn(nameof(Incoming.ClientLatencyPingResponse))]
    private async void HandleLatencyResponse(InterceptArgs e)
    {
        if (e.Packet.ReadInt() == 0) return;

        if (UserData is null && !_isLoadingProfile)
        {
            _isLoadingProfile = true;
            await Interceptor.SendAsync(Out.InfoRetrieve);
        }

        if (Achievements is null) await Interceptor.SendAsync(Out.GetUserAchievements);

        if (Credits is null && !_isLoadingCredits)
        {
            _isLoadingCredits = true;
            await Interceptor.SendAsync(Out.GetCredits);
        }
    }

    [InterceptIn(nameof(Incoming.UserObject))]
    private void HandleUserObject(InterceptArgs e)
    {
        if (_isLoadingProfile)
        {
            e.Block();
            _isLoadingProfile = false;
        }

        UserData = UserData.Parse(e.Packet);

        _taskUserData = Task.FromResult<IUserData>(UserData);

        _tcsUserData?.TrySetResult(UserData);
        _tcsUserData = null;

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

    [InterceptIn(nameof(Incoming.WalletBalance))]
    private void HandleWalletBalance(InterceptArgs e)
    {
        if (_isLoadingCredits)
        {
            _isLoadingCredits = false;
            e.Block();
        }

        Credits = (int)e.Packet.ReadFloatAsString();

        OnCreditsUpdated();
    }

    [InterceptIn(nameof(Incoming.ActivityPoints))]
    private void HandleActivityPoints(InterceptArgs e)
    {
        Points = ActivityPoints.Parse(e.Packet);

        OnLoadedPoints();
    }

    [InterceptIn(nameof(Incoming.ActivityPointNotification))]
    private void HandleActivityPointNotification(InterceptArgs e)
    {
        int amount = e.Packet.ReadInt();
        int change = e.Packet.ReadInt();
        ActivityPointType type = (ActivityPointType)e.Packet.ReadInt();

        Points[type] = amount;

        OnPointsUpdated(type, amount, change);
    }

    [InterceptIn(nameof(Incoming.PossibleUserAchievements))]
    private void HandlePossibleUserAchievements(InterceptArgs e)
    {
        Achievements = Achievements.Parse(e.Packet);

        OnLoadedAchievements();
    }

    [InterceptIn(nameof(Incoming.PossibleAchievement))]
    private void HandlePossibleAchievement(InterceptArgs e)
    {
        Achievement achievement = Achievement.Parse(e.Packet);
        Achievements?.Update(achievement);

        OnAchievementUpdated(achievement);
    }
}
