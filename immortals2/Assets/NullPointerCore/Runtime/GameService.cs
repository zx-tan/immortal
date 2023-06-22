using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NullPointerCore
{
	public class GameService : MonoBehaviour
	{

		protected virtual void OnEnable()
		{
			Game.Register(this);
		}

		protected virtual void OnDisable()
		{
			Game.Unregister(this);
		}
	}
}