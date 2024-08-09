using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
	private static Random _random = new Random();

	public static T GetRandomItem<T>( this IEnumerable<T> source )
	{
		if ( source == null || !source.Any() )
		{
			throw new InvalidOperationException( "Cannot get a random item from an empty or null collection." );
		}

		int index = _random.Next( 0, source.Count() );
		return source.ElementAt( index );
	}
}
