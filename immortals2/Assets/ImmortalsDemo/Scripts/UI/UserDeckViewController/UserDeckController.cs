﻿using NullPointerCore;
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

		[Header("Deck")]
		[SerializeField] private GameObject _deckCardPrefab;
		[SerializeField] private Transform _deckContentAttack;
		[SerializeField] private Image _deckCostBar;
		[SerializeField] private Text _deckCostText;

		[Header("Pool")]
		[SerializeField] private GameObject _poolCardPrefab;
		[SerializeField] private Transform _poolContent;
		[SerializeField] private Text _foundInPoolText;

		private DECK_VIEW _deckView;
		private GameController _gameController;
		private PlayerDeck _playerDeck;

		private Player LocalPlayer { get { return Game.GetSystem<GameController>().localPlayer; } }

		protected PlayerDeck Deck 
		{
			get
			{
				if (_playerDeck == null && LocalPlayer != null)
					_playerDeck = LocalPlayer.GetComponent<PlayerDeck>();
				return _playerDeck;
			}
		}

		public void OnEnable()
		{
			if (LocalPlayer != null)
			{
				_playerDeck = LocalPlayer.GetComponent<PlayerDeck>();
			}

			_gameController = Game.GetSystem<GameController>();
		}

		public void OnViewDefenseDeck()
		{
			if (_gameController == null)
			{
				return;
			}
			
			_deckView = DEFENSE_VIEW;
			InitialiseDeckContent(_gameController.Config.DefensiveUnits);
		}

		public void OnViewAttackDeck()
		{
			if (_gameController == null)
			{
				return;
			}

			_deckView = ATTACK_VIEW;
			InitialiseDeckContent(_gameController.Config.AttackUnits);
			RefreshAttackDeck();
		}

		private void InitialiseDeckContent(IEnumerable<UnitConfig> poolProvider)
		{
			ClearContent(_poolContent);
			FillCardPool(poolProvider);
		}

		private void RefreshAttackDeck()
		{
			ClearContent(_deckContentAttack);
			if (Deck != null)
			{
				FillCardDeck(Deck.attackCards, _deckContentAttack);

				if (_deckCostBar != null)
					_deckCostBar.fillAmount = (float)Deck.CurrentAttackDeckCost / _gameController.Config.attackDeckLimit;
				if (_deckCostText != null)
					_deckCostText.text = Deck.CurrentAttackDeckCost + "/" + _gameController.Config.attackDeckLimit;
			}
		}

		private void FillCardDeck(IEnumerable<StackedCard> deckProvider, Transform parent)
		{
			if (_poolCardPrefab == null || _poolContent == null)
				return;

			int index = 0;
			foreach (StackedCard stack in deckProvider)
			{
				UICardItem cardUI = CreateCardItem(_deckCardPrefab, parent);
				cardUI.Setup(stack.type);
				cardUI.Index = index;
				cardUI.SetCardAmount(stack.stack);
				cardUI.RemoveRequested.AddListener(OnCardRemoveClicked);
				cardUI.UpgradeRequested.AddListener(OnCardUpgradeClicked);
				cardRoot.SetActive(true);
				index++;
			}
		}

		private void FillCardPool(IEnumerable<UnitConfig> poolProvider)
		{
			if (_poolCardPrefab == null || _poolContent == null)
				return;

			int count = 0;
			foreach (UnitConfig unit in poolProvider)
			{
				UICardItem cardUI = CreateCardItem(_poolCardPrefab, _poolContent);
				cardUI.Setup(unit);
				cardUI.UseRequested.AddListener(this.OnCardUseClicked);
				cardUI.UpgradeRequested.AddListener(this.OnCardUpgradeClicked);
				cardRoot.SetActive(true);
				cardRoot.transform.SetAsFirstSibling();
				count++;
			}
			if (_foundInPoolText != null)
			{
				int total = gameController.Config.units.Count;
				string finalText = string.Format("Found: {0}/{1}", count, total);
				_foundInPoolText.text = Game.GetSystem<LanguageSystem>().GetText(finalText);
			}
		}

		private UICardItem CreateCardItem(GameObject prefab, Transform parent)
		{
			GameObject cardRoot = GameObject.Instantiate(prefab, parent);
			UICardItem cardUI = cardRoot.GetComponent<UICardItem>();

			return cardUI;
		}

		private void ClearContent(Transform content)
		{
			for(int i= content.childCount-1; i>=0; i--)
			{
				Transform child = content.GetChild(i);
				GameObject.Destroy(child.gameObject);
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