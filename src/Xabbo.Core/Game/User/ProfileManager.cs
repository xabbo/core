using System;
using System.Threading.Tasks;

using Xabbo.Messages;

using Xabbo.Core.Events;
using Xabbo.Extension;
using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

namespace Xabbo.Core.Game;

public sealed class ProfileManager : GameStateManager
{
    private Task<IUserData> _taskUserData;
    private TaskCompletionSource<IUserData>? _tcsUserData;

    private bool _isLoadingProfile, _isLoadingCredits;

    public UserData? UserData { get; private set; }
    public int? HomeRoom { get; private set; }
    public Achievements? Achievements { get; private set; }

    public int? AchievementScore { get; private set; }
    public int? Credits { get; private set; } public ActivityPoints Points { get; private set; }
    public int? Diamonds => Points.TryGetValue(ActivityPointType.Diamond, out int value) ? (int?)value : null;
    public int? Duckets => Points.TryGetValue(ActivityPointType.Ducket, out int value) ? (int?)value : null;

    #region - Events -
    public event EventHandler? LoadedUserData;
    private void OnLoadedUserData() => LoadedUserData?.Invoke(this, EventArgs.Empty);

    public event EventHandler? UserDataUpdated;
    private void OnUserDataUpdated() => UserDataUpdated?.Invoke(this, EventArgs.Empty);

    public event EventHandler? HomeRoomUpdated;
    private void OnHomeRoomUpdated() => HomeRoomUpdated?.Invoke(this, EventArgs.Empty);

    public event EventHandler? LoadedAchievements;
    private void OnLoadedAchievements() => LoadedAchievements?.Invoke(this, EventArgs.Empty);

    public event EventHandler? AchievementUpdated;
    private void OnAchievementUpdated(IAchievement achievement)
        => AchievementUpdated?.Invoke(this, EventArgs.Empty); // TODO AchievementEventArgs

    public event EventHandler? CreditsUpdated;
    private void OnCreditsUpdated() => CreditsUpdated?.Invoke(this, EventArgs.Empty);

    public event EventHandler? LoadedPoints;
    private void OnLoadedPoints() => LoadedPoints?.Invoke(this, EventArgs.Empty);

    public event EventHandler<PointsUpdatedEventArgs>? PointsUpdated;
    private void OnPointsUpdated(ActivityPointType type, int amount, int change)
        => PointsUpdated?.Invoke(this, new PointsUpdatedEventArgs(type, amount, change));
    #endregion

#pragma warning disable CS8618
    public ProfileManager(IExtension extension)
        : base(extension)
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

    protected override void OnDisconnected() => Reset();

    /// <summary>
    /// Waits for the user data to load, or returns the user's data immediately if it has already loaded.
    /// </summary>
    public Task<IUserData> GetUserDataAsync() => _taskUserData;

    // [InterceptIn(nameof(In.ClientLatencyPingResponse))]
    private void HandleLatencyResponse(Intercept e)
    {
        if (e.Packet.Read<int>() == 0) return;

        if (UserData is null && !_isLoadingProfile)
        {
            _isLoadingProfile = true;
            Ext.Send(Out.InfoRetrieve);
        }

        if (Achievements is null) Ext.Send(Out.GetAchievements);

        if (Credits is null && !_isLoadingCredits)
        {
            _isLoadingCredits = true;
            Ext.Send(Out.GetCreditsInfo);
        }
    }

    // [InterceptIn(nameof(In.UserObject))]
    private void HandleUserObject(Intercept e)
    {
        if (_isLoadingProfile)
        {
            e.Block();
            _isLoadingProfile = false;
        }

        UserData = e.Packet.Parse<UserData>();

        _taskUserData = Task.FromResult<IUserData>(UserData);

        _tcsUserData?.TrySetResult(UserData);
        _tcsUserData = null;

        OnLoadedUserData();
    }

    // [InterceptIn(nameof(In.UpdateFigure))]
    private void HandleUpdateFigure(Intercept e)
    {
        if (UserData is null) return;

        UserData.Figure = e.Packet.Read<string>();
        UserData.Gender = H.ToGender(e.Packet.Read<string>());

        OnUserDataUpdated();
    }

    // [InterceptIn(nameof(In.UpdateAvatar))]
    private void HandleUpdateAvatar(Intercept e)
    {
        if (UserData is null) return;

        int index = e.Packet.Read<int>();
        if (index == -1)
        {
            UserData.Figure = e.Packet.Read<string>();
            UserData.Gender = H.ToGender(e.Packet.Read<string>());
            UserData.Motto = e.Packet.Read<string>();
            AchievementScore = e.Packet.Read<int>();

            OnUserDataUpdated();
        }
    }

    // @Update [InterceptIn(nameof(In.?)]
    private void HandleUserHomeRoom(Intercept e)
    {
        HomeRoom = e.Packet.Read<int>();
    }

    // [InterceptIn(nameof(In.WalletBalance))]
    private void HandleWalletBalance(Intercept e)
    {
        if (_isLoadingCredits)
        {
            _isLoadingCredits = false;
            e.Block();
        }

        Credits = (int)e.Packet.Read<float>();

        OnCreditsUpdated();
    }

    // [InterceptIn(nameof(In.ActivityPoints))]
    private void HandleActivityPoints(Intercept e)
    {
        Points = e.Packet.Parse<ActivityPoints>();

        OnLoadedPoints();
    }

    // [InterceptIn(nameof(In.ActivityPointNotification))]
    private void HandleActivityPointNotification(Intercept e)
    {
        int amount = e.Packet.Read<int>();
        int change = e.Packet.Read<int>();
        ActivityPointType type = (ActivityPointType)e.Packet.Read<int>();

        Points[type] = amount;

        OnPointsUpdated(type, amount, change);
    }

    // [InterceptIn(nameof(In.PossibleUserAchievements))]
    private void HandlePossibleUserAchievements(Intercept e)
    {
        Achievements = e.Packet.Parse<Achievements>();

        OnLoadedAchievements();
    }

    // [InterceptIn(nameof(In.PossibleAchievement))]
    private void HandlePossibleAchievement(Intercept e)
    {
        Achievement achievement = e.Packet.Parse<Achievement>();
        Achievements?.Update(achievement);

        OnAchievementUpdated(achievement);
    }
}
