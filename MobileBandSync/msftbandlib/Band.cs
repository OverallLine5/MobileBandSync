using MobileBandSync.MSFTBandLib;
using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Exceptions;
using MobileBandSync.MSFTBandLib.Includes;
using MobileBandSync.MSFTBandLib.Helpers;
using MobileBandSync.MSFTBandLib.Metrics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Linq;
using Windows.Storage.Streams;
using MobileBandSync.MSFTBandLib.UWP;
using System.Globalization;
using System.Threading;
using Windows.UI.Core;
using MobileBandSync.Data;
using Microsoft.Data.Sqlite;
using Windows.Devices.Bluetooth;

namespace MobileBandSync.MSFTBandLib
{
    public class BandMetadataRange
    {
        private BandMetadataRange()
        {
        }

        public uint StartingSeqNumber { get; set; }
        public uint EndingSeqNumber { get; set; }
        public uint ByteCount { get; set; }

        public static int GetSerializedByteCount()
        {
            return 12;
        }

        public static BandMetadataRange DeserializeFromBytes( byte[] data )
        {
            return new BandMetadataRange
            {
                StartingSeqNumber = BitConverter.ToUInt32( data, 0 ),
                EndingSeqNumber = BitConverter.ToUInt32( data, 4 ),
                ByteCount = BitConverter.ToUInt32( data, 8 )
            };
        }


        internal void SerializeToBytes( ref byte[] data )
        {
            var stream = new MemoryStream( GetSerializedByteCount() );
            var writer = new BinaryWriter( stream );

            writer.Write( StartingSeqNumber );
            writer.Write( EndingSeqNumber );
            writer.Write( ByteCount );

            data = stream.ToArray();
        }


        internal byte[] SerializeToByteArray()
        {
            int num = 0;
            byte[] array = new byte[BandMetadataRange.GetSerializedByteCount()];
            BandBitConverter.GetBytes( this.StartingSeqNumber, array, num );
            num += 4;
            BandBitConverter.GetBytes( this.EndingSeqNumber, array, num );
            num += 4;
            BandBitConverter.GetBytes( this.ByteCount, array, num );
            return array;
        }

        private const int serializedByteCount = 12;
    }


    public class BandStatus
    {
        private BandStatus()
        {
        }

        public ushort PacketType { get; private set; }
        public uint Status { get; private set; }

        public static int GetSerializedByteCount()
        {
            return 12;
        }

        public static BandStatus DeserializeFromBytes( byte[] data )
        {
            return new BandStatus
            {
                PacketType = BitConverter.ToUInt16( data, 0 ),
                Status = BitConverter.ToUInt32( data, 2 ),
            };
        }


        internal void SerializeToBytes( ref byte[] data )
        {
            var stream = new MemoryStream( GetSerializedByteCount() );
            var writer = new BinaryWriter( stream );

            writer.Write( PacketType );
            writer.Write( Status );

            data = stream.ToArray();
        }


        internal byte[] SerializeToByteArray()
        {
            int num = 0;
            byte[] array = new byte[BandMetadataRange.GetSerializedByteCount()];
            BandBitConverter.GetBytes( this.PacketType, array, num );
            num += 2;
            BandBitConverter.GetBytes( this.Status, array, num );
            return array;
        }

        private const int serializedByteCount = 6;
    }


