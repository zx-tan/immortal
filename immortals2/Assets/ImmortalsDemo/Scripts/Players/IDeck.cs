using NullPointerCore;
using System.Collections.Generic;

namespace Immortals
{

	public interface IDeck
	{
		int CurrentDefenseDeckCost {
			get;
		}

		int CurrentAttackDeckCost 
		{
			get;
		}
		bool TryAddDefenseCard(string cardId);
		bool TryAddAttackCard(string cardId);
		bool TryRemoveDefenseCard(int cardIndex);
		bool TryRemoveAttackCard(int cardIndex);
	}
}