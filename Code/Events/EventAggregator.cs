using System;
using System.Collections.Generic;

public class EventAggregator
{
	private readonly Dictionary<Type, List<object>> _subscribers = new Dictionary<Type, List<object>>();

	public void Subscribe<TEvent>( Action<TEvent> handler )
	{
		if ( !_subscribers.ContainsKey( typeof( TEvent ) ) )
		{
			_subscribers[typeof( TEvent )] = new List<object>();
		}
		_subscribers[typeof( TEvent )].Add( handler );
	}

	public void Unsubscribe<TEvent>( Action<TEvent> handler )
	{
		if ( _subscribers.ContainsKey( typeof( TEvent ) ) )
		{
			_subscribers[typeof( TEvent )].Remove( handler );
		}
	}

	public void Publish<TEvent>( TEvent eventItem )
	{
		if ( _subscribers.ContainsKey( eventItem.GetType() ) )
		{
			foreach ( var handler in _subscribers[eventItem.GetType()] )
			{
				((Action<TEvent>)handler).Invoke( eventItem );
			}
		}
	}
}

