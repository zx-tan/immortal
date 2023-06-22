using UnityEngine;

namespace NullPointerCore
{
	/// <summary>
	/// Base class for all the player systems.
	/// </summary>
	[RequireComponent(typeof(Player))]
	public abstract class PlayerSystem : MonoBehaviour
	{
		/// <summary>
		/// cache for the Player component.
		/// </summary>
		private Player playerRef = null;

		/// <summary>
		/// Quick Reference to the Player owner of this system.
		/// </summary>
		public Player ThisPlayer 
		{
			get
			{
				if ( playerRef==null )
					playerRef = GetComponent<Player>();
				return playerRef;
			}
		}
	}
}
