using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NullPointerEditor
{
	public static class EditorUIHelpers
	{
		public static GameObject CreateNewUI()
		{
			GameObject gameObject = new GameObject("Canvas");
			gameObject.layer = LayerMask.NameToLayer("UI");
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			gameObject.AddComponent<CanvasScaler>();
			gameObject.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			CreateEventSystem(false);
			return gameObject;
		}

		private static void CreateEventSystem(bool select)
		{
			CreateEventSystem(select, null);
		}

		public static void CreateEventSystem(bool select, GameObject parent)
		{
			EventSystem eventSystem = Object.FindObjectOfType<EventSystem>();
			if (eventSystem == null)
			{
				GameObject gameObject = new GameObject("EventSystem");
				GameObjectUtility.SetParentAndAlign(gameObject, parent);
				eventSystem = gameObject.AddComponent<EventSystem>();
				gameObject.AddComponent<StandaloneInputModule>();
				Undo.RegisterCreatedObjectUndo(gameObject, "Create " + gameObject.name);
			}
			if (select && eventSystem != null)
				Selection.activeGameObject = eventSystem.gameObject;
		}



		public static GameObject CreateUIElementRoot(string name, Vector2 size)
		{
			GameObject gameObject = new GameObject(name);
			RectTransform rectTransform = gameObject.AddComponent<RectTransform>();
			rectTransform.sizeDelta = size;
			return gameObject;
		}



		public static GameObject GetOrCreateCanvasGameObject()
		{
			GameObject activeGameObject = Selection.activeGameObject;
			Canvas canvas = (!(activeGameObject != null)) ? null : activeGameObject.GetComponentInParent<Canvas>();
			GameObject result;
			if (canvas != null && canvas.gameObject.activeInHierarchy)
			{
				result = canvas.gameObject;
			}
			else
			{
				canvas = (UnityEngine.Object.FindObjectOfType(typeof(Canvas)) as Canvas);
				if (canvas != null && canvas.gameObject.activeInHierarchy)
					result = canvas.gameObject;
				else
					result = EditorUIHelpers.CreateNewUI();
			}
			return result;
		}

		public static void PlaceUIElementRoot(GameObject element, GameObject parent)
		{
			GameObject gameObject = parent as GameObject;
			if (gameObject == null || gameObject.GetComponentInParent<Canvas>() == null)
			{
				gameObject = GetOrCreateCanvasGameObject();
			}
			string uniqueNameForSibling = GameObjectUtility.GetUniqueNameForSibling(gameObject.transform, element.name);
			element.name = uniqueNameForSibling;
			Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
			Undo.SetTransformParent(element.transform, gameObject.transform, "Parent " + element.name);
			GameObjectUtility.SetParentAndAlign(element, gameObject);
			//if (gameObject != parent)
			//{
			//	SetPositionVisibleinSceneView(gameObject.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());
			//}
			Selection.activeGameObject = element;
		}

		private static void SetPositionVisibleinSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			if (sceneView == null && SceneView.sceneViews.Count > 0)
			{
				sceneView = (SceneView.sceneViews[0] as SceneView);
			}
			if (!(sceneView == null) && !(sceneView.camera == null))
			{
				Camera camera = sceneView.camera;
				Vector3 zero = Vector3.zero;
				Vector2 vector;
				if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2((float)(camera.pixelWidth / 2), (float)(camera.pixelHeight / 2)), camera, out vector))
				{
					vector.x += canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
					vector.y += canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;
					vector.x = Mathf.Clamp(vector.x, 0f, canvasRTransform.sizeDelta.x);
					vector.y = Mathf.Clamp(vector.y, 0f, canvasRTransform.sizeDelta.y);
					zero.x = vector.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
					zero.y = vector.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;
					Vector3 vector2;
					vector2.x = canvasRTransform.sizeDelta.x * (0f - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
					vector2.y = canvasRTransform.sizeDelta.y * (0f - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;
					Vector3 vector3;
					vector3.x = canvasRTransform.sizeDelta.x * (1f - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
					vector3.y = canvasRTransform.sizeDelta.y * (1f - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;
					zero.x = Mathf.Clamp(zero.x, vector2.x, vector3.x);
					zero.y = Mathf.Clamp(zero.y, vector2.y, vector3.y);
				}
				itemTransform.anchoredPosition = zero;
				itemTransform.localRotation = Quaternion.identity;
				itemTransform.localScale = Vector3.one;
			}
		}
	}
}