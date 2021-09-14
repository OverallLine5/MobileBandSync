using System;

namespace MobileBandSync.MSFTBandLib.Exceptions {

/// <summary>Band connection not connected exception</summary>
public class BandConnectionConnectedNot : Exception {

	/// <summary>Constructor.</summary>
	public BandConnectionConnectedNot() : 
	base("Band connection is not connected.") {}

}

}