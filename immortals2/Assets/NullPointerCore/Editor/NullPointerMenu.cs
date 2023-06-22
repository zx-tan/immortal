using NullPointerCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace NullPointerEditor
{
	public static class NullPointerMenu
	{
		[MenuItem("GameObject/NullPointerGame/UISelectionInput", false, 7)]
		public static void AddSelectionInput(MenuCommand menuCommand)
		{
			CreateUISelectionInput(menuCommand.context as GameObject);
		}

		public static void CreateUISelectionInput(GameObject context)
		{
			GameObject element = EditorUIHelpers.CreateUIElementRoot("GameplayArea", new Vector2(0, 0f));
			RectTransform component = element.GetComponent<RectTransform>();
			component.anchorMin = Vector2.zero;
			component.anchorMax = Vector2.one;
			component.anchoredPosition = Vector2.zero;
			Image image = element.AddComponent<Image>();
			image.sprite = null;
			image.type = Image.Type.Sliced;
			image.color = new Color(1, 1, 1, 0);
			EditorUIHelpers.PlaceUIElementRoot(element, context);
			component.sizeDelta = Vector2.zero;
			element.AddComponent<UISelectionInput>();
		}
	}
}