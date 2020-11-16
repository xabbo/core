﻿using System;
using System.Collections.Generic;

namespace Xabbo.Core.Messages
{
    public sealed class Incoming : HeaderDictionary
    {
        public Incoming()
            : base(Destination.Client)
        { }

        public Incoming(IReadOnlyDictionary<string, short> values)
            : base(Destination.Client, values)
        { }

        public Header AchievementList { get; private set; }
        public Header AchievementProgress { get; private set; }
        public Header AchievementUnlocked { get; private set; }
        public Header AchievementsConfiguration { get; private set; }
        public Header AddBot { get; private set; }
        public Header AddFloorItem { get; private set; }
        public Header AddHabboItem { get; private set; }
        public Header AddPet { get; private set; }
        public Header AddUserBadge { get; private set; }
        public Header AddWallItem { get; private set; }
        public Header AdventCalendarData { get; private set; }
        public Header AdventCalendarProduct { get; private set; }
        public Header AlertLimitedSoldOut { get; private set; }
        public Header AlertPurchaseFailed { get; private set; }
        public Header AlertPurchaseUnavailable { get; private set; }
        public Header BaseJumpJoinQueue { get; private set; }
        public Header BaseJumpLeaveQueue { get; private set; }
        public Header BaseJumpLoadGame { get; private set; }
        public Header BaseJumpLoadGameURL { get; private set; }
        public Header BaseJumpUnloadGame { get; private set; }
        public Header BonusRare { get; private set; }
        public Header BotError { get; private set; }
        public Header BotSettings { get; private set; }
        public Header BubbleAlert { get; private set; }
        public Header BuildersClubExpired { get; private set; }
        public Header BullyReportClosed { get; private set; }
        public Header BullyReportRequest { get; private set; }
        public Header BullyReportedMessage { get; private set; }
        public Header CameraCompetitionStatus { get; private set; }
        public Header CameraPrice { get; private set; }
        public Header CameraPublishWaitMessage { get; private set; }
        public Header CameraPurchaseSuccesfull { get; private set; }
        public Header CameraRoomThumbnailSaved { get; private set; }
        public Header CameraURL { get; private set; }
        public Header CanCreateEvent { get; private set; }
        public Header CanCreateRoom { get; private set; }
        public Header CantScratchPetNotOldEnough { get; private set; }
        public Header CatalogMode { get; private set; }
        public Header CatalogPage { get; private set; }
        public Header CatalogPagesList { get; private set; }
        public Header CatalogSearchResult { get; private set; }
        public Header CatalogUpdated { get; private set; }
        public Header CfhTopicsMessage { get; private set; }
        public Header ChangeNameUpdate { get; private set; }
        public Header CloseWebPage { get; private set; }
        public Header ClubCenterData { get; private set; }
        public Header ClubData { get; private set; }
        public Header ClubGiftReceived { get; private set; }
        public Header ClubGifts { get; private set; }
        public Header CompetitionEntrySubmitResult { get; private set; }
        public Header ConnectionError { get; private set; }
        public Header ConvertedForwardToRoom { get; private set; }
        public Header CraftableProducts { get; private set; }
        public Header CraftingComposerFour { get; private set; }
        public Header CraftingRecipe { get; private set; }
        public Header CraftingResult { get; private set; }
        public Header CustomNotification { get; private set; }
        public Header DailyQuest { get; private set; }
        public Header DebugConsole { get; private set; }
        public Header Discount { get; private set; }
        public Header DoorbellAddUser { get; private set; }
        public Header EffectsListAdd { get; private set; }
        public Header EffectsListEffectEnable { get; private set; }
        public Header EffectsListRemove { get; private set; }
        public Header EpicPopupFrame { get; private set; }
        public Header ErrorLogin { get; private set; }
        public Header ExtendClubMessage { get; private set; }
        public Header FavoriteRoomChanged { get; private set; }
        public Header FavoriteRoomsCount { get; private set; }
        public Header FloodCounter { get; private set; }
        public Header FloorItemUpdate { get; private set; }
        public Header FloorPlanEditorBlockedTiles { get; private set; }
        public Header FloorPlanEditorDoorSettings { get; private set; }
        public Header ForwardToRoom { get; private set; }
        public Header FreezeLives { get; private set; }
        public Header FriendChatMessage { get; private set; }
        public Header FriendFindingRoom { get; private set; }
        public Header FriendRequest { get; private set; }
        public Header FriendRequestError { get; private set; }
        public Header FriendToolbarNotification { get; private set; }
        public Header Friends { get; private set; }
        public Header Game2WeeklyLeaderboard { get; private set; }
        public Header Game2WeeklySmallLeaderboard { get; private set; }
        public Header GameAchievementsList { get; private set; }
        public Header GameCenterAccountInfo { get; private set; }
        public Header GameCenterFeaturedPlayers { get; private set; }
        public Header GameCenterGame { get; private set; }
        public Header GameCenterGameList { get; private set; }
        public Header GenerateSecretKey { get; private set; }
        public Header GenericAlert { get; private set; }
        public Header GenericErrorMessages { get; private set; }
        public Header GiftConfiguration { get; private set; }
        public Header GiftReceiverNotFound { get; private set; }
        public Header GroupParts { get; private set; }
        public Header GuardianNewReportReceived { get; private set; }
        public Header GuardianVotingRequested { get; private set; }
        public Header GuardianVotingResult { get; private set; }
        public Header GuardianVotingTimeEnded { get; private set; }
        public Header GuardianVotingVotes { get; private set; }
        public Header GuideSessionAttached { get; private set; }
        public Header GuideSessionDetached { get; private set; }
        public Header GuideSessionEnded { get; private set; }
        public Header GuideSessionError { get; private set; }
        public Header GuideSessionInvitedToGuideRoom { get; private set; }
        public Header GuideSessionMessage { get; private set; }
        public Header GuideSessionPartnerIsPlaying { get; private set; }
        public Header GuideSessionPartnerIsTyping { get; private set; }
        public Header GuideSessionRequesterRoom { get; private set; }
        public Header GuideSessionStarted { get; private set; }
        public Header GuideTools { get; private set; }
        public Header GuildAcceptMemberError { get; private set; }
        public Header GuildBought { get; private set; }
        public Header GuildBuyRooms { get; private set; }
        public Header GuildConfirmRemoveMember { get; private set; }
        public Header GuildEditFail { get; private set; }
        public Header GuildFavoriteRoomUserUpdate { get; private set; }
        public Header GuildForumAddComment { get; private set; }
        public Header GuildForumComments { get; private set; }
        public Header GuildForumData { get; private set; }
        public Header GuildForumList { get; private set; }
        public Header GuildForumThreadMessages { get; private set; }
        public Header GuildForumThreads { get; private set; }
        public Header GuildForumsUnreadMessagesCount { get; private set; }
        public Header GuildFurniWidget { get; private set; }
        public Header GuildInfo { get; private set; }
        public Header GuildJoinError { get; private set; }
        public Header GuildList { get; private set; }
        public Header GuildManage { get; private set; }
        public Header GuildMemberUpdate { get; private set; }
        public Header GuildMembers { get; private set; }
        public Header GuildRefreshMembersList { get; private set; }
        public Header HabboMall { get; private set; }
        public Header HabboWayQuizComposer1 { get; private set; }
        public Header HabboWayQuizComposer2 { get; private set; }
        public Header HallOfFame { get; private set; }
        public Header HelperRequestDisabled { get; private set; }
        public Header HideDoorbell { get; private set; }
        public Header HotelClosedAndOpens { get; private set; }
        public Header HotelClosesAndWillOpenAt { get; private set; }
        public Header HotelViewBadgeButtonConfig { get; private set; }
        public Header HotelViewCatalogPageExpiring { get; private set; }
        public Header HotelViewCommunityGoal { get; private set; }
        public Header HotelView { get; private set; }
        public Header HotelViewConcurrentUsers { get; private set; }
        public Header HotelViewCustomTimer { get; private set; }
        public Header HotelViewData { get; private set; }
        public Header HotelViewExpiringCatalogPageCommposer { get; private set; }
        public Header HotelViewHideCommunityVoteButton { get; private set; }
        public Header HotelViewNextLTDAvailable { get; private set; }
        public Header HotelWillCloseInMinutesAndBackIn { get; private set; }
        public Header HotelWillCloseInMinutes { get; private set; }
        public Header InitFriends { get; private set; }
        public Header InventoryAchievements { get; private set; }
        public Header InventoryAddEffect { get; private set; }
        public Header InventoryBadges { get; private set; }
        public Header InventoryBots { get; private set; }
        public Header InventoryItemUpdate { get; private set; }
        public Header InventoryItems { get; private set; }
        public Header InventoryPets { get; private set; }
        public Header InventoryRefresh { get; private set; }
        public Header ItemExtraData { get; private set; }
        public Header ItemState { get; private set; }
        public Header ItemStateComposer2 { get; private set; }
        public Header ItemsDataUpdate { get; private set; }
        public Header JukeBoxMySongs { get; private set; }
        public Header JukeBoxNowPlayingMessage { get; private set; }
        public Header JukeBoxPlayListAddSong { get; private set; }
        public Header JukeBoxPlayList { get; private set; }
        public Header JukeBoxPlayListUpdated { get; private set; }
        public Header JukeBoxPlaylistFull { get; private set; }
        public Header JukeBoxTrackCode { get; private set; }
        public Header JukeBoxTrackData { get; private set; }
        public Header LatencyResponse { get; private set; }
        public Header LeprechaunStarterBundle { get; private set; }
        public Header LoadFriendRequests { get; private set; }
        public Header LoveLockFurniFinished { get; private set; }
        public Header LoveLockFurniFriendConfirmed { get; private set; }
        public Header LoveLockFurniStart { get; private set; }
        public Header MachineID { get; private set; }
        public Header MarketplaceBuyError { get; private set; }
        public Header MarketplaceCancelSale { get; private set; }
        public Header MarketplaceConfig { get; private set; }
        public Header MarketplaceItemInfo { get; private set; }
        public Header MarketplaceItemPosted { get; private set; }
        public Header MarketplaceOffers { get; private set; }
        public Header MarketplaceOwnItems { get; private set; }
        public Header MarketplaceSellItem { get; private set; }
        public Header MeMenuSettings { get; private set; }
        public Header MessagesForYou { get; private set; }
        public Header MessengerError { get; private set; }
        public Header MessengerInit { get; private set; }
        public Header MinimailCount { get; private set; }
        public Header MinimailNewMessage { get; private set; }
        public Header ModTool { get; private set; }
        public Header ModToolComposerOne { get; private set; }
        public Header ModToolComposerTwo { get; private set; }
        public Header ModToolIssueChatlog { get; private set; }
        public Header ModToolIssueHandled { get; private set; }
        public Header ModToolIssueHandlerDimensions { get; private set; }
        public Header ModToolIssueInfo { get; private set; }
        public Header ModToolIssueResponseAlert { get; private set; }
        public Header ModToolIssueUpdate { get; private set; }
        public Header ModToolReportReceivedAlert { get; private set; }
        public Header ModToolRoomChatlog { get; private set; }
        public Header ModToolRoomInfo { get; private set; }
        public Header ModToolSanctionData { get; private set; }
        public Header ModToolSanctionInfo { get; private set; }
        public Header ModToolUserChatlog { get; private set; }
        public Header ModToolUserInfo { get; private set; }
        public Header ModToolUserRoomVisits { get; private set; }
        public Header MoodLightData { get; private set; }
        public Header MutedWhisper { get; private set; }
        public Header MysticBoxClose { get; private set; }
        public Header MysticBoxPrize { get; private set; }
        public Header MysticBoxStartOpen { get; private set; }
        public Header NewNavigatorCategoryUserCount { get; private set; }
        public Header NewNavigatorCollapsedCategories { get; private set; }
        public Header NewNavigatorEventCategories { get; private set; }
        public Header NewNavigatorLiftedRooms { get; private set; }
        public Header NewNavigatorMetaData { get; private set; }
        public Header NewNavigatorRoomEvent { get; private set; }
        public Header NewNavigatorSavedSearches { get; private set; }
        public Header NewNavigatorSearchResults { get; private set; }
        public Header NewNavigatorSettings { get; private set; }
        public Header NewUserGift { get; private set; }
        public Header NewUserIdentity { get; private set; }
        public Header NewYearResolutionCompleted { get; private set; }
        public Header NewYearResolution { get; private set; }
        public Header NewYearResolutionProgress { get; private set; }
        public Header NewsWidgets { get; private set; }
        public Header NotEnoughPointsType { get; private set; }
        public Header NuxAlert { get; private set; }
        public Header ObjectOnRoller { get; private set; }
        public Header OldPublicRooms { get; private set; }
        public Header OpenRoomCreationWindow { get; private set; }
        public Header OtherTradingDisabled { get; private set; }
        public Header PetBoughtNotification { get; private set; }
        public Header PetBreedingCompleted { get; private set; }
        public Header PetBreedingFailed { get; private set; }
        public Header PetBreedingResult { get; private set; }
        public Header PetBreedingStart { get; private set; }
        public Header PetBreedingStartFailed { get; private set; }
        public Header PetBreeds { get; private set; }
        public Header PetError { get; private set; }
        public Header PetInfo { get; private set; }
        public Header PetLevelUp { get; private set; }
        public Header PetLevelUpdated { get; private set; }
        public Header PetNameError { get; private set; }
        public Header PetPackageNameValidation { get; private set; }
        public Header PetStatusUpdate { get; private set; }
        public Header PetTrainingPanel { get; private set; }
        public Header PickMonthlyClubGiftNotification { get; private set; }
        public Header Ping { get; private set; }
        public Header PollQuestions { get; private set; }
        public Header PollStart { get; private set; }
        public Header PostItData { get; private set; }
        public Header PostItStickyPoleOpen { get; private set; }
        public Header PresentItemOpened { get; private set; }
        public Header PrivateRooms { get; private set; }
        public Header ProfileFriends { get; private set; }
        public Header PromoteOwnRoomsList { get; private set; }
        public Header PublicRooms { get; private set; }
        public Header PurchaseOK { get; private set; }
        public Header QuestCompleted { get; private set; }
        public Header QuestExpired { get; private set; }
        public Header ReceiveInvitation { get; private set; }
        public Header ReceivePrivateMessage { get; private set; }
        public Header RecyclerComplete { get; private set; }
        public Header RecyclerLogic { get; private set; }
        public Header RedeemVoucherError { get; private set; }
        public Header RedeemVoucherOK { get; private set; }
        public Header ReloadRecycler { get; private set; }
        public Header RemoveBot { get; private set; }
        public Header RemoveFloorItem { get; private set; }
        public Header RemoveFriend { get; private set; }
        public Header RemoveGuildFromRoom { get; private set; }
        public Header RemoveHabboItem { get; private set; }
        public Header RemovePet { get; private set; }
        public Header RemoveRoomEvent { get; private set; }
        public Header RemoveWallItem { get; private set; }
        public Header RentableItemBuyOutPrice { get; private set; }
        public Header RentableSpaceInfo { get; private set; }
        public Header ReportRoomForm { get; private set; }
        public Header RoomAccessDenied { get; private set; }
        public Header RoomAdError { get; private set; }
        public Header RoomAddRightsList { get; private set; }
        public Header RoomBannedUsers { get; private set; }
        public Header RoomCategories { get; private set; }
        public Header RoomCategoryUpdateMessage { get; private set; }
        public Header RoomChatSettings { get; private set; }
        public Header RoomCreated { get; private set; }
        public Header RoomData { get; private set; }
        public Header RoomEditSettingsError { get; private set; }
        public Header RoomEnterError { get; private set; }
        public Header RoomEntryInfo { get; private set; }
        public Header RoomEventMessage { get; private set; }
        public Header RoomFilterWords { get; private set; }
        public Header RoomFloorItems { get; private set; }
        public Header RoomFloorThicknessUpdated { get; private set; }
        public Header RoomHeightMap { get; private set; }
        public Header RoomInvite { get; private set; }
        public Header RoomInviteError { get; private set; }
        public Header RoomMessagesPostedCount { get; private set; }
        public Header RoomModel { get; private set; }
        public Header RoomMuted { get; private set; }
        public Header RoomNoRights { get; private set; }
        public Header RoomOpen { get; private set; }
        public Header RoomOwner { get; private set; }
        public Header RoomPaint { get; private set; }
        public Header RoomPetExperience { get; private set; }
        public Header RoomPetHorseFigure { get; private set; }
        public Header RoomPetRespect { get; private set; }
        public Header RoomRelativeMap { get; private set; }
        public Header RoomRemoveRightsList { get; private set; }
        public Header RoomRights { get; private set; }
        public Header RoomRightsList { get; private set; }
        public Header RoomScore { get; private set; }
        public Header RoomSettings { get; private set; }
        public Header RoomSettingsSaved { get; private set; }
        public Header RoomSettingsUpdated { get; private set; }
        public Header RoomThickness { get; private set; }
        public Header RoomUnitIdle { get; private set; }
        public Header RoomUserAction { get; private set; }
        public Header RoomUserDance { get; private set; }
        public Header RoomUserData { get; private set; }
        public Header RoomUserEffect { get; private set; }
        public Header RoomUserHandItem { get; private set; }
        public Header RoomUserIgnored { get; private set; }
        public Header RoomUserNameChanged { get; private set; }
        public Header RoomUserReceivedHandItem { get; private set; }
        public Header RoomUserRemove { get; private set; }
        public Header RoomUserRemoveRights { get; private set; }
        public Header RoomUserRespect { get; private set; }
        public Header RoomUserShout { get; private set; }
        public Header RoomUserStatus { get; private set; }
        public Header RoomUserTags { get; private set; }
        public Header RoomUserTalk { get; private set; }
        public Header RoomUserTyping { get; private set; }
        public Header RoomUserUnbanned { get; private set; }
        public Header RoomUserWhisper { get; private set; }
        public Header RoomUsers { get; private set; }
        public Header RoomUsersGuildBadges { get; private set; }
        public Header RoomWallItems { get; private set; }
        public Header SecureLoginOK { get; private set; }
        public Header SessionRights { get; private set; }
        public Header SimplePollAnswer { get; private set; }
        public Header SimplePollAnswers { get; private set; }
        public Header SimplePollStart { get; private set; }
        public Header SpectatingRoom { get; private set; }
        public Header StaffAlertAndOpenHabboWay { get; private set; }
        public Header StaffAlertWIthLinkAndOpenHabboWay { get; private set; }
        public Header StaffAlertWithLink { get; private set; }
        public Header StalkError { get; private set; }
        public Header SubmitCompetitionRoom { get; private set; }
        public Header Tags { get; private set; }
        public Header TalentLevelUpdate { get; private set; }
        public Header TalentTrack { get; private set; }
        public Header TalentTrackEmailFailed { get; private set; }
        public Header TalentTrackEmailVerified { get; private set; }
        public Header TargetedOffer { get; private set; }
        public Header TradeAccepted { get; private set; }
        public Header TradeCloseWindow { get; private set; }
        public Header TradeComplete { get; private set; }
        public Header TradeStart { get; private set; }
        public Header TradeStartFail { get; private set; }
        public Header TradeStopped { get; private set; }
        public Header TradeUpdate { get; private set; }
        public Header TradingWaitingConfirm { get; private set; }
        public Header UpdateFailed { get; private set; }
        public Header UpdateFriend { get; private set; }
        public Header UpdateStackHeight { get; private set; }
        public Header UpdateStackHeightTileHeight { get; private set; }
        public Header UpdateUserLook { get; private set; }
        public Header UserAchievementScore { get; private set; }
        public Header UserBadges { get; private set; }
        public Header UserCitizenShip { get; private set; }
        public Header UserClothes { get; private set; }
        public Header UserClub { get; private set; }
        public Header UserCredits { get; private set; }
        public Header UserCurrency { get; private set; }
        public Header UserData { get; private set; }
        public Header UserEffectsList { get; private set; }
        public Header UserHomeRoom { get; private set; }
        public Header UserPerks { get; private set; }
        public Header UserPermissions { get; private set; }
        public Header UserPoints { get; private set; }
        public Header UserProfile { get; private set; }
        public Header UserSearchResult { get; private set; }
        public Header UserWardrobe { get; private set; }
        public Header VerifyMobileNumber { get; private set; }
        public Header VerifyMobilePhoneCodeWindow { get; private set; }
        public Header VerifyMobilePhoneDone { get; private set; }
        public Header VerifyMobilePhoneWindow { get; private set; }
        public Header VerifyPrimes { get; private set; }
        public Header VipTutorialsStart { get; private set; }
        public Header WallItemUpdate { get; private set; }
        public Header WatchAndEarnReward { get; private set; }
        public Header WelcomeGift { get; private set; }
        public Header WelcomeGiftError { get; private set; }
        public Header WiredConditionData { get; private set; }
        public Header WiredEffectData { get; private set; }
        public Header WiredRewardAlert { get; private set; }
        public Header WiredSaved { get; private set; }
        public Header WiredTriggerData { get; private set; }
        public Header YouTradingDisabled { get; private set; }
        public Header YoutubeDisplayList { get; private set; }
        public Header YoutubeMessageComposer2 { get; private set; }
        public Header YoutubeMessageComposer3 { get; private set; }
    }
}