    /// <summary>
    /// Microsoft Band device class
    /// 
    /// Most methods will throw `BandConnectedNot` from the `Command` 
    /// method when trying to access Bluetooth endpoints when not connected.
    /// </summary>
    public class Band<T> : BandInterface
    where T : class, BandSocketInterface
    {
        /// <summary>MAC address</summary>
        public string Mac { get; protected set; }

        ///	<summary>Bluetooth name</summary>
        public string Name { get; protected set; }

        internal object protocolLock;
        internal BluetoothDevice _device = null;

        /// <summary>Get currently connected</summary>
        public bool Connected
        {
            get { return this.Connection.Connected; }
            set { throw new Exception( "Can't change connection directly!" ); }
        }   /// <summary>Band Bluetooth connection</summary>

        public BandConnection<T> Connection { get; protected set; }
        public BluetoothDevice GetDevice() { return _device; }


        /// <summary>Construct a new device instance.</summary>
        /// <param name="mac">MAC address</param>
        /// <param name="name">Bluetooth name</param>
        //--------------------------------------------------------------------------------------------------------------------
        public Band( string mac, string name )
        //--------------------------------------------------------------------------------------------------------------------
        {
            this.Mac = mac;
            this.Name = name;
            this.Connection = new BandConnection<T>( this );
            this._device = null;
        }


        /// <summary>Construct a new device instance.</summary>
        //--------------------------------------------------------------------------------------------------------------------
        public Band( BluetoothDevice device )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( device != null )
            {
                this.Mac = device.HostName.ToString();
                this.Name = device.Name;
                this.Connection = new BandConnection<T>( this );
                this._device = device;
            }
        }


        /// <summary>Get MAC address.</summary>
        /// <returns>string</returns>
        //--------------------------------------------------------------------------------------------------------------------
        public string GetMac()
        //--------------------------------------------------------------------------------------------------------------------
        {
            return this.Mac;
        }


        /// <summary>Get Bluetooth name.</summary>
        /// <returns>string</returns>
        //--------------------------------------------------------------------------------------------------------------------
        public string GetName()
        //--------------------------------------------------------------------------------------------------------------------
        {
            return this.Name;
        }


