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

		[Header("Pool")]
		[SerializeField] private GameObject _poolCardPrefab;
		[SerializeField] private Transform _poolContent;
		[SerializeField] private Text foundInPoolText;

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
				FillCardPool(gameController.Config.DefensiveUnits);
			}
		}

		public void OnViewAttackDeck()
		{
			if (gameController != null)
			{
				_deckView = ATTACK_VIEW;
				FillCardPool(gameController.Config.AttackUnits);
			}
		}

		private void FillCardPool(IEnumerable<UnitConfig> poolProvider)
		{
			if (_poolCardPrefab == null || _poolContent == null)
				return;

			int count = 0;
			foreach (UnitConfig unit in poolProvider)
			{
				GameObject cardRoot = GameObject.Instantiate(_poolCardPrefab, _poolContent);
				UICardItem cardUI = cardRoot.GetComponent<UICardItem>();
				cardUI.Setup(unit);
				cardUI.UseRequested.AddListener(this.OnCardUseClicked);
				cardUI.UpgradeRequested.AddListener(this.OnCardUpgradeClicked);
				cardRoot.SetActive(true);
				cardRoot.transform.SetAsFirstSibling();
				count++;
			}
			if (foundInPoolText != null)
			{
				int total = gameController.Config.units.Count;
				string finalText = string.Format("Found: {0}/{1}", count, total);
				foundInPoolText.text = Game.GetSystem<LanguageSystem>().GetText(finalText);
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