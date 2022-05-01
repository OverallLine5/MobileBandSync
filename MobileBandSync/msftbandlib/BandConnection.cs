using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Exceptions;
using MobileBandSync.MSFTBandLib.Libs;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

namespace MobileBandSync.MSFTBandLib
{

    /// <summary>
    /// Microsoft Band connection class
    /// </summary>
    public class BandConnection<T> : BandConnectionInterface, IDisposable
    where T : class, BandSocketInterface
    {

        /// <summary>Band instance</summary>
        protected BandInterface Band;

        /// <summary>Currently connected</summary>
        public bool Connected { get; protected set; }

        ///	<summary>Disposed</summary>
        public bool Disposed { get; protected set; }

        /// <summary>Band main service socket</summary>
        public readonly BandSocketInterface Cargo;

        /// <summary>Band push service socket</summary>
        public readonly BandSocketInterface Push;


        /// <summary>
        /// Create a new connection instance.
        /// 
        /// Socket instances are created for Cargo and Push using the 
        /// given socket type for the connection type, which must 
        /// implement `BandSocket`.
        /// </summary>
        /// <param name="Band">Band to connect to</param>
        public BandConnection()
        {
            this.Cargo = Activator.CreateInstance(
                typeof( T ), new object[] { }
            ) as T;
            this.Push = Activator.CreateInstance(
                typeof( T ), new object[] { }
            ) as T;
        }


        /// <summary>
        /// Create a new connection instance with a given Band.
        /// </summary>
        /// <param name="Band">Band to connect to</param>
        public BandConnection( BandInterface Band ) : this()
        {
            this.Band = Band;
        }


        /// <summary>Dispose of the connection.</summary>
        public void Dispose()
        {
            this.Dispose( true );
            GC.SuppressFinalize( this );
        }


        /// <summary>Dispose of the connection.</summary>
        /// <param name="disposing">Disposing (not used)</param>
        protected virtual void Dispose( bool disposing )
        {
            if( !this.Disposed )
            {
                this.Cargo.Dispose();
                this.Push.Dispose();
                this.Disposed = true;
            }
        }


        /// <summary>
        /// Connect to the currently active Band instance.
        /// 
        /// Throws if already connected.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="BandConnectionConnected"></exception>
        public async Task Connect( Action<UInt64, UInt64> Progress = null )
        {
            if( !this.Connected )
            {
                this.Connected = true;
                string mac = this.Band.GetMac();
                await this.Cargo.Connect( mac, Guid.Parse( Services.CARGO ), Progress );
                //await this.Push.Connect(mac, Guid.Parse(Services.PUSH));
            }
            else throw new BandConnectionConnected();
        }


        /// <summary>
        /// Connect to a given Band, replacing any existing Band instance.
        /// 
        /// Throws if already connected.
        /// </summary>
        /// <param name="Band">Band to connect to</param>
        /// <returns>Task</returns>
        /// <exception cref="BandConnectionConnected"></exception>
        public async Task Connect( BandInterface Band )
        {
            if( !this.Connected )
            {
                this.Band = Band;
                await this.Connect();
            }
            else throw new BandConnectionConnected();
        }


        /// <summary>
        /// Disconnect all open Band sockets.
        /// 
        /// Throws if not connected.
        /// </summary>
        /// <returns>Task</returns>
        /// <exception cref="BandConnectionConnectedNot"></exception>
        public async Task Disconnect()
        {
            if( this.Connected )
            {
                await this.Cargo.Disconnect();
                await this.Push.Disconnect();
                this.Connected = false;
            }
            else throw new BandConnectionConnectedNot();
        }


        /// <summary>
        /// Send command to the device and get a response.
        /// 
        /// Throws if not connected.
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="args">Arguments to send</param>
        /// <param name="buffer">Receiving buffer size</param>
        /// <returns>Task<CommandResponse></returns>
        /// <exception cref="BandConnectionConnectedNot"></exception>
        public async Task<CommandResponse> Command(
            CommandEnum command,
            Func<uint> BufferSize,
            byte[] args = null, uint buffer = Network.BUFFER_SIZE,
            Action<UInt64, UInt64> Progress = null )
        {

            if( !this.Connected )
                throw new BandConnectionConnectedNot();

            CommandPacket packet = new CommandPacket( command, BufferSize, args );
            return await this.Cargo.Request( packet, buffer, Progress );
        }

        public async Task CommandStore(
            CommandEnum command,
            Func<uint> BufferSize,
            byte[] args = null, uint buffer = Network.BUFFER_SIZE,
            Action<UInt64, UInt64> Progress = null )
        {

            if( !this.Connected )
                throw new BandConnectionConnectedNot();

            CommandPacket packet = new CommandPacket( command, BufferSize );
            await this.Cargo.Send( packet, args );
        }

        public async Task<int> CommandStoreStatus(
            CommandEnum command,
            Func<uint> BufferSize,
            byte[] args = null, uint buffer = Network.BUFFER_SIZE,
            Action<UInt64, UInt64> Progress = null )
        {

            if( !this.Connected )
                throw new BandConnectionConnectedNot();

            CommandPacket packet = new CommandPacket( command, BufferSize );
            return await this.Cargo.SendStatus( packet, args );
        }
    }

}