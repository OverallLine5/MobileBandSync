namespace MobileBandSync.MSFTBandLib.Command {

/// <summary>Commands</summary>
public enum CommandEnum : ushort {

	ChunkCounts = 35977,

	ChunkRangeDelete = 35856,

	ChunkRangeGetData = 35983,

	ChunkRangeGetMetadata = 35982,

	FlushLog = 35853,

	GetApiVersion = 30342,

	GetDeviceName = 30339,

	[CommandDataSize(16)]
	GetDeviceTime = 30082,

	GetLogVersion = 30341,

	GetMaxTileCount = 54421,

	GetMaxTileCountAllocated = 54422,

	GetMeTileImage = 50062,

	GetMeTileImageId = 51858,

	GetSdkVersion = 30215,

	[CommandDataSize(12)]
	GetSerialNumber = 30856,

	GetSettingsMask = 54413,

	GetStatisticsRun = 52866,

	[CommandDataSize(54)]
	GetStatisticsSleep = 52868,

	GetStatisticsWorkout = 52867,

	GetStatisticsWorkoutGuided = 52869,

	GetTile = 54407,

	GetTiles = 54400,

	GetTilesDefaults = 54404,

	GetTilesNoImages = 54418,

	GetUniqueId = 30337,

	Notification = 52224,

	OobeFinalise = 44290,

	OobeGetComplete = 51859,

	OobeSetComplete = 51713,

	OobeGetStage = 44417,

	OobeSetStage = 44288,

	ProfileGetDataApp = 50566,

	ProfileGetDataFw = 50568,

	ProfileSetDataApp = 50439,

	ProfileSetDataFw = 50441,

	SetDeviceTime = 29953,

	SetMeTileImage = 49937,

	SetSettingsMask = 54286,

	SetThemeColor = 55296,

	SetTile = 54278,

	SetTiles = 54273,

	StartStripSyncEnd = 54275,

	StartStripSyncStart = 54274,

	Subscribe = 36608,

	SubscriptionGetData = 36739,

	SubscriptionGetDataLength = 36738,

	SubscriptionSubscribeId = 36615,

	SubscriptionUnsubscribeId = 36616,

	TilesDisableSetting = 54288,

	TilesEnableSetting = 54287,

	UINavigateScreen = 49920,

	Unsubscribe = 36609

}

}