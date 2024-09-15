using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;

namespace Xabbo.Core.Game;

[Intercept(~ClientType.Shockwave)]
public sealed partial class ProfileManager : GameStateManager
{
    private readonly ILogger Log;

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
    public ProfileManager(IInterceptor interceptor, ILoggerFactory? loggerFactory = null)
        : base(interceptor)
    {
        Log = (ILogger?)loggerFactory?.CreateLogger<ProfileManager>() ?? NullLogger.Instance;
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
}
