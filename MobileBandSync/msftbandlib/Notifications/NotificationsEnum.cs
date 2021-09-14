namespace MobileBandSync.MSFTBandLib.Notifications {

/// <summary>Notification IDs</summary>
public enum NotificationsEnum : ushort {

	SMS = 1,

	Email = 2,

	CalendarAddEvent = 16,

	CalendarClear = 17,

	CallAnswered = 12,

	CallHangup = 14,

	CallIncoming = 11,

	CallMissed = 13,

	GenericDialog = 100,

	GenericPageClear = 103,

	GenericTileClear = 102,

	GenericUpdate = 101,

	Messaging = 8,

	Voicemail = 15

}

}