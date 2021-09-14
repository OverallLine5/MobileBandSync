namespace MobileBandSync.MSFTBandLib.Facility {

/// <summary>Facility identifiers</summary>
public enum FacilityEnum : int {

    Invalid = 0,

    ReservedBase = 1,

    ReservedEnd = 31,

    DriverDma = 32,

    DriverBtle = 33,

    DriverPdb = 34,

    DriverI2c = 35,

    DriverAdc = 36,

    DriverGpio = 37,

    DriverDac = 38,

    DriverAnalogMgr = 39,

    DriverRtc = 40,

    DriverMotor = 41,

    DriverDisplay = 43,

    DriverUartAsync = 44,

    DriverPmu = 45,

    DriverExternalRam = 46,

    DriverAls = 47,

    DriverTimers = 48,

    DriverFlexBus = 49,

    DriverSpi = 50,

    DriverEFlash = 51,

    DriverPwm = 52,

    DriverCrc = 53,

    DriverPFlash = 54,

    DriverFpu = 55,

    DriverCoreModule = 56,

    DriverCrashDump = 57,

    DriverUsb = 58,

    DriverMmcau = 59,

    DriversEnd = 111,

    LibraryDebug = 112,

    LibraryRuntime = 113,

    LibraryUsbCmdProtocol = 114,

    LibraryBTPS = 115,

    LibraryTouch = 116,

    LibraryTime = 117,

    LibraryJutil = 118,

    LibraryHRManager = 119,

    LibraryConfiguration = 120,

    LibraryButton = 121,

    LibraryBacklight = 122,

    LibraryMotion = 123,

    LibraryActMon = 124,

    LibraryBattery = 125,

    LibraryGps = 126,

    LibraryHRLed = 127,

    LibraryDfu = 128,

    LibraryHeartRate = 129,

    LibraryMicrophone = 131,

    LibraryGsr = 132,

    LibraryUV = 133,

    LibrarySkinTemp = 134,

    LibraryAmbTemp = 135,

    LibraryPedometer = 136,

    LibraryCalories = 137,

    LibraryDistance = 138,

    LibraryAlgoMath = 139,

    LibraryLogger = 140,

    LibraryPeg = 141,

    LibraryFile = 142,

    LibraryRemoteSubscription = 143,

    LibraryPower = 144,

    LibraryUVExposure = 145,

    LibraryMinuteTimer = 146,

    LibraryRecovery = 147,

    LibrarySubscriptionBase = 148,

    LibraryDateChangeSubscription = 149,

    LibraryHREstimator = 150,

    LibraryUSBConnection = 151,

    LibrarySRAMFWUpdate = 152,

    LibraryAutoBrightness = 153,

    LibraryHaptic = 154,

    LibraryFitnessPlans = 155,

    LibrarySleepRecovery = 156,

    LibraryFirstBeat = 157,

    LibraryAncsNotificationCache = 158,

    LibraryKeyboard = 159,

    LibraryHrAccelSync = 160,

    LibraryGolf = 161,

    ModuleOobe = 173,

    LibrariesEnd = 191,

    ModuleMain = 192,

    ModuleBehavior = 193,

    ModuleFireballTransportLayer = 194,

    ModuleFireballUI = 195,

    ModuleFireballUtilities = 196,

    ModuleProfile = 197,

    ModuleLoggerSubscriptions = 198,

    ModuleFireballTilesModels = 199,

    ModulePowerManager = 200,

    ModuleHrPowerManager = 201,

    ModuleSystemSettings = 202,

    ModuleFireballHardwareManager = 203,

    ModuleNotification = 204,

    ModuleFtlTouchManager = 205,

    ModulePersistedStatistics = 206,

    ModuleAlgorithms = 207,

    ModulePersistedApplicationData = 208,

    ModuleDeviceContact = 209,

    ModuleInstrumentation = 210,

    ModuleFireballAppsManagement = 211,

    ModuleInstalledAppList = 212,

    ModuleFireballPageManagement = 213,

    ModuleUnitTests = 214,

    ModuleBatteryGauge = 215,

    ModuleThemeColor = 216,

    ModuleGoalTracker = 217,

    ModuleKfrost = 218,

    ModulePal = 219,

    ModuleGestures = 220,

    ModuleCortana = 221,

    ModuleVoicePush = 222,

    ModulesEnd = 223,

    ApplicationMain = 224,

    Application1BL = 225,

    Application2UP = 226,

    ApplicationsEnd = 239,

    Reserved2Base = 240,

    Reserved2End = 255,

}

}