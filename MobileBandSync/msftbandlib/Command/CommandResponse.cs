using MobileBandSync.MSFTBandLib.Includes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileBandSync.MSFTBandLib.Command
{

    /// <summary>
    /// Command response class
    /// 
    /// Handles the parsing of packets received from the Band into status 
    /// and data bytes; Band will send data in multiple transmissions for 
    /// one response, and some commands include the status bytes before 
    /// the data bytes, some include the status bytes after the data, and 
    /// some send the status bytes as a separate transmission.
    /// 	
    /// This class can handle all three cases when adding new 
    /// 	received response packets into the response instance.
    /// </summary>
    public class CommandResponse
    {

        /// <summary>Status bytes</summary>
        public byte[] Status = new byte[6];

        /// <summary>Received data byte sequences</summary>
        public List<byte[]> Data = new List<byte[]>();

        /// <summary>Response status bytes sequence length</summary>
        public const int RESPONSE_STATUS_LENGTH = 6;


        /// <summary>
        /// Add a new response bytes sequence to the response.
        /// 
        /// Automatically detects the presence of the Band status byte 
        /// sequence at the start of end of the bytes array and handles it 
        /// accordingly, assigning it to `Status` (overwriting any previous 
        /// `Status` found in a previous byte sequence added to this response 
        /// instance) and using the rest of the bytes as data bytes.
        /// 	
        /// The data bytes are appended as a new item in the data list.
        /// </summary>
        /// <param name="bytes">bytes</param>
        public void AddResponse( byte[] bytes )
        {
            if( ResponseBytesStartWithStatus( bytes ) )
            {
                this.Status = GetResponseStatusBytesStart( bytes );
                if( ResponseBytesContainData( bytes ) )
                {
                    this.AddResponseData( GetResponseDataBytesStart( bytes ) );
                }
            }
            else if( ResponseBytesEndWithStatus( bytes ) )
            {
                this.Status = GetResponseStatusBytesEnd( bytes );
                if( ResponseBytesContainData( bytes ) )
                {
                    this.AddResponseData( GetResponseDataBytesEnd( bytes ) );
                }
            }
            else this.AddResponseData( bytes );
        }


        /// <summary>
        /// Add a response data bytes sequence to the data list.
        /// </summary>
        /// <param name="bytes">bytes</param>
        protected void AddResponseData( byte[] bytes )
        {
            this.Data.Add( bytes );
        }


        /// <summary>
        /// Get the data associated with the response.
        /// 
        /// Returns the data bytes array in the `Data` list 
        /// of received data sequences at the given index.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>byte[]</returns>
        public byte[] GetData( int index = 0 )
        {
            byte[] btResult = null;

            if( Data.Count > index && this.Data[index].Length > 0 )
                btResult = this.Data[index];

            return btResult;
        }

        public static byte[] Combine( byte[] first, byte[] second )
        {
            byte[] bytes = new byte[first.Length + second.Length];
            System.Buffer.BlockCopy( first, 0, bytes, 0, first.Length );
            System.Buffer.BlockCopy( second, 0, bytes, first.Length, second.Length );
            return bytes;
        }

        public byte[] GetAllData()
        {
            int iIndex = 0;
            byte[] btResult = null;

            while( Data.Count > iIndex && Data[iIndex].Length > 0 )
            {
                if( iIndex == 0 )
                    btResult = this.Data[iIndex];
                else if( btResult != null )
                    btResult = Combine( btResult, Data[iIndex] );

                iIndex++;
            }
            return btResult;
        }


        /// <summary>
        /// Get the data associated with the response as a `ByteStream`.
        /// 
        /// Returns a `ByteStream` for the data bytes array in the `Data` 
        /// list of received data sequences at the given index.
        /// </summary>
        /// <param name="index">index</param>
        /// <returns>ByteStream</returns>
        public ByteStream GetByteStream( int index = 0 )
        {
            var data = this.GetData( index );
            return data != null ? new ByteStream( data ) : null;
        }


        /// <summary>
        ///	Get whether the response has status bytes.
        /// </summary>
        /// <returns>bool</returns>
        public bool StatusReceived()
        {
            return !this.Status.All( s => s == 0 );
        }


        /// <summary>
        /// Get Band data bytes from the start of an array of response bytes, 
        /// assuming the offset is the length of the Band status byte sequence.
        /// 	
        /// Doesn't verify the bytes are data or that the offset is correct.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>byte[]</returns>
        public static byte[] GetResponseDataBytesStart( byte[] bytes )
        {
            return bytes.Skip( RESPONSE_STATUS_LENGTH ).ToArray();
        }


        /// <summary>
        /// Get Band data bytes from the end of an array of response bytes, 
        /// assuming the offset is the length of the Band status byte sequence.
        /// 	
        /// Doesn't verify the bytes are data or that the offset is correct.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>byte[]</returns>
        public static byte[] GetResponseDataBytesEnd( byte[] bytes )
        {
            int offset = ( bytes.Length - RESPONSE_STATUS_LENGTH );
            return bytes.Take( offset ).ToArray();
        }


        /// <summary>
        /// Get Band status bytes from the start of an array of 
        /// response bytes, assuming the status byte sequence 
        /// is of length `RESPONSE_STATUS_LENGTH`.
        /// 	
        /// Does not verify that the selected bytes are status bytes.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>byte[]</returns>
        public static byte[] GetResponseStatusBytesStart( byte[] bytes )
        {
            return bytes.Take( RESPONSE_STATUS_LENGTH ).ToArray();
        }


        /// <summary>
        /// Get Band status bytes from the end of an array of 
        /// response bytes, assuming the status byte sequence 
        /// is of length `RESPONSE_STATUS_LENGTH`.
        /// 	
        /// Does not verify that the selected bytes are status bytes.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>byte[]</returns>
        public static byte[] GetResponseStatusBytesEnd( byte[] bytes )
        {
            int offset = ( bytes.Length - RESPONSE_STATUS_LENGTH );
            return bytes.Skip( offset ).ToArray();
        }


        /// <summary>
        /// Get whether an array of response bytes appears to be a 
        /// Band status sequence (starts with Band status byte indicators).
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>bool</returns>
        public static bool ResponseBytesAreStatus( byte[] bytes )
        {
            int[] ints = bytes.Select( b => (int)b ).ToArray();
            return ( ints[0] == 254 && ints[1] == 166 );
        }


        /// <summary>
        /// Get whether an array of Band response bytes appears to 
        /// contain data bytes (array is longer than the regular 
        /// status byte sequence length).
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>bool</returns>
        public static bool ResponseBytesContainData( byte[] bytes )
        {
            return ( bytes.Length > RESPONSE_STATUS_LENGTH );
        }


        /// <summary>
        /// Get whether an array of Band response bytes appears to 
        /// start with a Band status byte sequence (bytes given 
        /// by `GetResponseStatusBytesStart` appear to be status 
        /// bytes as given by `ResponseBytesAreStatus`).
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>bool</returns>
        public static bool ResponseBytesStartWithStatus( byte[] bytes )
        {
            byte[] status = GetResponseStatusBytesStart( bytes );
            return ResponseBytesAreStatus( status );
        }


        /// <summary>
        /// Get whether an array of Band response bytes appears to 
        /// end with a Band status byte sequence (bytes given 
        /// `GetResponseStatusBytesEnd` appear to be status bytes 
        /// as given by `ResponseBytesAreStatus`).
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <returns>bool</returns>
        public static bool ResponseBytesEndWithStatus( byte[] bytes )
        {
            byte[] status = GetResponseStatusBytesEnd( bytes );
            return ResponseBytesAreStatus( status );
        }

    }

}