using System;
using System.Threading.Tasks;
using MobileBandSync.MSFTBandLib.Command;
using Windows.Storage.Streams;

namespace MobileBandSync.MSFTBandLib
{
    //--------------------------------------------------------------------------------------------------------------------
    public class ProgressEventArgs : EventArgs
    //--------------------------------------------------------------------------------------------------------------------
    {
        public UInt64 Completed { get; private set; }
        public UInt64 Total { get; private set; }
        public string StatusText { get; private set; }

        public ProgressEventArgs( UInt64 uiCompleted, UInt64 uiTotal, string strStatusText )
            : base()
        {
            this.Completed = uiCompleted;
            this.Total = uiTotal;
            this.StatusText = strStatusText;
        }
    }


    /// <summary>
    /// Band socket interface
    /// </summary>
    public interface BandSocketInterface : IDisposable
    {
        /// <summary>Connect to a Band.</summary>
        /// <param name="mac">Band address</param>
        /// <param name="uuid">RFCOMM service UUID</param>
        /// <returns>Task</returns>
        Task Connect(string mac, Guid uuid, Action<UInt64, UInt64> Progress = null );

	    /// <summary>Close the connection.</summary>
	    /// <returns>Task</returns>
	    Task Disconnect();

	    /// <summary>Send a command packet to the device.</summary>
	    /// <param name="packet">Command packet</param>
	    /// <returns>Task</returns>
	    Task Send(CommandPacket packet);
        Task Send( CommandPacket packet, byte[] bytesToSend );
        Task<int> SendStatus( CommandPacket packet, byte[] bytesToSend );
    
        /// <summary>
        /// Receive bytes up to a specified buffer size.
        /// 
        /// Receives and adds bytes to a single `CommandResponse` 
        /// 	object until no more bytes are received or a 
        /// 	Band status has been received (indicating end of data).
        /// </summary>
        /// <param name="buffer">buffer</param>
        /// <returns>Task<CommandResponse></returns>
    Task<CommandResponse> Receive(uint buffer, Action<UInt64, UInt64> Progress = null );

	    /// <summary>
	    /// Send command packet to device and get a response.
	    /// 
	    /// Refer to `Send(...)` and `Receive(...)`.
	    /// </summary>
	    /// <param name="packet">Command packet</param>
	    /// <param name="buffer">Buffer size to receive from</param>
	    /// <returns>Task<CommandResponse></returns>
	Task<CommandResponse> Request(CommandPacket packet, uint buffer, Action<UInt64, UInt64> Progress = null );

        DataReader GetDataReader();

        DataWriter GetDataWriter();
    }
}