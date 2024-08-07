using System;

namespace SoulsBox
{
	public static class Vector3Extensions
	{
		public static Vector3 RoundToCardinal(this Vector3 vector3Input )
		{
			Vector2 input = new( vector3Input.x, vector3Input.y );

			// Normalize the input vector to get the direction
			Vector2 normalized = input.Normal;

			// Compute the angle in radians
			float angle = (float)Math.Atan2( normalized.y, normalized.x );

			// Convert radians to degrees
			float degrees = angle * (180.0f / (float)Math.PI);

			// Round to the nearest 90 degrees (0, 90, 180, 270)
			int roundedDegrees = (int)Math.Round( degrees / 90.0f ) * 90;

			// Map the rounded degree to a cardinal direction
			switch ( roundedDegrees % 360 )
			{
				case 0:
					return new Vector3( 1, 0, 0 );   // Right
				case 90:
				case -270:
					return new Vector3( 0, 1, 0 );   // Up
				case 180:
				case -180:
					return new Vector3( -1, 0, 0 );  // Left
				case 270:
				case -90:
					return new Vector3( 0, -1, 0 );   // Down
				default:
					throw new Exception( "Unexpected angle" );
			}
		}

		public static float SignedAngle( this Vector3 from, Vector3 to )
		{
			float angle = Vector3.GetAngle( from, to );
			float crossY = from.x * to.z - from.z * to.x;
			return crossY > 0 ? angle : -angle;
		}
	}
}
