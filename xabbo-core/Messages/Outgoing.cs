﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xabbo.Core.Messages
{
    public sealed class Outgoing : HeaderDictionary
    {
        public Outgoing() : base(Destination.Server) { }

        public Outgoing(IReadOnlyDictionary<string, short> values)
            : base(Destination.Server, values)
        { }

        public Header InfoRetrieve { get; private set; }
        public Header GetCredits { get; private set; }
        public Header MessengerInit { get; private set; }
        public Header FriendListUpdate { get; private set; }
        public Header AddYourFavouriteRoom { get; private set; }
        public Header DeleteYourFavouriteRoom { get; private set; }
        public Header DeleteFlat { get; private set; }
        public Header SubscriptionGetUserInfo { get; private set; }
        public Header SubscriptionGetKickbackInfo { get; private set; }
        public Header CreateNewFlat { get; private set; }
        public Header SendMessage { get; private set; }
        public Header SendRoomInvite { get; private set; }
        public Header AcceptFriend { get; private set; }
        public Header DeclineFriend { get; private set; }
        public Header RequestFriend { get; private set; }
        public Header RemoveFriend { get; private set; }
        public Header HabboSearch { get; private set; }
        public Header ApprovePetName { get; private set; }
        public Header UpdateAvatar { get; private set; }
        public Header CustomizeAvatarWithFurni { get; private set; }
        public Header Chat { get; private set; }
        public Header Quit { get; private set; }
        public Header Shout { get; private set; }
        public Header Whisper { get; private set; }
        public Header PickUpPetFromRoom { get; private set; }
        public Header GoToFlat { get; private set; }
        public Header PickUpAllItemsFromRoom { get; private set; }
        public Header FlatPropertyByItem { get; private set; }
        public Header PickItemUpFromRoom { get; private set; }
        public Header TradeUnaccept { get; private set; }
        public Header TradeAccept { get; private set; }
        public Header TradeClose { get; private set; }
        public Header TradeOpen { get; private set; }
        public Header TradeAddItem { get; private set; }
        public Header MoveRoomItem { get; private set; }
        public Header SetStuffData { get; private set; }
        public Header Move { get; private set; }
        public Header ThrowDice { get; private set; }
        public Header DiceOff { get; private set; }
        public Header PresentOpen { get; private set; }
        public Header LookTo { get; private set; }
        public Header PassHandItem { get; private set; }
        public Header DropHandItem { get; private set; }
        public Header PassHandItemToPet { get; private set; }
        public Header GetItemData { get; private set; }
        public Header SetStickyData { get; private set; }
        public Header RemoveItem { get; private set; }
        public Header Posture { get; private set; }
        public Header GiveSupplementToPet { get; private set; }
        public Header TradeAddItems { get; private set; }
        public Header PlaceStuffFromStripDEPRECATED { get; private set; }
        public Header MoveItemDEPRECATED { get; private set; }
        public Header AmbassadorAlert { get; private set; }
        public Header Dance { get; private set; }
        public Header Expression { get; private set; }
        public Header KickUser { get; private set; }
        public Header AssignRights { get; private set; }
        public Header RemoveRights { get; private set; }
        public Header RemoveOwnRights { get; private set; }
        public Header PurchaseFromCatalog { get; private set; }
        public Header GetCatalogIndex { get; private set; }
        public Header GetCatalogPage { get; private set; }
        public Header ShowSign { get; private set; }
        public Header PlaceRoomItem { get; private set; }
        public Header PlaceWallItem { get; private set; }
        public Header MoveWallItem { get; private set; }
        public Header RelinkTeleports { get; private set; }
        public Header Goaway { get; private set; }
        public Header GetRentOrBuyoutOffer { get; private set; }
        public Header ExtendRentOrBuyoutFurniInRoom { get; private set; }
        public Header ExtendRentOrBuyoutFurniInInventory { get; private set; }
        public Header RedeemVoucherCode { get; private set; }
        public Header RedeemVoucherCodeWithHc { get; private set; }
        public Header GetUserFlatCategories { get; private set; }
        public Header GetEventFlatCats { get; private set; }
        public Header RemoveAllRights { get; private set; }
        public Header GetAvailableBadges { get; private set; }
        public Header SetSelectedBadges { get; private set; }
        public Header GetSelectedBadges { get; private set; }
        public Header InterstitialShown { get; private set; }
        public Header GetInterstitial { get; private set; }
        public Header ConvertFurniToCredits { get; private set; }
        public Header Pong { get; private set; }
        public Header ClientSuspended { get; private set; }
        public Header ClientResumed { get; private set; }
        public Header ModerationAction { get; private set; }
        public Header InitDhHandshake { get; private set; }
        public Header CompleteDhHandshake { get; private set; }
        public Header RoomQueueChange { get; private set; }
        public Header GetOpeningHours { get; private set; }
        public Header SetWallItemAnimationState { get; private set; }
        public Header GetFurniAliases { get; private set; }
        public Header GetSpectatorAmount { get; private set; }
        public Header GetSongInfo { get; private set; }
        public Header GetSongId { get; private set; }
        public Header SetChatPreferences { get; private set; }
        public Header GetAccountPreferences { get; private set; }
        public Header SetSoundSettings { get; private set; }
        public Header GetHabboGroupBadges { get; private set; }
        public Header GetHabboGroupDetails { get; private set; }
        public Header EnterOneWayDoor { get; private set; }
        public Header GetFriendRequests { get; private set; }
        public Header StartPoll { get; private set; }
        public Header RejectPoll { get; private set; }
        public Header PollAnswer { get; private set; }
        public Header GetPendingCallsForHelp { get; private set; }
        public Header DeletePendingCallsForHelp { get; private set; }
        public Header SetRoomInvitePreferences { get; private set; }
        public Header SetRoomCameraPreferences { get; private set; }
        public Header SetNewNavigatorPreferences { get; private set; }
        public Header SetUiFlags { get; private set; }
        public Header SetChatStylePreference { get; private set; }
        public Header GetSoundMachinePlayList { get; private set; }
        public Header SpinWheelOfFortune { get; private set; }
        public Header GetNowPlaying { get; private set; }
        public Header AddJukeboxDisc { get; private set; }
        public Header RemoveJukeboxDisc { get; private set; }
        public Header GetJukeboxDiscs { get; private set; }
        public Header GetUserSongDiscs { get; private set; }
        public Header RemoveAllJukeboxDiscs { get; private set; }
        public Header RateFlat { get; private set; }
        public Header FollowFriend { get; private set; }
        public Header VisitUser { get; private set; }
        public Header SetFurniRandomState { get; private set; }
        public Header ClientLatencyPingRequest { get; private set; }
        public Header ClientLatencyPingReport { get; private set; }
        public Header UserStartTyping { get; private set; }
        public Header UserCancelTyping { get; private set; }
        public Header IgnoreUser { get; private set; }
        public Header IgnoreAvatarId { get; private set; }
        public Header GetIgnoreList { get; private set; }
        public Header UnignoreUser { get; private set; }
        public Header RoomBanWithDuration { get; private set; }
        public Header RoomMuteUser { get; private set; }
        public Header RoomMuteUnmuteAll { get; private set; }
        public Header RoomUnmuteUser { get; private set; }
        public Header RoomDimmerEditPresets { get; private set; }
        public Header RoomDimmerSavePreset { get; private set; }
        public Header RoomDimmerChangeState { get; private set; }
        public Header RoomMuteAll { get; private set; }
        public Header RoomAdCancelAd { get; private set; }
        public Header RoomAdEditAd { get; private set; }
        public Header PurchaseRoomAd { get; private set; }
        public Header RoomAdGetRooms { get; private set; }
        public Header RoomAdListAds { get; private set; }
        public Header RoomAdEventTabViewed { get; private set; }
        public Header RoomAdEventTabAdClicked { get; private set; }
        public Header RoomAdPurchaseInitiated { get; private set; }
        public Header GetUserAchievements { get; private set; }
        public Header RespectUser { get; private set; }
        public Header UseAvatarEffect { get; private set; }
        public Header ActivateAvatarEffect { get; private set; }
        public Header PurchaseAndActivateAvatarEffect { get; private set; }
        public Header GetWardrobe { get; private set; }
        public Header SaveWardrobeOutfit { get; private set; }
        public Header MysteryBoxWaitingCanceled { get; private set; }
        public Header ResetResolutionAchievement { get; private set; }
        public Header GetUserAchievementsForAResolution { get; private set; }
        public Header GetOfficialRooms { get; private set; }
        public Header FriendFurnitureLockConfirm { get; private set; }
        public Header GetPopularRoomTags { get; private set; }
        public Header GetCategoriesWithVisitorCount { get; private set; }
        public Header UpdateNavigatorSettings { get; private set; }
        public Header GetGuestRoom { get; private set; }
        public Header UpdateRoomThumbnail { get; private set; }
        public Header CanCreateRoom { get; private set; }
        public Header ConvertGlobalRoomId { get; private set; }
        public Header GetRoomEntryData { get; private set; }
        public Header FlatOpc { get; private set; }
        public Header UseStuff { get; private set; }
        public Header UseWallItem { get; private set; }
        public Header GetRoomSettings { get; private set; }
        public Header SaveRoomSettings { get; private set; }
        public Header TradeConfirmAccept { get; private set; }
        public Header TradeConfirmDecline { get; private set; }
        public Header GetInventory { get; private set; }
        public Header TradeRemoveItem { get; private set; }
        public Header GetInventoryPeer { get; private set; }
        public Header UpdateRoomFilter { get; private set; }
        public Header GetRoomFilter { get; private set; }
        public Header UpdateRoomCategoryAndTrade { get; private set; }
        public Header StageStartPerformance { get; private set; }
        public Header StageVotePerformance { get; private set; }
        public Header LoginWithTicket { get; private set; }
        public Header GetClientFaqs { get; private set; }
        public Header GetFaqCategories { get; private set; }
        public Header GetFaqText { get; private set; }
        public Header SearchFaqs { get; private set; }
        public Header GetFaqCategory { get; private set; }
        public Header LogFlashPerformance { get; private set; }
        public Header LogLagWarning { get; private set; }
        public Header LogAirPerformance { get; private set; }
        public Header PopularRoomsSearch { get; private set; }
        public Header RoomsWithHighestScoreSearch { get; private set; }
        public Header MyFriendsRoomsSearch { get; private set; }
        public Header RoomsWhereMyFriendsAreSearch { get; private set; }
        public Header MyRoomsSearch { get; private set; }
        public Header MyFavouriteRoomsSearch { get; private set; }
        public Header MyRoomHistorySearch { get; private set; }
        public Header RoomTextSearch { get; private set; }
        public Header RoomTagSearch { get; private set; }
        public Header MyFrequentlyVisitedRoomsSearch { get; private set; }
        public Header GuildBaseSearch { get; private set; }
        public Header RoomAdSearch { get; private set; }
        public Header MyRoomRightsSearch { get; private set; }
        public Header MyGuildBasesSearch { get; private set; }
        public Header MyRecommendedRoomsSearch { get; private set; }
        public Header CloseIssueDefaultAction { get; private set; }
        public Header PickIssues { get; private set; }
        public Header ReleaseIssues { get; private set; }
        public Header CloseIssues { get; private set; }
        public Header CreateIssue { get; private set; }
        public Header GetModeratorUserInfo { get; private set; }
        public Header GetUserChatLog { get; private set; }
        public Header GetRoomChatLog { get; private set; }
        public Header GetCfhChatLog { get; private set; }
        public Header GetRoomVisits { get; private set; }
        public Header GetModeratorRoomInfo { get; private set; }
        public Header ModerateRoom { get; private set; }
        public Header ModAlert { get; private set; }
        public Header ModMessage { get; private set; }
        public Header ModKick { get; private set; }
        public Header ModBan { get; private set; }
        public Header ModMute { get; private set; }
        public Header ModToolPreferences { get; private set; }
        public Header ModToolNextSanction { get; private set; }
        public Header ModTradingLock { get; private set; }
        public Header CreateImIssue { get; private set; }
        public Header ChangeAvatarNameInRoom { get; private set; }
        public Header CheckAvatarName { get; private set; }
        public Header PurchaseFromCatalogAsGift { get; private set; }
        public Header GetGiftWrappingConfiguration { get; private set; }
        public Header GetSelectableClubGiftInfo { get; private set; }
        public Header SelectClubGift { get; private set; }
        public Header ChangeAvatarName { get; private set; }
        public Header DefaultSanction { get; private set; }
        public Header GetRoomTypes { get; private set; }
        public Header SetClothingChangeFurnitureData { get; private set; }
        public Header LogToEventLog { get; private set; }
        public Header ToggleRoomStaffPick { get; private set; }
        public Header ChangeAvatarMotto { get; private set; }
        public Header ForwardToARandomPromotedRoom { get; private set; }
        public Header ForwardToSomeRoom { get; private set; }
        public Header FriendBarHelperFindFriends { get; private set; }
        public Header SetRelationshipStatus { get; private set; }
        public Header GetRelationshipStatusInfo { get; private set; }
        public Header GetEventStream { get; private set; }
        public Header SetEventStreamPublishingAllowed { get; private set; }
        public Header StreamLike { get; private set; }
        public Header StreamStatus { get; private set; }
        public Header GetEventStreamForAccount { get; private set; }
        public Header StreamComment { get; private set; }
        public Header GetStreamNotificationCount { get; private set; }
        public Header GetStreamNotifications { get; private set; }
        public Header Disconnect { get; private set; }
        public Header GetCommunityGoalProgress { get; private set; }
        public Header SubmitRoomToCompetition { get; private set; }
        public Header RoomCompetitionInit { get; private set; }
        public Header VoteForRoom { get; private set; }
        public Header GetSecondsUntil { get; private set; }
        public Header ForwardToRandomCompetitionRoom { get; private set; }
        public Header CompetitionRoomsSearch { get; private set; }
        public Header GetCommunityGoalHallOfFame { get; private set; }
        public Header ForwardToACompetitionRoom { get; private set; }
        public Header IsUserPartOfCompetition { get; private set; }
        public Header GetCurrentTimingCode { get; private set; }
        public Header ForwardToASubmittableRoom { get; private set; }
        public Header GetConcurrentUsersGoalProgress { get; private set; }
        public Header RequestConcurrentUsersGoalReward { get; private set; }
        public Header HelpRequestSessionCreate { get; private set; }
        public Header HelpRequestSessionGuideDecides { get; private set; }
        public Header HelpRequestSessionRequesterCancels { get; private set; }
        public Header HelpRequestSessionResolved { get; private set; }
        public Header HelpRequestSessionFeedback { get; private set; }
        public Header GuideOnDutyUpdate { get; private set; }
        public Header HelpRequestSessionMessage { get; private set; }
        public Header HelpRequestSessionGetRequesterRoom { get; private set; }
        public Header HelpRequestSessionReported { get; private set; }
        public Header HelpRequestSessionInviteRequester { get; private set; }
        public Header HelpRequestSessionTyping { get; private set; }
        public Header QuizGetQuestions { get; private set; }
        public Header QuizPostAnswers { get; private set; }
        public Header GetTalentTrack { get; private set; }
        public Header GetTalentTrackLevel { get; private set; }
        public Header GuideAdvertisementRead { get; private set; }
        public Header ChangePetName { get; private set; }
        public Header GetPetConfigurations { get; private set; }
        public Header ChangePassword { get; private set; }
        public Header LoginWithPassword { get; private set; }
        public Header LoginWithPasswordDEPRECATED { get; private set; }
        public Header LoginWithFacebookToken { get; private set; }
        public Header LoginWithToken { get; private set; }
        public Header CreateAccount { get; private set; }
        public Header CaptchaRequest { get; private set; }
        public Header ClearDeviceLoginToken { get; private set; }
        public Header GetDeviceLoginTokens { get; private set; }
        public Header UniqueMachineId { get; private set; }
        public Header ClientStatistics { get; private set; }
        public Header GetAccountProgressionInfo { get; private set; }
        public Header GetHotlooks { get; private set; }
        public Header GetAvatarList { get; private set; }
        public Header CreateNewAvatar { get; private set; }
        public Header DeactivateAvatar { get; private set; }
        public Header GetInitialRooms { get; private set; }
        public Header SelectInitialRoom { get; private set; }
        public Header GetCfhStatus { get; private set; }
        public Header RoomNetworkForward { get; private set; }
        public Header Navigator2Init { get; private set; }
        public Header Navigator2Search { get; private set; }
        public Header Navigator2AddSavedSearch { get; private set; }
        public Header Navigator2DeleteSavedSearch { get; private set; }
        public Header Navigator2AddCollapsedCategory { get; private set; }
        public Header Navigator2RemoveCollapsedCategory { get; private set; }
        public Header Navigator2SetSearchCodeViewMode { get; private set; }
        public Header VersionCheck { get; private set; }
        public Header GetCraftableProducts { get; private set; }
        public Header GetCraftingRecipe { get; private set; }
        public Header Craft { get; private set; }
        public Header CraftSecret { get; private set; }
        public Header GetCraftingRecipesAvailable { get; private set; }
        public Header GetProductOffers { get; private set; }
        public Header OpenCampaignCalendarDoor { get; private set; }
        public Header StaffOpenCampaignCalendarDoor { get; private set; }
        public Header PickUpAllFurniAndResetHeightmap { get; private set; }
        public Header UpdateRoomFloorProperties { get; private set; }
        public Header StackingHelperSetCaretHeight { get; private set; }
        public Header GetRoomEntryTile { get; private set; }
        public Header GetRoomOccupiedTiles { get; private set; }
        public Header YoutubeDisplayGetStatus { get; private set; }
        public Header YoutubeDisplaySetPlaylist { get; private set; }
        public Header YoutubeDisplayControlPlayback { get; private set; }
        public Header RentableSpaceGetInfo { get; private set; }
        public Header RentableSpaceRentSpace { get; private set; }
        public Header RentableSpaceExtendRent { get; private set; }
        public Header RentableSpaceCancel { get; private set; }
        public Header BuildersClubPlaceRoomItem { get; private set; }
        public Header BuildersClubPlaceWallItem { get; private set; }
        public Header BuildersClubQueryFurniCount { get; private set; }
        public Header TryPhoneNumber { get; private set; }
        public Header VerifyCode { get; private set; }
        public Header SetPhoneNumberCollectionStatus { get; private set; }
        public Header GiveGift { get; private set; }
        public Header RestartPhoneNumberCollection { get; private set; }
        public Header GiveStarGems { get; private set; }
        public Header NuxGetGifts { get; private set; }
        public Header ScriptProceed { get; private set; }
        public Header GetNextTargetedOffer { get; private set; }
        public Header PurchaseTargetedOffer { get; private set; }
        public Header SetTargetedOfferState { get; private set; }
        public Header ShopTargetedOfferViewed { get; private set; }
        public Header GetTargetedOffer { get; private set; }
        public Header GetTargetedOfferList { get; private set; }
        public Header GetGuideReportingStatus { get; private set; }
        public Header ChatReviewSessionCreate { get; private set; }
        public Header ChatReviewSessionGuideDecidesOnOffer { get; private set; }
        public Header ChatReviewSessionGuideVote { get; private set; }
        public Header ChatReviewSessionGuideDetached { get; private set; }
        public Header AccountSafetylockGetQuestions { get; private set; }
        public Header AccountSafetylockUnlock { get; private set; }
        public Header AccountSafetyLock { get; private set; }
        public Header SubmitGdprRequest { get; private set; }
        public Header CancelGdprRequest { get; private set; }
        public Header GetGdprRequest { get; private set; }
        public Header GetForumStats { get; private set; }
        public Header GetForumThreads { get; private set; }
        public Header GetForumThreadMessages { get; private set; }
        public Header UpdateForumReadMarkers { get; private set; }
        public Header GetForumThread { get; private set; }
        public Header GetForumsList { get; private set; }
        public Header UpdateForumSettings { get; private set; }
        public Header GetUnreadForumsCount { get; private set; }
        public Header PostForumMessage { get; private set; }
        public Header ModerateForumThread { get; private set; }
        public Header ModerateForumMessage { get; private set; }
        public Header ReportForumThread { get; private set; }
        public Header ReportForumMessage { get; private set; }
        public Header UpdateForumThread { get; private set; }
        public Header GetRoomUsersClassification { get; private set; }
        public Header GetPeerUsersClassification { get; private set; }
        public Header ReportSelfie { get; private set; }
        public Header ReportPhoto { get; private set; }
        public Header UserFeedback { get; private set; }
        public Header GetReputation { get; private set; }
        public Header GetFurniByRoomInventory { get; private set; }
        public Header GetInventoryForDebugging { get; private set; }
        public Header GetPetInventory { get; private set; }
        public Header GetNewPetInfo { get; private set; }
        public Header PlacePetToFlat { get; private set; }
        public Header RemovePetFromFlat { get; private set; }
        public Header GetAvailablePetCommands { get; private set; }
        public Header RespectPet { get; private set; }
        public Header OpenPetPackage { get; private set; }
        public Header GetSellablePetPalettes { get; private set; }
        public Header CustomizePetWithFurni { get; private set; }
        public Header RemoveSaddle { get; private set; }
        public Header MarketplaceMakeOffer { get; private set; }
        public Header MarketplaceGetConfiguration { get; private set; }
        public Header MarketplaceCanMakeOffer { get; private set; }
        public Header MarketplaceBuyTokens { get; private set; }
        public Header MarketplaceBuyOffer { get; private set; }
        public Header MarketplaceCancelOffer { get; private set; }
        public Header MarketplaceRedeemOfferCredits { get; private set; }
        public Header MountPet { get; private set; }
        public Header MarketplaceSearchOffers { get; private set; }
        public Header MarketplaceListOwnOffers { get; private set; }
        public Header MarketplaceGetItemStats { get; private set; }
        public Header TogglePetRidingAccessRights { get; private set; }
        public Header PetSelected { get; private set; }
        public Header MovePetInFlat { get; private set; }
        public Header BreedPets { get; private set; }
        public Header HarvestPet { get; private set; }
        public Header TogglePetBreedingRights { get; private set; }
        public Header PlaceBotToFlat { get; private set; }
        public Header GetBotInventory { get; private set; }
        public Header CommandBot { get; private set; }
        public Header GetIsOfferGiftable { get; private set; }
        public Header GetHabboClubOffers { get; private set; }
        public Header GetBadgePointLimits { get; private set; }
        public Header ChargeStuff { get; private set; }
        public Header MarkCatalogNewAdditionsPageOpened { get; private set; }
        public Header GetHabboVipMembershipExtendOffer { get; private set; }
        public Header PurchaseDiscountedVipMembershipExtension { get; private set; }
        public Header PurchaseDiscountedBasicMembershipExtension { get; private set; }
        public Header GetHabboBasicMembershipExtendOffer { get; private set; }
        public Header GetSnowWarGameTokensOffer { get; private set; }
        public Header PurchaseSnowWarGameTokens { get; private set; }
        public Header GetBundleDiscountRuleset { get; private set; }
        public Header GetProductOffer { get; private set; }
        public Header RequestBadge { get; private set; }
        public Header GetIsBadgeRequestFulfilled { get; private set; }
        public Header ConfirmPetBreeding { get; private set; }
        public Header CancelPetBreeding { get; private set; }
        public Header CompostPlant { get; private set; }
        public Header MarketplaceCancelAllOffers { get; private set; }
        public Header SendPetToHoliday { get; private set; }
        public Header UserDefinedRoomEventsUpdateTrigger { get; private set; }
        public Header UserDefinedRoomEventsUpdateAction { get; private set; }
        public Header UserDefinedRoomEventsUpdateCondition { get; private set; }
        public Header UserDefinedRoomEventsOpen { get; private set; }
        public Header UserDefinedRoomEventsApplySnapshot { get; private set; }
        public Header RemoveBotFromFlat { get; private set; }
        public Header GetQuests { get; private set; }
        public Header AcceptQuest { get; private set; }
        public Header RejectQuest { get; private set; }
        public Header OpenQuestTracker { get; private set; }
        public Header StartCampaign { get; private set; }
        public Header GetDailyQuest { get; private set; }
        public Header GetMessageOfTheDay { get; private set; }
        public Header ResetUnseenCounter { get; private set; }
        public Header ActivateQuest { get; private set; }
        public Header CancelQuest { get; private set; }
        public Header GetSeasonalQuests { get; private set; }
        public Header FriendRequestQuestComplete { get; private set; }
        public Header GetIdentityAgreementTypes { get; private set; }
        public Header SaveAgreements { get; private set; }
        public Header GetIdentityAgreements { get; private set; }
        public Header GetEmailStatus { get; private set; }
        public Header ChangeEmail { get; private set; }
        public Header PlacePostIt { get; private set; }
        public Header AddSpamWallPostIt { get; private set; }
        public Header SetMannequinFigure { get; private set; }
        public Header JoinHabboGroup { get; private set; }
        public Header SelectFavouriteHabboGroup { get; private set; }
        public Header DeselectFavouriteHabboGroup { get; private set; }
        public Header GetExtendedProfile { get; private set; }
        public Header GetGuildCreationInfo { get; private set; }
        public Header CreateGuild { get; private set; }
        public Header GetGuildEditInfo { get; private set; }
        public Header UpdateGuildIdentity { get; private set; }
        public Header GetGuildEditorData { get; private set; }
        public Header GetGuildMembershipRequests { get; private set; }
        public Header ApproveMembershipRequest { get; private set; }
        public Header RejectMembershipRequest { get; private set; }
        public Header ApproveAllMembershipRequests { get; private set; }
        public Header RemoveAdminRightsFromMember { get; private set; }
        public Header AddAdminRightsToMember { get; private set; }
        public Header KickMember { get; private set; }
        public Header UpdateGuildSettings { get; private set; }
        public Header GetFlatControllers { get; private set; }
        public Header UpdateGuildBadge { get; private set; }
        public Header UpdateGuildColors { get; private set; }
        public Header GetGuildMembers { get; private set; }
        public Header GetGuildMemberships { get; private set; }
        public Header GuildMemberHqFurniCount { get; private set; }
        public Header SetMannequinName { get; private set; }
        public Header GetGuildFurniContextMenuInfo { get; private set; }
        public Header GetDirectClubBuyAllowed { get; private set; }
        public Header GetExtendedProfileByUsername { get; private set; }
        public Header GetSeasonalCalendarDailyOffer { get; private set; }
        public Header SetRoomSessionTags { get; private set; }
        public Header GetCatalogPageWithEarliestExpiry { get; private set; }
        public Header GetLimitedFurniTimingInfo { get; private set; }
        public Header GetCommunityGoalEarnedPrizes { get; private set; }
        public Header RedeemCommunityGoalPrize { get; private set; }
        public Header DeactivateGuild { get; private set; }
        public Header GetCatalogPageExpiration { get; private set; }
        public Header SetRoomBackgroundColorData { get; private set; }
        public Header GetBannedUsers { get; private set; }
        public Header RoomUnbanUser { get; private set; }
        public Header GetPromoArticles { get; private set; }
        public Header GetBonusRareInfo { get; private set; }
        public Header OpenMysteryTrophy { get; private set; }
        public Header CommunityGoalVote { get; private set; }
        public Header GetFlatFavouriteCount { get; private set; }
        public Header GetExternalImageFurniData { get; private set; }
        public Header StartCreateGuild { get; private set; }
        public Header CommitCreateGuild { get; private set; }
        public Header GetBotCommandConfigurationData { get; private set; }
        public Header RemoveUnseenElements { get; private set; }
        public Header RemoveUnseenElement { get; private set; }
        public Header RequestCameraToken { get; private set; }
        public Header RenderRoom { get; private set; }
        public Header PurchasePhoto { get; private set; }
        public Header PublishPhoto { get; private set; }
        public Header InitCamera { get; private set; }
        public Header CompetitionPhoto { get; private set; }
        public Header RenderAndSaveRoomThumbnailPhoto { get; private set; }
        public Header MeltdownWatchVerify { get; private set; }
        public Header EarningStatus { get; private set; }
        public Header ClaimEarning { get; private set; }
        public Header VaultStatus { get; private set; }
        public Header WithdrawVault { get; private set; }
        public Header UpdateAccountPreferences { get; private set; }
        public Header UnlinkIdentificationMethod { get; private set; }
        public Header LinkIdentificationMethod { get; private set; }
        public Header ClientDebugPingPeerSlow { get; private set; }
        public Header ClientDebugPingPeerFast { get; private set; }
        public Header ClientDebugPingRoomSlow { get; private set; }
        public Header ClientDebugPingRoomFast { get; private set; }
        public Header ClientDebugPingMessengerSlow { get; private set; }
        public Header ClientDebugPingMessengerFast { get; private set; }
        public Header ClientDebugPingNavigatorSlow { get; private set; }
        public Header ClientDebugPingNavigatorFast { get; private set; }
        public Header ClientDebugPingRoomDirectorySlow { get; private set; }
        public Header ClientDebugPingRoomDirectoryFast { get; private set; }
        public Header ClientDebugPingProxySlow { get; private set; }
        public Header ClientDebugPingProxyFast { get; private set; }
        public Header Hello { get; private set; }
        public Header GetProxyId { get; private set; }
    }
}
