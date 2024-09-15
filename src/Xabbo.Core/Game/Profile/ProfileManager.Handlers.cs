using System.Threading.Tasks;

using Xabbo.Messages.Flash;

namespace Xabbo.Core.Game;

partial class ProfileManager
{
    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.LatencyPingResponse))]
    private void HandleLatencyPingResponse(Intercept e)
    {
        if (e.Packet.Read<int>() == 0) return;

        if (UserData is null && !_isLoadingProfile)
        {
            _isLoadingProfile = true;
            Interceptor.Send(Out.InfoRetrieve);
        }

        if (Achievements is null) Interceptor.Send(Out.GetAchievements);

        if (Credits is null && !_isLoadingCredits)
        {
            _isLoadingCredits = true;
            Interceptor.Send(Out.GetCreditsInfo);
        }
    }

    [InterceptIn(nameof(In.UserObject))]
    private void HandleUserObject(Intercept e)
    {
        if (_isLoadingProfile)
        {
            e.Block();
            _isLoadingProfile = false;
        }

        UserData = e.Packet.Read<UserData>();

        _taskUserData = Task.FromResult<IUserData>(UserData);

        _tcsUserData?.TrySetResult(UserData);
        _tcsUserData = null;

        OnLoadedUserData();
    }

    [Intercept(~ClientType.Shockwave)]
    [InterceptIn(nameof(In.FigureUpdate))]
    private void HandleFigureUpdate(Intercept e)
    {
        if (UserData is null) return;

        UserData.Figure = e.Packet.Read<string>();
        UserData.Gender = H.ToGender(e.Packet.Read<string>());

        OnUserDataUpdated();
    }

    [InterceptIn(nameof(In.UserUpdate))]
    private void HandleUserUpdate(Intercept e)
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

    // TODO: [InterceptIn(nameof(In.?)]
    private void HandleUserHomeRoom(Intercept e)
    {
        HomeRoom = e.Packet.Read<int>();
    }

    [InterceptIn(nameof(In.CreditBalance))]
    private void HandleCreditBalance(Intercept e)
    {
        if (_isLoadingCredits)
        {
            _isLoadingCredits = false;
            e.Block();
        }

        Credits = (int)e.Packet.Read<float>();

        OnCreditsUpdated();
    }

    [InterceptIn(nameof(In.ActivityPoints))]
    private void HandleActivityPoints(Intercept e)
    {
        Points = e.Packet.Read<ActivityPoints>();

        OnLoadedPoints();
    }

    [InterceptIn(nameof(In.HabboActivityPointNotification))]
    private void HandleHabboActivityPointNotification(Intercept e)
    {
        int amount = e.Packet.Read<int>();
        int change = e.Packet.Read<int>();
        var type = (ActivityPointType)e.Packet.Read<int>();

        Points[type] = amount;

        OnPointsUpdated(type, amount, change);
    }

    [InterceptIn(nameof(In.Achievements))]
    private void HandleAchievements(Intercept e)
    {
        Achievements = e.Packet.Read<Achievements>();

        OnLoadedAchievements();
    }

    [InterceptIn(nameof(In.Achievement))]
    private void HandleAchievement(Intercept e)
    {
        var achievement = e.Packet.Read<Achievement>();
        Achievements?.Add(achievement);

        OnAchievementUpdated(achievement);
    }
}