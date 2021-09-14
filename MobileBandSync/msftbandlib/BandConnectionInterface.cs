using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Includes;
using MobileBandSync.MSFTBandLib.Libs;
using System;
using System.Threading.Tasks;

namespace MobileBandSync.MSFTBandLib {

/// <summary>
/// Microsoft Band connection interface
/// </summary>
public interface BandConnectionInterface : IDisposable {

    /// <summary>
    /// Create a new connection to a given Band.
    /// </summary>
    /// <param name="Band">Band to connect to</param>
    /// <returns>Task</returns>
    Task Connect(BandInterface Band);

    /// <summary>
    /// Disconnect from the Band if connected.
    /// </summary>
    Task Disconnect();

        /// <summary>
        /// Send command to the device and get response bytes.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="args">Arguments to send</param>
        /// <param name="buffer">Receiving buffer size</param>
        /// <returns>Task<CommandResponse></returns>
        Task<CommandResponse> Command(
            CommandEnum command, Func<uint> BufferSize,
            byte[] args = null, uint buffer = Network.BUFFER_SIZE,
            Action<UInt64, UInt64> Progress = null
        );
        Task CommandStore(
            CommandEnum command, Func<uint> BufferSize,
            byte[] args = null, uint buffer = Network.BUFFER_SIZE,
            Action<UInt64, UInt64> Progress = null
        );

    }

}