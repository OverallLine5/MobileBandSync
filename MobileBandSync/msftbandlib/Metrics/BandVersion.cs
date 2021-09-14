using MobileBandSync.MSFTBandLib.Command;
using MobileBandSync.MSFTBandLib.Includes;
using MobileBandSync.MSFTBandLib.Helpers;
using System;

namespace MobileBandSync.MSFTBandLib.Metrics
{

    /// <summary>Sleep metric</summary>
    public class BandVersion
    {
        public string AppName { get; protected set; }
        public byte PCBId { get; protected set; }
        public ushort VersionMajor { get; protected set; }
        public ushort VersionMinor { get; protected set; }
        public uint Revision { get; protected set; }
        public uint BuildNumber { get; protected set; }
        public byte DebugBuild { get; protected set; }


        public BandVersion() { }


        public BandVersion( CommandResponse response )
        {
            ByteStream bytes = response.GetByteStream();

            int num = 0;
            string strAppName = "";
            char cPart = (char)bytes.GetByte( num++ );

            while( cPart != 0 )
            {
                strAppName += cPart;
                cPart = (char)bytes.GetByte( num++ );
            }

            AppName = strAppName;
            PCBId = bytes.GetByte( num );
            VersionMajor = bytes.GetUshort( num + 1 );
            VersionMinor = bytes.GetUshort( num + 3 );
            Revision = bytes.GetUint32( num + 5 );
            BuildNumber = bytes.GetUint32( num + 9 );
            DebugBuild = bytes.GetByte( num + 13 );
        }

    }

}
