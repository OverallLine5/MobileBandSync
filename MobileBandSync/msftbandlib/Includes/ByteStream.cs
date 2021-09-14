using System;
using System.IO;

namespace MobileBandSync.MSFTBandLib.Includes {

/// <summary>
/// Byte stream class
/// 
/// This is a thin wrapper around `MemoryStream`, `BinaryReader` 
/// and `BinaryWriter` to simplify writing/reading byte arrays.
/// </summary>
public class ByteStream : IDisposable {

	///	<summary>Disposed</summary>
	public bool Disposed { get; protected set; }

	/// <summary>Memory stream</summary>
	public MemoryStream MemoryStream;

	/// <summary>Binary reader</summary>
	public BinaryReader BinaryReader;

	/// <summary>Binary writer</summary>
	public BinaryWriter BinaryWriter;


	/// <summary>Construct.</summary>
	public ByteStream() {
		this.MemoryStream = new MemoryStream();
		this.BinaryReader = new BinaryReader(this.MemoryStream);
		this.BinaryWriter = new BinaryWriter(this.MemoryStream);
	}


	/// <summary>Construct and write bytes.</summary>
	/// <param name="bytes">bytes</param>
	/// <returns>public</returns>
	public ByteStream(byte[] bytes) : this() {
		this.Write(bytes);
	}


	/// <summary>Dispose of the resources.</summary>
	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}


	/// <summary>Dispose of the resources.</summary>
	/// <param name="disposing">Disposing (not used)</param>
	protected virtual void Dispose(bool disposing) {
		if (!this.Disposed) {
			this.BinaryReader.Dispose();
			this.BinaryWriter.Dispose();
			this.MemoryStream.Dispose();
			this.Disposed = true;
		}
	}


	/// <summary>Write bytes.</summary>
	/// <param name="bytes">bytes</param>
	/// <returns>public</returns>
	public void Write(byte[] bytes) {
		this.BinaryWriter.Write(bytes);
	}


	/// <summary>Get the current byte array.</summary>
	/// <returns>byte[]</returns>
	public byte[] GetBytes() {
		return this.MemoryStream.ToArray();
	}


	/// <summary>
	/// Read an 4-byte unsigned integer from the stream using 
	/// the `BinaryReader`. Sets stream to given position, and 
	/// advances stream position by 4 once done. Little-endian encoding.
	/// </summary>
	/// <param name="position">position</param>
	/// <returns>uint</returns>
	public uint GetUint32(int position=0) {
		this.BinaryReader.BaseStream.Position = position;
		return this.BinaryReader.ReadUInt32();
	}


	/// <summary>
	/// Read an 8-byte unsigned integer from the stream using 
	/// the `BinaryReader`. Sets stream to given position, and 
	/// advances stream position by 8 once done. Little-endian encoding.
	/// </summary>
	/// <param name="position">position</param>
	/// <returns>ulong</returns>
	public ulong GetUint64(int position=0) {
		this.BinaryReader.BaseStream.Position = position;
		return this.BinaryReader.ReadUInt64();
	}


	/// <summary>
	/// Read a string from the stream as characters from a given 
	/// position in the stream (position is set and will not be restored).
	/// 	
	/// Note: Implemented with `BinaryReader.ReadChars`, instead 
	/// of `BinaryReader.ReadString`, because the latter expects 
	/// strings to be prepended with their length.
	/// 
	/// This implementation works with any string-like data and 
	/// enables the consumer to specify how many characters to get.
	/// </summary>
	/// <param name="position">Position to read from</param>
	/// <param name="chars">Characters to read (0: all)</param>
	/// <returns>string</returns>
	public string GetString(int position=0, long chars=0) {
		if (chars == 0) chars = this.BinaryReader.BaseStream.Length;
		this.BinaryReader.BaseStream.Position = position;
		return new string(this.BinaryReader.ReadChars((int) chars));
	}


	public byte GetByte(int position=0) {
		this.BinaryReader.BaseStream.Position = position;
		return this.BinaryReader.ReadByte();
	}

	/// <summary>
	/// Read a 2-byte unsigned integer from the stream using 
	/// the `BinaryReader`. Sets stream to given position, and 
	/// advances stream position by 2 once done. Little-endian encoding.
	/// </summary>
	/// <param name="position">Position to read from</position>
	/// <returns>ushort</returns>
	public ushort GetUshort(int position=0) {
		this.BinaryReader.BaseStream.Position = position;
		return this.BinaryReader.ReadUInt16();
	}


	/// <summary>
	/// Get an array of `ushort` from the stream using 
	/// the `BinaryReader`, reading sequentially from the 
	/// given start position. Does not verify the specified 
	/// number of `ushort` is available.
	/// </summary>
	/// <param name="count">Number of `ushort` to read</param>
	/// <param name="pos">Position to read from</param>
	/// <returns>ushort[]</returns>
	public ushort[] GetUshorts(int count, int pos=0) {
		ushort[] ushorts = new ushort[count];
		for (int i = 0; i < count; i++) {
			pos = (i == 0) ? pos : (pos + 2);
			ushorts[i] = this.GetUshort(pos);
		}
		return ushorts;
	}

}

}