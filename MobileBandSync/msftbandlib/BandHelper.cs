using MobileBandSync.MSFTBandLib.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MobileBandSync.MSFTBandLib
{
    internal enum TX : byte
    {
        False,
        True
    }

    internal static class CancellationTokenExtension
    {
        public static void WaitAndThrowIfCancellationRequested( this CancellationToken token, TimeSpan timeout )
        {
            token.WaitHandle.WaitOne( timeout );
            token.ThrowIfCancellationRequested();
        }

        public static void WaitAndThrowIfCancellationRequested( this CancellationToken token, int timeout )
        {
            token.WaitHandle.WaitOne( timeout );
            token.ThrowIfCancellationRequested();
        }
    }

    internal static class BandBitConverter
    {

        public static void GetBytes( short i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 2 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 16; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255 );
            }
        }


        public static void GetBytes( ushort i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 2 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 16; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255 );
            }
        }


        public static void GetBytes( int i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 4 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 32; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255 );
            }
        }


        public static void GetBytes( uint i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 4 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 32; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255U );
            }
        }


        public static void GetBytes( long i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 8 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 64; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255L );
            }
        }


        public static void GetBytes( ulong i, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 8 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            for( int j = 0; j < 64; j += 8 )
            {
                buffer[offset++] = (byte)( i >> j & 255UL );
            }
        }


        public static Guid ToGuid( byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 16 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            if( buffer.Length == 16 )
            {
                return new Guid( buffer );
            }
            return new Guid( BitConverter.ToInt32( buffer, offset ), BitConverter.ToInt16( buffer, offset + 4 ), BitConverter.ToInt16( buffer, offset + 6 ), buffer[offset + 8], buffer[offset + 9], buffer[offset + 10], buffer[offset + 11], buffer[offset + 12], buffer[offset + 13], buffer[offset + 14], buffer[offset + 15] );
        }


        public static void GetBytes( Guid guid, byte[] buffer, int offset )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || buffer.Length < offset + 16 )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            guid.ToByteArray().CopyTo( buffer, offset );
        }


        public static string ToString( byte[] buffer )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            return BandBitConverter.ToStringInternal( buffer, 0, buffer.Length );
        }


        public static string ToString( byte[] buffer, int offset, int count )
        {
            if( buffer == null )
            {
                throw new ArgumentNullException( "buffer" );
            }
            if( offset < 0 || ( buffer.Length != 0 && offset >= buffer.Length ) )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            if( count < 0 || offset + count > buffer.Length )
            {
                throw new ArgumentOutOfRangeException( "offset" );
            }
            return BandBitConverter.ToStringInternal( buffer, offset, count );
        }


        private static string ToStringInternal( byte[] buffer, int offset, int count )
        {
            StringBuilder stringBuilder = new StringBuilder( count * 2 );
            while( count > 0 )
            {
                byte b = buffer[offset];
                stringBuilder.Append( BandBitConverter.HexCharTable[b >> 4 & 15] );
                stringBuilder.Append( BandBitConverter.HexCharTable[(int)( b & 15 )] );
                count--;
                offset++;
            }
            return stringBuilder.ToString();
        }


        private static readonly char[] HexCharTable = "0123456789ABCDEF".ToCharArray();
    }


    internal static class DeviceCommands
    {

        internal static ushort MakeCommand( FacilityEnum category, TX isTXCommand, byte index )
        {
            ushort usCategory = (ushort)( (ushort)category << 8 );
            ushort usisTXCommand = (ushort)( (ushort)isTXCommand << 7 );
            return (ushort)( usCategory | usisTXCommand | (ushort)index );
        }


        internal static void LookupCommand( ushort commandId, out FacilityEnum category, out TX isTXCommand, out byte index )
        {
            category = (FacilityEnum)( ( commandId & 65280 ) >> 8 );
            isTXCommand = (TX)( ( commandId & 128 ) >> 7 );
            index = (byte)( commandId & 127 );
        }


        internal const ushort IndexShift = 0;


        internal const ushort IndexBits = 7;


        internal const ushort IndexMask = 127;


        internal const ushort TXShift = 7;


        internal const ushort TXBits = 1;


        internal const ushort TXMask = 128;


        internal const ushort CategoryShift = 8;


        internal const ushort CategoryBits = 8;


        internal const ushort CategoryMask = 65280;


        internal static ushort CargoCoreModuleGetVersion = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.True, 1 );


        internal static ushort CargoCoreModuleGetUniqueID = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.True, 2 );


        internal static ushort CargoCoreModuleWhoAmI = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.True, 3 );


        internal static ushort CargoCoreModuleGetLogVersion = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.True, 5 );


        internal static ushort CargoCoreModuleGetApiVersion = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.True, 6 );


        internal static ushort CargoCoreModuleSdkCheck = DeviceCommands.MakeCommand( FacilityEnum.LibraryJutil, TX.False, 7 );


        internal static ushort CargoTimeGetUtcTime = DeviceCommands.MakeCommand( FacilityEnum.LibraryTime, TX.True, 0 );


        internal static ushort CargoTimeSetUtcTime = DeviceCommands.MakeCommand( FacilityEnum.LibraryTime, TX.False, 1 );


        internal static ushort CargoTimeGetLocalTime = DeviceCommands.MakeCommand( FacilityEnum.LibraryTime, TX.True, 2 );


        internal static ushort CargoTimeSetTimeZoneFile = DeviceCommands.MakeCommand( FacilityEnum.LibraryTime, TX.False, 4 );


        internal static ushort CargoTimeZoneFileGetVersion = DeviceCommands.MakeCommand( FacilityEnum.LibraryTime, TX.True, 6 );


        internal static ushort CargoLoggerGetChunkData = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.True, 1 );


        internal static ushort CargoLoggerEnableLogging = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.False, 3 );


        internal static ushort CargoLoggerDisableLogging = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.False, 4 );


        internal static ushort CargoLoggerGetChunkCounts = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.True, 9 );


        internal static ushort CargoLoggerFlush = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.False, 13 );


        internal static ushort CargoLoggerGetChunkRangeMetadata = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.True, 14 );


        internal static ushort CargoLoggerGetChunkRangeData = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.True, 15 );


        internal static ushort CargoLoggerDeleteChunkRange = DeviceCommands.MakeCommand( FacilityEnum.LibraryLogger, TX.False, 16 );


        internal static ushort CargoProfileGetDataApp = DeviceCommands.MakeCommand( FacilityEnum.ModuleProfile, TX.True, 6 );


        internal static ushort CargoProfileSetDataApp = DeviceCommands.MakeCommand( FacilityEnum.ModuleProfile, TX.False, 7 );


        internal static ushort CargoProfileGetDataFW = DeviceCommands.MakeCommand( FacilityEnum.ModuleProfile, TX.True, 8 );


        internal static ushort CargoProfileSetDataFW = DeviceCommands.MakeCommand( FacilityEnum.ModuleProfile, TX.False, 9 );


        internal static ushort CargoRemoteSubscriptionSubscribe = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.False, 0 );


        internal static ushort CargoRemoteSubscriptionUnsubscribe = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.False, 1 );


        internal static ushort CargoRemoteSubscriptionGetDataLength = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.True, 2 );


        internal static ushort CargoRemoteSubscriptionGetData = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.True, 3 );


        internal static ushort CargoRemoteSubscriptionSubscribeId = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.False, 7 );


        internal static ushort CargoRemoteSubscriptionUnsubscribeId = DeviceCommands.MakeCommand( FacilityEnum.LibraryRemoteSubscription, TX.False, 8 );


        internal static ushort CargoNotification = DeviceCommands.MakeCommand( FacilityEnum.ModuleNotification, TX.False, 0 );


        internal static ushort CargoNotificationProtoBuf = DeviceCommands.MakeCommand( FacilityEnum.ModuleNotification, TX.False, 5 );


        internal static ushort CargoDynamicAppRegisterApp = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 0 );


        internal static ushort CargoDynamicAppRemoveApp = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 1 );


        internal static ushort CargoDynamicAppRegisterAppIcons = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 2 );


        internal static ushort CargoDynamicAppSetAppTileIndex = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 3 );


        internal static ushort CargoDynamicAppSetAppBadgeTileIndex = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 5 );


        internal static ushort CargoDynamicAppSetAppNotificationTileIndex = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballAppsManagement, TX.False, 11 );


        internal static ushort CargoDynamicPageLayoutSet = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballPageManagement, TX.False, 0 );


        internal static ushort CargoDynamicPageLayoutRemove = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballPageManagement, TX.False, 1 );


        internal static ushort CargoDynamicPageLayoutGet = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballPageManagement, TX.True, 2 );


        internal static ushort CargoInstalledAppListGet = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 0 );


        internal static ushort CargoInstalledAppListSet = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 1 );


        internal static ushort CargoInstalledAppListStartStripSyncStart = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 2 );


        internal static ushort CargoInstalledAppListStartStripSyncEnd = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 3 );


        internal static ushort CargoInstalledAppListGetDefaults = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 4 );


        internal static ushort CargoInstalledAppListSetTile = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 6 );


        internal static ushort CargoInstalledAppListGetTile = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 7 );


        internal static ushort CargoInstalledAppListGetSettingsMask = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 13 );


        internal static ushort CargoInstalledAppListSetSettingsMask = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 14 );


        internal static ushort CargoInstalledAppListEnableSetting = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 15 );


        internal static ushort CargoInstalledAppListDisableSetting = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.False, 16 );


        internal static ushort CargoInstalledAppListGetNoImages = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 18 );


        internal static ushort CargoInstalledAppListGetDefaultsNoImages = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 19 );


        internal static ushort CargoInstalledAppListGetMaxTileCount = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 21 );


        internal static ushort CargoInstalledAppListGetMaxTileAllocatedCount = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstalledAppList, TX.True, 22 );


        internal static ushort CargoSystemSettingsOobeCompleteClear = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 0 );


        internal static ushort CargoSystemSettingsOobeCompleteSet = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 1 );


        internal static ushort CargoSystemSettingsFactoryReset = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.True, 7 );


        internal static ushort CargoSystemSettingsGetTimeZone = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.True, 10 );


        internal static ushort CargoSystemSettingsSetTimeZone = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 11 );


        internal static ushort CargoSystemSettingsSetEphemerisFile = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 15 );


        internal static ushort CargoSystemSettingsGetMeTileImageID = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.True, 18 );


        internal static ushort CargoSystemSettingsOobeCompleteGet = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.True, 19 );


        internal static ushort CargoSystemSettingsEnableDemoMode = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 25 );


        internal static ushort CargoSystemSettingsDisableDemoMode = DeviceCommands.MakeCommand( FacilityEnum.ModuleSystemSettings, TX.False, 26 );


        internal static ushort CargoSRAMFWUpdateLoadData = DeviceCommands.MakeCommand( FacilityEnum.LibrarySRAMFWUpdate, TX.False, 0 );


        internal static ushort CargoSRAMFWUpdateBootIntoUpdateMode = DeviceCommands.MakeCommand( FacilityEnum.LibrarySRAMFWUpdate, TX.False, 1 );


        internal static ushort CargoSRAMFWUpdateValidateAssets = DeviceCommands.MakeCommand( FacilityEnum.LibrarySRAMFWUpdate, TX.True, 2 );


        internal static ushort CargoEFlashRead = DeviceCommands.MakeCommand( FacilityEnum.DriverEFlash, TX.True, 1 );


        internal static ushort CargoGpsIsEnabled = DeviceCommands.MakeCommand( FacilityEnum.LibraryGps, TX.True, 6 );


        internal static ushort CargoGpsEphemerisCoverageDates = DeviceCommands.MakeCommand( FacilityEnum.LibraryGps, TX.True, 13 );


        internal static ushort CargoFireballUINavigateToScreen = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.False, 0 );


        internal static ushort CargoFireballUIClearMeTileImage = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.False, 6 );


        internal static ushort CargoFireballUISetSmsResponse = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.False, 7 );


        internal static ushort CargoFireballUIGetAllSmsResponse = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.True, 11 );


        internal static ushort CargoFireballUIReadMeTileImage = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.True, 14 );


        internal static ushort CargoFireballUIWriteMeTileImageWithID = DeviceCommands.MakeCommand( FacilityEnum.ModuleFireballUI, TX.False, 17 );


        internal static ushort CargoThemeColorSetFirstPartyTheme = DeviceCommands.MakeCommand( FacilityEnum.ModuleThemeColor, TX.False, 0 );


        internal static ushort CargoThemeColorGetFirstPartyTheme = DeviceCommands.MakeCommand( FacilityEnum.ModuleThemeColor, TX.True, 1 );


        internal static ushort CargoThemeColorSetCustomTheme = DeviceCommands.MakeCommand( FacilityEnum.ModuleThemeColor, TX.False, 2 );


        internal static ushort CargoThemeColorReset = DeviceCommands.MakeCommand( FacilityEnum.ModuleThemeColor, TX.False, 4 );


        internal static ushort CargoHapticPlayVibrationStream = DeviceCommands.MakeCommand( FacilityEnum.LibraryHaptic, TX.False, 0 );


        internal static ushort CargoGoalTrackerSet = DeviceCommands.MakeCommand( FacilityEnum.ModuleGoalTracker, TX.False, 0 );


        internal static ushort CargoFitnessPlansWriteFile = DeviceCommands.MakeCommand( FacilityEnum.LibraryFitnessPlans, TX.False, 4 );


        internal static ushort CargoGolfCourseFileWrite = DeviceCommands.MakeCommand( FacilityEnum.LibraryGolf, TX.False, 0 );


        internal static ushort CargoGolfCourseFileGetMaxSize = DeviceCommands.MakeCommand( FacilityEnum.LibraryGolf, TX.True, 1 );


        internal static ushort CargoOobeSetStage = DeviceCommands.MakeCommand( FacilityEnum.ModuleOobe, TX.False, 0 );


        internal static ushort CargoOobeGetStage = DeviceCommands.MakeCommand( FacilityEnum.ModuleOobe, TX.True, 1 );


        internal static ushort CargoOobeFinalize = DeviceCommands.MakeCommand( FacilityEnum.ModuleOobe, TX.False, 2 );


        internal static ushort CargoCortanaNotification = DeviceCommands.MakeCommand( FacilityEnum.ModuleCortana, TX.False, 0 );


        internal static ushort CargoCortanaStart = DeviceCommands.MakeCommand( FacilityEnum.ModuleCortana, TX.False, 1 );


        internal static ushort CargoCortanaStop = DeviceCommands.MakeCommand( FacilityEnum.ModuleCortana, TX.False, 2 );


        internal static ushort CargoCortanaCancel = DeviceCommands.MakeCommand( FacilityEnum.ModuleCortana, TX.False, 3 );


        internal static ushort CargoPersistedAppDataSetRunMetrics = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 0 );


        internal static ushort CargoPersistedAppDataGetRunMetrics = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 1 );


        internal static ushort CargoPersistedAppDataSetBikeMetrics = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 2 );


        internal static ushort CargoPersistedAppDataGetBikeMetrics = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 3 );


        internal static ushort CargoPersistedAppDataSetBikeSplitMult = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 4 );


        internal static ushort CargoPersistedAppDataGetBikeSplitMult = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 5 );


        internal static ushort CargoPersistedAppDataSetWorkoutActivities = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 9 );


        internal static ushort CargoPersistedAppDataGetWorkoutActivities = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 16 );


        internal static ushort CargoPersistedAppDataSetSleepNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 17 );


        internal static ushort CargoPersistedAppDataGetSleepNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 18 );


        internal static ushort CargoPersistedAppDataDisableSleepNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 19 );


        internal static ushort CargoPersistedAppDataSetLightExposureNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 21 );


        internal static ushort CargoPersistedAppDataGetLightExposureNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.True, 22 );


        internal static ushort CargoPersistedAppDataDisableLightExposureNotification = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedApplicationData, TX.False, 23 );


        internal static ushort CargoGetProductSerialNumber = DeviceCommands.MakeCommand( FacilityEnum.LibraryConfiguration, TX.True, 8 );


        internal static ushort CargoKeyboardCmd = DeviceCommands.MakeCommand( FacilityEnum.LibraryKeyboard, TX.False, 0 );


        internal static ushort CargoSubscriptionLoggerSubscribe = DeviceCommands.MakeCommand( FacilityEnum.ModuleLoggerSubscriptions, TX.False, 0 );


        internal static ushort CargoSubscriptionLoggerUnsubscribe = DeviceCommands.MakeCommand( FacilityEnum.ModuleLoggerSubscriptions, TX.False, 1 );


        internal static ushort CargoCrashDumpGetFileSize = DeviceCommands.MakeCommand( FacilityEnum.DriverCrashDump, TX.True, 1 );


        internal static ushort CargoCrashDumpGetAndDeleteFile = DeviceCommands.MakeCommand( FacilityEnum.DriverCrashDump, TX.True, 2 );


        internal static ushort CargoInstrumentationGetFileSize = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstrumentation, TX.True, 4 );


        internal static ushort CargoInstrumentationGetFile = DeviceCommands.MakeCommand( FacilityEnum.ModuleInstrumentation, TX.True, 5 );


        internal static ushort CargoPersistedStatisticsRunGet = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedStatistics, TX.True, 2 );


        internal static ushort CargoPersistedStatisticsWorkoutGet = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedStatistics, TX.True, 3 );


        internal static ushort CargoPersistedStatisticsSleepGet = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedStatistics, TX.True, 4 );


        internal static ushort CargoPersistedStatisticsGuidedWorkoutGet = DeviceCommands.MakeCommand( FacilityEnum.ModulePersistedStatistics, TX.True, 5 );
    }

}
