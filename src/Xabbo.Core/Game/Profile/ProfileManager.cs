﻿using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Xabbo.Interceptor;
using Xabbo.Messages.Flash;
using Xabbo.Core.Events;
using Xabbo.Core.Messages.Outgoing;

namespace Xabbo.Core.Game;

/// <summary>
/// Manages the current user's information.
/// </summary>
public sealed partial class ProfileManager : GameStateManager
{
    private readonly ILogger Log;

    private TaskCompletionSource<IUserData> _tcsUserData = new(TaskCreationOptions.RunContinuationsAsynchronously);

    private bool _isLoadingProfile, _isLoadingCredits;

    /// <summary>
    /// Gets the user's current information.
    /// </summary>
    public UserData? UserData { get; private set; }

    /// <summary>
    /// Gets the user's current achievements.
    /// </summary>
    public Achievements? Achievements { get; private set; }

    /// <summary>
    /// Gets the user's current achievement score.
    /// </summary>
    public int? AchievementScore { get; private set; }

    /// <summary>
    /// Gets the user's current credits.
    /// </summary>
    public int? Credits { get; private set; }

    /// <summary>
    /// Gets the user's current activity points.
    /// </summary>
    public ActivityPoints Points { get; private set; }

    /// <summary>
    /// Gets the user's current amount of diamonds.
    /// </summary>
    public int? Diamonds => Points.TryGetValue(ActivityPointType.Diamond, out int value) ? (int?)value : null;

    /// <summary>
    /// Gets the user's current amount of duckets.
    /// </summary>
    public int? Duckets => Points.TryGetValue(ActivityPointType.Ducket, out int value) ? (int?)value : null;

    #region - Events -
    /// <summary>
    /// Invoked when the user's data is first loaded.
    /// </summary>
    public event Action? UserDataLoaded;

    /// <summary>
    /// Invoked when the user's data is updated.
    /// </summary>
    public event Action? UserDataUpdated;

    /// <summary>
    /// Invoked when the user's achievements are loaded.
    /// </summary>
    public event Action? AchievementsLoaded;

    /// <summary>
    /// Invoked when the an achievement of the user is updated.
    /// </summary>
    public event Action<AchievementUpdatedEventArgs>? AchievementUpdated;

    /// <summary>
    /// Invoked when the user's credits are updated.
    /// </summary>
    public event Action<CreditsUpdatedEventArgs>? CreditsUpdated;

    /// <summary>
    /// Invoked when the user's activity points are loaded.
    /// </summary>
    public event Action? PointsLoaded;

    /// <summary>
    /// Invoked when the user's activity points are updated.
    /// </summary>
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
    /// Waits for the user data to load, or returns the user's data immediately if it has already been loaded.
    /// </summary>
    public Task<IUserData> GetUserDataAsync() => _tcsUserData.Task;
}
