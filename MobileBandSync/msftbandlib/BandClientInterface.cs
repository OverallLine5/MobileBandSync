using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MobileBandSync.MSFTBandLib {

/// <summary>Band client interface</summary>
public interface BandClientInterface {

	/// <summary>
	/// Get an array of all available paired Bands.
	/// </summary>
	/// <returns>Task<List<BandInterface>></returns>
	Task<List<BandInterface>> GetPairedBands();

}

}