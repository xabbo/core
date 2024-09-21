using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Outgoing;

namespace Xabbo.Core.Game;

public sealed partial class ProfileManager : GameStateManager
{
    private readonly ILogger Log;

    private TaskCompletionSource<IUserData> _tcsUserData = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private bool _isLoadingProfile, _isLoadingCredits;

    public UserData? UserData { get; private set; }
    public Achievements? Achievements { get; private set; }

    public int? AchievementScore { get; private set; }
    public int? Credits { get; private set; }
    public ActivityPoints Points { get; private set; }
    public int? Diamonds => Points.TryGetValue(ActivityPointType.Diamond, out int value) ? (int?)value : null;
    public int? Duckets => Points.TryGetValue(ActivityPointType.Ducket, out int value) ? (int?)value : null;

    #region - Events -
    public event Action? UserDataLoaded;
    public event Action? UserDataUpdated;
    public event Action? AchievementsLoaded;
    public event Action<AchievementUpdatedEventArgs>? AchievementUpdated; // TODO AchievementEventArgs
    public event Action<CreditsUpdatedEventArgs>? CreditsUpdated;
    public event Action? PointsLoaded;
    public event Action<ActivityPointUpdatedEventArgs>? ActivityPointUpdated;
    #endregion

#pragma warning disable CS8618

    public ProfileManager(IInterceptor interceptor, ILoggerFactory? loggerFactory = null)
        : base(interceptor)
    {
        Log = (ILogger?)loggerFactory?.CreateLogger<ProfileManager>() ?? NullLogger.Instance;
        Reset();
    }

#pragma warning restore CS8618

    protected override void OnInitialize(bool initializingAfterConnect)
    {
        base.OnInitialize(initializingAfterConnect);

        if (UserData is null && !_isLoadingProfile)
        {
            Log.LogInformation("Requesting user data.");

            _isLoadingProfile = true;
            Interceptor.Send(new GetUserDataMsg());
        }

        if (!Session.IsOrigins && Achievements is null)
        {
            Log.LogInformation("Requesting achievements.");
            Interceptor.Send(Out.GetAchievements);
        }

        if (Credits is null && !_isLoadingCredits)
        {
            Log.LogInformation("Requesting credits.");

            _isLoadingCredits = true;
            Interceptor.Send(Out.GetCreditsInfo);
        }
    }

    protected override void OnDisconnected() => Reset();

    private void Reset()
    {
        _tcsUserData = new TaskCompletionSource<IUserData>();

        UserData = null;
        Points = new ActivityPoints();
        Achievements = null;

        _isLoadingProfile = false;
        _isLoadingCredits = false;
    }

    /// <summary>
    /// Waits for the user data to load, or returns the user's data immediately if it has already loaded.
    /// </summary>
    public Task<IUserData> GetUserDataAsync() => _tcsUserData.Task;
}
