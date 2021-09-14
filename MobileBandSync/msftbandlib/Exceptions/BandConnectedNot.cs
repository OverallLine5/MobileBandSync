using System;

namespace MobileBandSync.MSFTBandLib.Exceptions {

/// <summary>Band not connected exception</summary>
public class BandConnectedNot : Exception {

	/// <summary>Constructor.</summary>
	public BandConnectedNot() : base("Band is not connected.") {}

}

}