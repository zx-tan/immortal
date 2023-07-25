using NullPointerCore;
using System.Collections.Generic;

namespace Immortals
{

	public class PlayerDeck : PlayerSystem
	{
		[SerializeField] private HeroConfig selectedHero;
		[SerializeField] private List<StackedCard> defenseCards = new List<StackedCard>();
		[SerializeField] private List<StackedCard> attackCards = new List<StackedCard>();

		private GameController gameControlCache;

		public GameController GameControl {
			get
			{
				if (gameControlCache == null)
					gameControlCache = Game.GetSystem<GameController>();
				return gameControlCache;
			}
		}

		public int CurrentDefenseDeckCost {
			get
			{
				return CalculateDeckCost(defenseCards);
			}
		}

		public int CurrentAttackDeckCost 
		{
			get
			{
				return CalculateDeckCost(attackCards);
			}
		}

		private int CalculateDeckCost(List<StackedCard> stackedCards)
		{
			int resultCost = 0;
			foreach(StackedCard card in stackedCards)
				resultCost += card.type.cost * card.stack;
			return resultCost;
		}

		public bool TryAddDefenseCard(string cardId)
		{
			bool found = false;
			UnitConfig unitConfig = GameControl.GetUnit(cardId);

			if (CurrentDefenseDeckCost + unitConfig.cost > GameControl.Config.defenseDeckLimit)
				return false;

			foreach (StackedCard card in defenseCards)
			{
				if (card.type == unitConfig)
				{
					if (card.CanStack(1))
						card.StackPush();
					else
						return false;
					found = true;
					break;
				}
			}
			if (!found)
			{
				StackedCard card = new StackedCard(unitConfig);
				defenseCards.Add(card);
			}

			return true;
		}


		public bool TryAddAttackCard(string cardId)
		{
			UnitConfig unitConfig = GameControl.GetUnit(cardId);

			if (CurrentAttackDeckCost + unitConfig.cost > GameControl.Config.attackDeckLimit)
				return false;

			return TryAddCard(unitConfig, attackCards);
		}

		private bool TryAddCard(UnitConfig unitConfig, List<StackedCard> stackedCards)
		{
			if (stackedCards.Count == 0)
			{
				stackedCards.Add(new StackedCard(unitConfig));
			}
			else if(stackedCards[stackedCards.Count-1].type == unitConfig)
			{
				if (stackedCards[stackedCards.Count - 1].CanStack(1))
					stackedCards[stackedCards.Count - 1].StackPush();
				else
					return false;
			}
			else
				stackedCards.Add(new StackedCard(unitConfig));

			return true;
		}

		public bool TryRemoveDefenseCard(int cardIndex)
		{
			return TryRemoveCard(cardIndex, defenseCards);
		}

		public bool TryRemoveAttackCard(int cardIndex)
		{
			return TryRemoveCard(cardIndex, attackCards);
		}

		private bool TryRemoveCard(int cardIndex, List<StackedCard> stackedCards)
		{
			if (IsCardIndexValid(stackedCards))
				return false;

			if (stackedCards[cardIndex].stack == 1)
				stackedCards.RemoveAt(cardIndex);
			else
				stackedCards[cardIndex].StackPop();
			return true;
		}

		private bool IsCardIndexValid(List<StackedCard> stackedCards)
		{
			return cardIndex < 0 || cardIndex >= stackedCards.Count;
		}
	}
}