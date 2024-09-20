using Microsoft.Extensions.Logging;

using Xabbo.Core.Events;
using Xabbo.Core.Messages.Incoming;
using Xabbo.Core.Messages.Incoming.Modern;

using Modern = Xabbo.Core.Messages.Incoming.Modern;

namespace Xabbo.Core.Game;

[Intercept]
partial class ProfileManager
{
    [Intercept]
    private void HandleUserData(Intercept<UserDataMsg> e)
    {
        using var scope = Log.MethodScope();

        if (_isLoadingProfile)
        {
            e.Block();
            _isLoadingProfile = false;
        }

        UserData = e.Msg.UserData;

        if (_tcsUserData.TrySetResult(UserData))
        {
            Log.LogInformation("Loaded user data.");
            UserDataLoaded?.Invoke();
        }
        else
        {
            Log.LogTrace("User data updated.");
            UserDataUpdated?.Invoke();
        }
    }

    [Intercept]
    private void HandleFigureUpdate(FigureUpdateMsg update)
    {
        if (UserData is null) return;

        UserData.Figure = update.Figure;
        UserData.Gender = update.Gender;

        using (Log.MethodScope())
            Log.LogTrace("User data updated.");
        UserDataUpdated?.Invoke();
    }

    [Intercept]
    private void HandleUserUpdate(AvatarUpdatedMsg update)
    {
        if (UserData is null) return;

        if (update.Index == -1)
        {
            UserData.Figure = update.Figure;
            UserData.Gender = update.Gender;
            UserData.Motto = update.Motto;
            AchievementScore = update.AchievementScore;

            using (Log.MethodScope())
                Log.LogTrace("User data updated.");
            UserDataUpdated?.Invoke();
        }
    }

    [Intercept]
    private void HandleCreditBalance(Intercept<CreditBalanceMsg> e)
    {
        if (_isLoadingCredits)
        {
            _isLoadingCredits = false;
            e.Block();
        }

        int? previousCredits = Credits;
        Credits = e.Msg.Credits;

        if (previousCredits != Credits)
        {
            using (Log.MethodScope())
            {
                if (!previousCredits.HasValue)
                    Log.LogInformation("Loaded credits: {Credits}.", Credits);
                else
                    Log.LogInformation("Credits updated: {Credits}.", Credits);
            }
            CreditsUpdated?.Invoke(new CreditsUpdatedEventArgs(e.Msg.Credits, previousCredits));
        }
    }

    [Intercept]
    private void HandleActivityPoints(Modern.ActivityPointsMsg msg)
    {
        Points = msg.ActivityPoints;

        using (Log.MethodScope())
            Log.LogTrace("Activity points loaded.");

        PointsLoaded?.Invoke();
    }

    [Intercept]
    private void HandleActivityPointUpdated(Modern.ActivityPointUpdatedMsg msg)
    {
        Points[msg.Type] = msg.Amount;

        using (Log.MethodScope())
            Log.LogTrace("Activity points updated: {ActivityPointType} {Change:+0;-#} = {Amount}.",
                msg.Type, msg.Change, msg.Amount);

        ActivityPointUpdated?.Invoke(new ActivityPointUpdatedEventArgs(msg.Type, msg.Amount, msg.Change));
    }

    [Intercept]
    private void HandleAchievements(Modern.AchievementsMsg msg)
    {
        Achievements = msg.Achievements;

        using (Log.MethodScope())
            Log.LogInformation("Loaded {Count} achievements.", Achievements.Count);

        AchievementsLoaded?.Invoke();
    }

    [Intercept]
    private void HandleAchievement(Modern.AchievementMsg msg)
    {
        Achievements?.Add(msg.Achievement);

        using (Log.MethodScope())
            Log.LogTrace("Achievement #{Id} ({BadgeId}) updated.", msg.Achievement.Id, msg.Achievement.BadgeId);

        AchievementUpdated?.Invoke(new AchievementUpdatedEventArgs(msg.Achievement));
    }
}