        /// <summary>Connect to the Band.</summary>
        /// <returns>Task</returns>
        /// <exception cref="BandConnected">Band is connected.</exception>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task Connect( Action<UInt64, UInt64> Progress = null )
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( !this.Connected )
            {
                await this.Connection.Connect( Progress );
            }
            else throw new BandConnected();
        }


        /// <summary>Disconnect from the Band.</summary>
        /// <returns>Task</returns>
        /// <exception cref="BandConnectedNot">Band not connected.</exception>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task Disconnect()
        //--------------------------------------------------------------------------------------------------------------------
        {
            if( this.Connected )
            {
                await this.Connection.Disconnect();
            }
        }


        /// <summary>Run a command using the Band's `BandConnection`.</summary>
        /// <param name="Command">Command to run</param>
        /// <returns>Task<CommandResponse></returns>
        /// <exception cref="BandConnectedNot">Band not connected.</exception>
        public async Task<CommandResponse> Command( CommandEnum Command, Func<uint> BufferSize, Action<UInt64, UInt64> Progress = null )
        {
            if( !this.Connected ) throw new BandConnectedNot();
            else return await this.Connection.Command( Command, BufferSize, null, 8192, Progress );
        }

        public async Task<CommandResponse> Command( CommandEnum Command, Func<uint> BufferSize, byte[] btArgs, uint uiBufferSize = 8192, Action<UInt64, UInt64> Progress = null )
        {
            if( !this.Connected ) throw new BandConnectedNot();
            else return await this.Connection.Command( Command, BufferSize, btArgs, 8192, Progress );
        }


        public async Task CommandStore( CommandEnum Command, Func<uint> BufferSize, byte[] btArgs = null, uint uiBufferSize = 8192, Action<UInt64, UInt64> Progress = null )
        {
            if( !this.Connected ) throw new BandConnectedNot();
            else
                await this.Connection.CommandStore( Command, BufferSize, btArgs, uiBufferSize, Progress );
        }


        public async Task<CommandResponse> CommandStoreStatus( CommandEnum Command, Func<uint> BufferSize, byte[] btArgs = null, uint uiBufferSize = 8192, Action<UInt64, UInt64> Progress = null )
        {
            if( !this.Connected ) throw new BandConnectedNot();
            else
                return await this.Connection.CommandStoreStatus( Command, BufferSize, btArgs, uiBufferSize, Progress );
        }


        /// <summary>Get the current device time.</summary>
        /// <returns>Task<DateTime></returns>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task<DateTime> GetDeviceTime()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var res = await this.Command( CommandEnum.GetDeviceTime, null );
            return TimeHelper.DateTimeResponse( ( (CommandResponse)res ) );
        }


        /// <summary>Get last sleep.</summary>
        /// <returns>Task<Sleep></returns>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task<Sleep> GetLastSleep()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var res = await this.Command( CommandEnum.GetStatisticsSleep, null );
            return new Sleep( ( (CommandResponse)res ) );
        }


        /// <summary>Get serial number from the Band.</summary>
        /// <returns>Task<String></returns>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task<string> GetSerialNumber()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var res = await this.Command( (CommandEnum)DeviceCommands.CargoGetProductSerialNumber, null );
            return ( (CommandResponse)res ).GetByteStream().GetString();
        }


        //--------------------------------------------------------------------------------------------------------------------
        public static byte[] Combine( byte[] first, byte[] second )
        //--------------------------------------------------------------------------------------------------------------------
        {
            byte[] bytes = new byte[first.Length + second.Length];
            System.Buffer.BlockCopy( first, 0, bytes, 0, first.Length );
            System.Buffer.BlockCopy( second, 0, bytes, first.Length, second.Length );
            return bytes;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<byte[]> GetSensorLog( Action<string> Report,
                                                Action<UInt64, UInt64> Progress,
                                                bool bCleanupSensorLog,
                                                bool bStoreSensorLog )
        //--------------------------------------------------------------------------------------------------------------------
        {
            byte[] btSensorLog = null;

            Progress( 0, 0 );

            try
            {
                BandMetadataRange metaData = null;
                int iRemainingChunks = 0;

                await DeviceLogDataFlush();

                try
                {
                    iRemainingChunks = await RemainingDeviceLogDataChunks();
                }
                catch( Exception ex )
                {
                    iRemainingChunks = 0;
                }
                if( iRemainingChunks > 0 )
                {
                    int iChunksToFetch = iRemainingChunks; // Math.Min( 128, iRemainingChunks );

                    metaData = await GetChunkRangeMetadata( iChunksToFetch );
                    if( metaData == null )
                    {
                        Report( "* error: failed to get chunk range metadata!" );
                    }
                    else
                    {
                        // set 100% to twice the number of bytes so that storing the workouts is able to handle the progress too
                        try
                        {
                            if( Progress != null )
                            {
                                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync( CoreDispatcherPriority.Normal,
                                 () =>
                                 {
                                     Progress( 0, metaData.ByteCount * 2 );
                                     return;
                                 } );
                            }
                        }
                        catch( Exception )
                        {
                        }

                        btSensorLog = await GetChunkRangeData( metaData, Progress );

                        if( btSensorLog != null && btSensorLog.Length > 0 )
                        {
                            if( bStoreSensorLog )
                            {
                                var uploadId = DateTime.UtcNow.ToString( "yyyyMMddHHmmssfff" );
                                StorageFolder sensorLogFolder = await KnownFolders.DocumentsLibrary.CreateFolderAsync( "SensorLog", CreationCollisionOption.OpenIfExists );

                                if( sensorLogFolder == null )
                                    return null;

                                StorageFile sensorLogFile = await sensorLogFolder.CreateFileAsync( "band-" + uploadId + "-Data.bin", CreationCollisionOption.ReplaceExisting );
                                await FileIO.WriteBytesAsync( sensorLogFile, btSensorLog );
                            }
                            if( bCleanupSensorLog )
                            {
                                await DeleteChunkRange( metaData );
                            }
                        }
                        else
                        {
                            Report( "* error: failed to get chunk range data!" );
                        }
                    }
                }
            }
            catch( Exception ex )
            {
                Report( "Error downloading the logs: " + ex.Message );
                return null;
            }
            return btSensorLog;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task<int> RemainingDeviceLogDataChunks()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var res = await this.Command( (CommandEnum)DeviceCommands.CargoLoggerGetChunkCounts, null );
            byte[] btChunkCount = ( (CommandResponse)res ).GetByteStream().GetBytes();

            var result = BitConverter.ToInt32( btChunkCount, 0 );

            return (int)result;
        }


        //--------------------------------------------------------------------------------------------------------------------
        public async Task DeviceLogDataFlush()
        //--------------------------------------------------------------------------------------------------------------------
        {
            try
            {
                BandStatus status = null;
                Func<uint> BufferSize = () => 0;
                int iTries = 0;

                do
                {
                    var res = await this.Command( (CommandEnum)DeviceCommands.CargoLoggerFlush, BufferSize, null );
                    byte[] btStatus = ( (CommandResponse)res ).Status;
                    if( CommandResponse.ResponseBytesAreStatus( btStatus ) )
                    {
                        status = BandStatus.DeserializeFromBytes( btStatus );
                    }
                    CancellationToken.None.WaitAndThrowIfCancellationRequested( 1000 );
                }
                while( status == null || status.Status != 0 || iTries++ > 5 );
            }
            catch( Exception ex )
            {

            }
        }

        //--------------------------------------------------------------------------------------------------------------------
        public async Task<BandMetadataRange> GetChunkRangeMetadata( int chunkCount )
        //--------------------------------------------------------------------------------------------------------------------
        {
            BandMetadataRange metaResult = null;

            var stream = new MemoryStream( 12 );
            var writer = new BinaryWriter( stream );
            writer.Write( chunkCount );
            var btArgs = stream.ToArray();

            try
            {
                Func<uint> BufferSize = () => 12;
                var res = await this.Command( (CommandEnum)DeviceCommands.CargoLoggerGetChunkRangeMetadata, BufferSize, btArgs );
                byte[] btMetadata = ( (CommandResponse)res ).GetByteStream().GetBytes();

                metaResult = BandMetadataRange.DeserializeFromBytes( btMetadata );
            }
            catch( Exception ex )
            {

            }
            return metaResult;
        }

        //--------------------------------------------------------------------------------------------------------------------
        public async Task<byte[]> GetChunkRangeData( BandMetadataRange metaData, 
                                                     Action<UInt64, UInt64> Progress )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var stream = new MemoryStream( 12 );
            var writer = new BinaryWriter( stream );
            writer.Write( metaData.StartingSeqNumber );
            writer.Write( metaData.EndingSeqNumber );
            writer.Write( metaData.ByteCount );
            var btArgs = stream.ToArray();
            byte[] btResult = null;

            try
            {
                Func<uint> BufferSize = () => metaData.ByteCount;
                var res = await this.Command( (CommandEnum)DeviceCommands.CargoLoggerGetChunkRangeData, BufferSize, btArgs, 8192, Progress );

                btResult = ( (CommandResponse)res ).GetAllData();
            }
            catch( Exception ex )
            {

            }
            return btResult;
        }

        //--------------------------------------------------------------------------------------------------------------------
        public async Task<bool> DeleteChunkRange( BandMetadataRange metaData )
        //--------------------------------------------------------------------------------------------------------------------
        {
            var stream = new MemoryStream( 12 );
            var writer = new BinaryWriter( stream );
            writer.Write( metaData.StartingSeqNumber );
            writer.Write( metaData.EndingSeqNumber );
            writer.Write( metaData.ByteCount );
            var btArgs = stream.ToArray();

            try
            {
                Func<uint> BufferSize = () => 12;
                await this.CommandStore( (CommandEnum)DeviceCommands.CargoLoggerDeleteChunkRange, BufferSize, btArgs );
            }
            catch( Exception ex )
            {

            }
            return true;
        }

        /// <summary>Get serial number from the Band.</summary>
        /// <returns>Task<String></returns>
        //--------------------------------------------------------------------------------------------------------------------
        public async Task<BandVersion> GetVersion()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var res = await this.Command( (CommandEnum)DeviceCommands.CargoCoreModuleGetVersion, null );
            return new BandVersion( ( (CommandResponse)res ) );
        }

        //--------------------------------------------------------------------------------------------------------------------
        public DataReader GetDataReader()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var connection = Connection as BandConnection<BandSocketUWP>;
            return ( connection != null ? connection.Cargo.GetDataReader() : null );
        }

        //--------------------------------------------------------------------------------------------------------------------
        public DataWriter GetDataWriter()
        //--------------------------------------------------------------------------------------------------------------------
        {
            var connection = Connection as BandConnection<BandSocketUWP>;
            return ( connection != null ? connection.Cargo.GetDataWriter() : null );
        }
    }
}