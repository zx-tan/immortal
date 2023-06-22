using NullPointerCore;
using System.Collections.Generic;

namespace Immortals
{

	public class PlayerDeck : PlayerSystem
	{
		public HeroConfig selectedHero;
		public List<StackedCard> defenseCards = new List<StackedCard>();
		public List<StackedCard> attackCards = new List<StackedCard>();

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
				int resultCost = 0;
				foreach (StackedCard card in defenseCards)
					resultCost += card.type.cost * card.stack;
				return resultCost;
			}
		}

		public int CurrentAttackDeckCost 
		{
			get
			{
				int resultCost = 0;
				foreach(StackedCard card in attackCards)
					resultCost += card.type.cost * card.stack;
				return resultCost;
			}
		}


		public void OnEnable()
		{
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

			if (attackCards.Count == 0)
			{
				attackCards.Add(new StackedCard(unitConfig));
			}
			else if(attackCards[attackCards.Count-1].type == unitConfig)
			{
				if (attackCards[attackCards.Count - 1].CanStack(1))
					attackCards[attackCards.Count - 1].StackPush();
				else
					return false;
			}
			else
				attackCards.Add(new StackedCard(unitConfig));

			return true;
		}

		public bool TryRemoveDefenseCard(int cardIndex)
		{
			if (cardIndex < 0 || cardIndex >= defenseCards.Count)
				return false;

			if (defenseCards[cardIndex].stack == 1)
				defenseCards.RemoveAt(cardIndex);
			else
				defenseCards[cardIndex].StackPop();
			return true;
		}

		public bool TryRemoveAttackCard(int cardIndex)
		{
			if (cardIndex < 0 || cardIndex >= attackCards.Count)
				return false;

			if (attackCards[cardIndex].stack == 1)
				attackCards.RemoveAt(cardIndex);
			else
				attackCards[cardIndex].StackPop();
			return true;
		}
	}
}