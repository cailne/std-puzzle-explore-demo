using UnityEditor;
using UnityEngine;

namespace Lucielle
{
	public class UtilityEditor : Editor
	{
		[MenuItem("Lucielle/Active Toggle _`")]
		private static void ToggleActivationSelection()
		{
			if (Selection.gameObjects != null)
			{
				if (Selection.gameObjects.Length > 1)
				{
					GameObject[] go = Selection.gameObjects;
					for (int i = 0; i < go.Length; i++)
					{
						if (go != null)
							go[i].SetActive(!go[i].activeSelf);
					}
					Undo.RegisterCompleteObjectUndo(go, "Set Active Multiple GO");
				}
				else
				{
					GameObject go = Selection.activeGameObject;
					if (go != null)
					{
						go.SetActive(!go.activeSelf);
						Undo.RegisterCompleteObjectUndo(go, "Set Active -> " + go.name);
					}
				}
			}
		}

		[MenuItem("Lucielle/PositionZero &_`")]
		private static void PositionToZero()
		{
			if (Selection.gameObjects != null)
			{
				if (Selection.gameObjects.Length > 1)
				{
					GameObject[] go = Selection.gameObjects;
					foreach (GameObject g in go)
					{
						if (go != null)
							g.transform.position = Vector3.zero;
					}
					Undo.RegisterCompleteObjectUndo(go, "Set Multiple Position GO to Zero");
				}
				else
				{
					GameObject go = Selection.activeGameObject;
					if (go != null)
					{
						go.transform.position = Vector3.zero;
						Undo.RegisterCompleteObjectUndo(go, "Set Position Zero -> " + go.name);
					}
				}
			}
		}

		[MenuItem("Lucielle/Auto Anchor #_`")]
		private static void uGUIAnchorAroundObject()
		{
			GameObject o = Selection.activeGameObject;
			if (o != null && o.GetComponent<RectTransform>() != null)
			{
				RectTransform r = o.GetComponent<RectTransform>();
				RectTransform p = o.transform.parent.GetComponent<RectTransform>();

				Vector2 offsetMin = r.offsetMin;
				Vector2 offsetMax = r.offsetMax;
				Vector2 _anchorMin = r.anchorMin;
				Vector2 _anchorMax = r.anchorMax;

				float parent_width = p.rect.width;
				float parent_height = p.rect.height;

				Vector2 anchorMin = new Vector2(_anchorMin.x + (offsetMin.x / parent_width),
											_anchorMin.y + (offsetMin.y / parent_height));
				Vector2 anchorMax = new Vector2(_anchorMax.x + (offsetMax.x / parent_width),
											_anchorMax.y + (offsetMax.y / parent_height));

				r.anchorMin = anchorMin;
				r.anchorMax = anchorMax;

				r.offsetMin = new Vector2(0, 0);
				r.offsetMax = new Vector2(0, 0);
				r.pivot = new Vector2(0.5f, 0.5f);

			}
		}

		private delegate void ChangePrefab(GameObject go);
		private const int SelectionThresholdForProgressBar = 20;
		private static bool showProgressBar;
		private static int changedObjectsCount;

		[MenuItem("Lucielle/Apply Changes To Selected Prefabs %j", false, 100)]
		private static void ApplyPrefabs()
		{
			SearchPrefabConnections(ApplyToSelectedPrefabs);
		}

		[MenuItem("Lucielle/Revert Changes Of Selected Prefabs", false, 100)]
		private static void ResetPrefabs()
		{
			SearchPrefabConnections(RevertToSelectedPrefabs);
		}

		[MenuItem("Lucielle/Apply Changes To Selected Prefabs #_2", true)]
		[MenuItem("Lucielle/Revert Changes Of Selected Prefabs #_3", true)]
		private static bool IsSceneObjectSelected()
		{
			return Selection.activeTransform != null;
		}

		private static void SearchPrefabConnections(ChangePrefab changePrefabAction)
		{
			GameObject[] selectedTransforms = Selection.gameObjects;
			int numberOfTransforms = selectedTransforms.Length;
			showProgressBar = numberOfTransforms >= SelectionThresholdForProgressBar;
			changedObjectsCount = 0;
			try
			{
				for (int i = 0; i < numberOfTransforms; i++)
				{
					GameObject go = selectedTransforms[i];
					if (showProgressBar)
					{
						EditorUtility.DisplayProgressBar("Update prefabs", "Updating prefab " + go.name + " (" + i + "/" + numberOfTransforms + ")",
							i / (float)numberOfTransforms);
					}
					IterateThroughObjectTree(changePrefabAction, go);
				}
			}
			finally
			{
				if (showProgressBar)
				{
					EditorUtility.ClearProgressBar();
				}
				Debug.LogFormat("{0} Prefab(s) updated", changedObjectsCount);
			}
		}

		private static void IterateThroughObjectTree(ChangePrefab changePrefabAction, GameObject go)
		{
			// var prefabType = PrefabUtility.GetPrefabType(go);
			PrefabAssetType assetType = PrefabUtility.GetPrefabAssetType(go);
			PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(go);
			if (status == PrefabInstanceStatus.Disconnected || assetType != PrefabAssetType.NotAPrefab)
			// if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.DisconnectedPrefabInstance)
			{
				// var prefabRoot = PrefabUtility.FindRootGameObjectWithSameParentPrefab(go);
				GameObject prefabRoot = PrefabUtility.GetOutermostPrefabInstanceRoot(go);
				if (prefabRoot != null)
				{
					changePrefabAction(prefabRoot);
					changedObjectsCount++;
					return;
				}
			}
			Transform transform = go.transform;
			int children = transform.childCount;
			for (int i = 0; i < children; i++)
			{
				GameObject childGo = transform.GetChild(i).gameObject;
				IterateThroughObjectTree(changePrefabAction, childGo);
			}
		}

		private static void ApplyToSelectedPrefabs(GameObject go)
		{
			// var prefabAsset = PrefabUtility.GetPrefabParent(go);
			GameObject prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);
			if (prefabAsset == null)
			{
				return;
			}
			string assetPath = AssetDatabase.GetAssetPath(prefabAsset);
			// PrefabUtility.ReplacePrefab(go, prefabAsset, ReplacePrefabOptions.ConnectToPrefab);
			PrefabUtility.SaveAsPrefabAsset(go, assetPath);
		}

		private static void RevertToSelectedPrefabs(GameObject go)
		{
			// PrefabUtility.ReconnectToLastPrefab(go);
			PrefabUtility.RevertPrefabInstance(go, InteractionMode.UserAction);
		}

		public static string fileName = "Screenshot ";
		public static int startNumber = 1;

		[MenuItem("Lucielle/Take Screenshot of Game View #_4")]
		private static void TakeScreenshot()
		{
			int number = startNumber;
			string name = "" + number;

			while (System.IO.File.Exists(fileName + name + ".jpg"))
			{
				number++;
				name = "" + number;
			}

			startNumber = number + 1;

			ScreenCapture.CaptureScreenshot(fileName + name + ".jpg");
		}

		[MenuItem("Lucielle/Clear All Playerprefs")]
		private static void DeleteAll()
		{
			PlayerPrefs.DeleteAll();
		}
	}
}