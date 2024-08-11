using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoulsBox
{
	public sealed class AnimTagEventManager
	{
		private readonly Dictionary<string, bool> _activeTags = new();
		private readonly Dictionary<string, Action> _startCallbacks = new();
		private readonly Dictionary<string, Action> _endCallbacks = new();
		private readonly Dictionary<string, Action> _firedCallbacks = new();

		public void RegisterRenderer( SkinnedModelRenderer renderer )
		{
			renderer.OnAnimTagEvent += OnAnimTagEvent;
		}

		private void OnAnimTagEvent( SceneModel.AnimTagEvent ev )
		{
			switch ( ev.Status )
			{
				case SceneModel.AnimTagStatus.Start:
					_activeTags[ev.Name] = true;
					if ( _startCallbacks.TryGetValue( ev.Name, out var startCallback ) )
					{
						startCallback?.Invoke();
					}
					break;

				case SceneModel.AnimTagStatus.End:
					_activeTags[ev.Name] = false;
					if ( _endCallbacks.TryGetValue( ev.Name, out var endCallback ) )
					{
						endCallback?.Invoke();
					}
					break;

				case SceneModel.AnimTagStatus.Fired:
					if ( _firedCallbacks.TryGetValue( ev.Name, out var firedCallback ) )
					{
						firedCallback?.Invoke();
					}
					break;
			}
		}

		public bool IsTagActive( string tagName )
		{
			return _activeTags.TryGetValue( tagName, out var isActive ) && isActive;
		}

		public void RegisterTagCallback( string tagName, SceneModel.AnimTagStatus status, Action callback )
		{
			switch ( status )
			{
				case SceneModel.AnimTagStatus.Start:
					if ( _startCallbacks.ContainsKey( tagName ) )
					{
						_startCallbacks[tagName] += callback;
					}
					else
					{
						_startCallbacks[tagName] = callback;
					}
					break;

				case SceneModel.AnimTagStatus.End:
					if ( _endCallbacks.ContainsKey( tagName ) )
					{
						_endCallbacks[tagName] += callback;
					}
					else
					{
						_endCallbacks[tagName] = callback;
					}
					break;

				case SceneModel.AnimTagStatus.Fired:
					if ( _firedCallbacks.ContainsKey( tagName ) )
					{
						_firedCallbacks[tagName] += callback;
					}
					else
					{
						_firedCallbacks[tagName] = callback;
					}
					break;
			}
		}
	}

}
