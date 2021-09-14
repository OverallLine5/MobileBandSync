using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Facility;
using MobileBandSync.MSFTBandLib.Includes;
using MobileBandSync.MSFTBandLib.Libs;
using System;
using System.IO;
using System.Reflection;

namespace MobileBandSync.MSFTBandLib.Command
{

    /// <summary>Command helper methods</summary>
    public static class CommandHelper
    {

        /// <summary>
        /// Create a new command given a Band facility.
        /// 
        /// You must specify the facility index and whether it is a TX bit.
        /// 
        /// This method uses bitwise operations to convert the facility/index 
        /// integers into a `ushort` which is returned as the command.
        /// 
        /// Reminders:
        ///  - `<<` shifts the left operand's value left by 
        /// 	the number of bits specified by the right operand.
        ///  - `|` copies a bit if it exists in either of its operands.
        ///  
        /// Returns a `ushort` for use as the command.
        /// </summary>
        /// <param name="facility">Facility</param>
        /// <param name="tx">TX bit</param>
        /// <param name="index">Index</param>
        /// <returns>ushort</returns>
        public static ushort Create( FacilityEnum facility, bool tx, int index )
        {
            return (ushort)( (int)facility << 8 | ( tx ? 1 : 0 ) << 7 | index );
        }


        /// <summary>Get the data size associated with a command.</summary>
        /// <param name="command">Command</param>
        /// <returns>int</returns>
        public static int GetCommandDataSize( CommandEnum command )
        {
            FieldInfo fi = typeof( CommandEnum ).GetRuntimeField( command.ToString() );
            Attribute attr = fi.GetCustomAttribute( typeof( CommandDataSize ) );

            return attr != null ? ( (CommandDataSize)attr ).DataSize : 8192;
        }


        public static int GetCommandDataSize( Func<uint> BufferSize )
        {
            uint size = BufferSize != null ? BufferSize() : 8192;
            return (int) size;
        }


        /// <summary>
        /// Get an array of bytes to use as the default arguments for a 
        /// command when no arguments are given; we have to specify the 
        /// expected data/response size as the arguments instead.
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>byte[]</returns>
        public static byte[] GetCommandDefaultArgumentsBytes( CommandEnum command )
        {
            ByteStream bytes = new ByteStream();
            bytes.BinaryWriter.Write( CommandHelper.GetCommandDataSize( command ) );
            return bytes.GetBytes();
        }
        public static byte[] GetCommandDefaultArgumentsBytes( Func<uint> BufferSize )
        {
            byte[] btResult = null;

            if( BufferSize != null )
            {
                ByteStream bytes = new ByteStream();
                bytes.BinaryWriter.Write( BufferSize() );
                btResult = bytes.GetBytes();
            }
            return btResult;
        }
    }

}