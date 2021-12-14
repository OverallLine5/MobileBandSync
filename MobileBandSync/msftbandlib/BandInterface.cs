using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Metrics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;

namespace MobileBandSync.MSFTBandLib
{

    /// <summary>Band interface</summary>
    public interface BandInterface
    {

        /// <summary>Get MAC address.</summary>
        /// <returns>string</returns>
        string GetMac();

        /// <summary>Get Bluetooth name.</summary>
        /// <returns>string</returns>
        string GetName();

        /// <summary>Get Bluetooth name.</summary>
        /// <returns>string</returns>
        BluetoothDevice GetDevice();

        /// <summary>Connect to the Band.</summary>
        /// <returns>Task</returns>
        Task Connect( Action<UInt64, UInt64> Progress = null );

        /// <summary>Disconnect from the Band.</summary>
        /// <returns>Task</returns>
        Task Disconnect();

        /// <summary>Run a command.</summary>
        /// <param name="Command">Command to run</param>
        /// <returns>Task<CommandResponse></returns>
        Task<CommandResponse> Command( CommandEnum Command, Func<uint> BufferSize, Action<UInt64, UInt64> Progress = null );

        Task CommandStore( CommandEnum Command, Func<uint> BufferSize, byte[] btArgs, uint uiBufferSize = 8192, Action<UInt64, UInt64> Progress = null );

        /// <summary>Get the current device time.</summary>
        /// <returns>Task<DateTime></returns>
        Task<DateTime> GetDeviceTime();

        /// <summary>Get last sleep.</summary>
        /// <returns>Task<Sleep></returns>
        Task<Sleep> GetLastSleep();

        /// <summary>Get serial number from the Band.</summary>
        /// <returns>Task<string></returns>
        Task<string> GetSerialNumber();

        Task<BandVersion> GetVersion();

        Task<byte[]> GetSensorLog( Action<string> Report, Action<UInt64, UInt64> Progress, bool bCleanupSensorLog, bool bStoreSensorLog );

        Task<bool> DeleteChunkRange( BandMetadataRange metaData );

        DataReader GetDataReader();

        DataWriter GetDataWriter();
    }
}