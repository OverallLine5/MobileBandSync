using MobileBandSync.MSFTBandLib.Command;
using System;
using System.Collections.Generic;
using static System.DateTimeKind;

namespace MobileBandSync.MSFTBandLib.Helpers {

/// <summary>Time helper</summary>
public static class TimeHelper {

	/// <summary>
	/// Create a `DateTime` from an array of `ushort`.
	/// 
	/// Array must be YYYY, MM, DD, HH, MM, SS.
	/// </summary>
	/// <param name="times">t</param>
	/// <returns>DateTime</returns>
	public static DateTime DateTimeUshorts(ushort[] t) {
		return new DateTime(t[0], t[1], t[2], t[3], t[4], t[5]);
	}


	/// <summary>
	/// Create a `DateTime` response from a Band command 
	/// response containing time specified as a sequence 
	/// of 8 `ushort` from a given position.
	/// </summary>
	/// <param name="response">response</param>
	/// <param name="position">Position to read sequence from </param>
	/// <returns>DateTime</returns>
	public static DateTime DateTimeResponse(
		CommandResponse response, int position=0) {

		ushort[] t = response.GetByteStream().GetUshorts(8, position);
		List<ushort> times = new List<ushort>(t);
		times.RemoveAt(2);
		return TimeHelper.DateTimeUshorts(times.ToArray());
	}

}

}