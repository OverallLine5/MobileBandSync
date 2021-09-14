using MobileBandSync.MSFTBandLib;
using MobileBandSync.MSFTBandLib.Libs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using System.Linq;

namespace MobileBandSync.MSFTBandLib.UWP {

    /// <summary>
    /// MobileBandSync.MSFTBandLib UWP implementation
    /// </summary>
    public class BandClientUWP : BandClientInterface
    {

        /// <summary>
        ///	Get list of all available paired Bands.
        /// </summary>
        /// <returns>Task<List<Band>></returns>
        public async Task<List<BandInterface>> GetPairedBands()
        {
            string selector;
            RfcommServiceId cargo;
            DeviceInformationCollection devices;
            List<BandInterface> bands = new List<BandInterface>();

            // Get devices
            cargo = RfcommServiceId.FromUuid( Guid.Parse( Services.CARGO ) );
            selector = RfcommDeviceService.GetDeviceSelector( cargo );
            devices = await DeviceInformation.FindAllAsync( selector );


            // Create Band instances
            foreach( DeviceInformation device in devices )
            {
                BluetoothDevice bt;
                bt = await BluetoothDevice.FromIdAsync( device.Id );

                if( bt != null )
                {
                    bands.Add( new Band<BandSocketUWP>( bt ) );
                }
            }
            return bands;
        }

        public async Task<List<BandInterface>> FakePairedBands()
        {
            string selector;
            RfcommServiceId cargo;
            DeviceInformationCollection devices;
            List<BandInterface> bands = new List<BandInterface>();

            // Get devices
            cargo = RfcommServiceId.FromUuid( Guid.Parse( Services.CARGO ) );
            selector = RfcommDeviceService.GetDeviceSelector( cargo );

            // Create Band instances
            bands.Add( new Band<BandSocketUWP>( "BluetoothHost", "BluetoothName" ) );

            return bands;
        }
    }

}