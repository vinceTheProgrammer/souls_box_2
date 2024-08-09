using System;

public static class FloatExtensions
{
	public static float RoundToNearestThreshold( this float value, float threshold )
	{
		if ( Math.Abs( value - 1 ) <= threshold )
		{
			return 1;
		}
		else if ( Math.Abs( value + 1 ) <= threshold )
		{
			return -1;
		}
		else if ( Math.Abs( value ) <= threshold )
		{
			return 0;
		}
		return value; // Return the original value if it's not within any threshold.
	}
}
