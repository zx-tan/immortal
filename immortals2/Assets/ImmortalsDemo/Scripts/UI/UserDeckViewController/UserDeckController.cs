using NullPointerCore;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Immortals
{
	public class UserDeckController : UIViewControllerBase
	{
		private enum DECK_VIEW {
			ATTACK_VIEW,
			DEFENSE_VIEW
		}

		private DECK_VIEW _deckView;
		private GameController gameController;
		private PlayerDeck playerDeck;
		private Player LocalPlayer { get { return Game.GetSystem<GameController>().localPlayer; } }

		protected PlayerDeck Deck 
		{
			get
			{
				if (playerDeck == null && LocalPlayer != null)
					playerDeck = LocalPlayer.GetComponent<PlayerDeck>();
				return playerDeck;
			}
		}

		public void OnEnable()
		{
			if (LocalPlayer != null)
			{
				playerDeck = LocalPlayer.GetComponent<PlayerDeck>();
			}

			gameController = Game.GetSystem<GameController>();
		}

		public void OnViewDefenseDeck()
		{
			if (gameController != null)
			{
				_deckView = DEFENSE_VIEW;
			}
		}

		public void OnViewAttackDeck()
		{
			if (gameController != null)
			{
				_deckView = ATTACK_VIEW;
			}
		}

		public void ResetView()
		{

		}

		public void UpdateView()
		{

		}
	}
}