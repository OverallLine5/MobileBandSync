using System;

namespace MobileBandSync.MSFTBandLib.Exceptions {

/// <summary>Band socket not connected exception</summary>
public class BandSocketConnectedNot : Exception {

	/// <summary>Constructor.</summary>
	public BandSocketConnectedNot() : 
	base("Band socket is not connected.") {}

}

}