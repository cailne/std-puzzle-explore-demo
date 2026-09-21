#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Lucielle
{
	/// <summary>
	/// A collection of utility functions for the Unity Editor.
	/// </summary>

	public static class EditorUtils
	{
		public static void SaveAssets(Object[] assets)
		{
			AssetDatabase.StartAssetEditing();
			foreach (Object asset in assets)
			{
				SaveAsset(asset);
			}
			AssetDatabase.StopAssetEditing();
		}

		public static void MarkDirty(Object obj)
		{
			EditorUtility.SetDirty(obj);
		}

		public static void SaveAsset(Object asset)
		{
			MarkDirty(asset);
			AssetDatabase.SaveAssets();
		}

		public static void SaveAndRefresh()
		{
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static void ForceSerialize(params Object[] objects)
		{
			List<string> pathList = new();
			foreach (Object obj in objects)
			{
				if (obj == null) continue;
				string assetPath = AssetDatabase.GetAssetPath(obj);
				
				if (string.IsNullOrEmpty(assetPath)) continue;
				pathList.Add(assetPath);
			}

			if (pathList.Count == 0) return;
			AssetDatabase.ForceReserializeAssets(pathList);
		}

		public static List<T> LoadAssets<T>(string assetPath, string filter = "") where T : Object
		{
			bool isUseFilter = !string.IsNullOrEmpty(filter);
			string[] guids = AssetDatabase.FindAssets(isUseFilter ? $"t:{filter}" : "", new string[] { assetPath });
			List<T> objects = new List<T>();
			for (int i = 0; i < guids.Length; i++)
			{
				string path = AssetDatabase.GUIDToAssetPath(guids[i]);
				T obj = LoadAsset<T>(path);

				if (obj is not T) continue;
				objects.Add(obj);
			}
			return objects;
		}

		public static T LoadAsset<T>(string assetPath) where T : Object
		{
			return AssetDatabase.LoadAssetAtPath<T>(assetPath);
		}

		public static async void DownloadFile(string url, Action<string, bool> Callback)
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(url);

			// Request and wait for the desired page.
			webRequest.SendWebRequest();
			while (!webRequest.isDone) await Task.Yield();

			bool isSuccess = webRequest.result == UnityWebRequest.Result.Success;
			Callback.Invoke(isSuccess ? webRequest.downloadHandler.text : webRequest.error, isSuccess);
		}

		public static async void DownloadFileVideo(string url, Action<UnityWebRequest> onDownloading, Action<UnityWebRequest> Callback)
		{
			UnityWebRequest webRequest = UnityWebRequest.Get(url);

			// Request and wait for the desired page.
			webRequest.SendWebRequest();
			while (!webRequest.isDone)
			{
				onDownloading?.Invoke(webRequest);
				await Task.Yield();
			}

			Callback.Invoke(webRequest);
		}

		public static string ReadText(string path)
		{
			string rootPath = Application.dataPath;
			string fullPath = System.IO.Path.Combine(rootPath.Replace("/Assets", ""), path);
			return System.IO.File.ReadAllText(fullPath);
		}

		public static string ReadText(Object obj)
		{
			return ReadText(AssetDatabase.GetAssetPath(obj));
		}

		public static void CreateFolderIfNotExist(string path)
		{
			string rootPath = Application.dataPath.Replace("/Assets", "");
			string currentPath = rootPath;
			foreach (string subPath in path.Split("/"))
			{
				currentPath = Path.Combine(currentPath, subPath);
				if (!Directory.Exists(currentPath))
				{
					Directory.CreateDirectory(currentPath);
				}
			}

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		public static bool IsFolderExist(string path)
		{
			return Directory.Exists(path);
		}

		public static void WriteAssetSO<T>(T assetSO, string path, string name) where T : ScriptableObject
		{
			assetSO.name = name;
			CreateFolderIfNotExist(path);
			string uniquePath = AssetDatabase.GenerateUniqueAssetPath($"{path}/{name}.asset");
			AssetDatabase.CreateAsset(assetSO, uniquePath);
		}

		public static void RenameAsset(Object asset, string newName)
		{
			AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(asset), newName);
		}

		public static string GetAssetPath(Object obj)
		{
			return AssetDatabase.GetAssetPath(obj);
		}

		public static string GetFileName(Object obj)
		{
			return Path.GetFileNameWithoutExtension(GetAssetPath(obj));
		}

		public static string GetFullPath(Object obj)
		{
			return Path.GetFullPath(GetAssetPath(obj));
		}

		public static string GetDirectoryName(Object obj)
		{
			return Path.GetDirectoryName(GetAssetPath(obj));
		}

		public static string GetLongestCommonPrefix(ICollection<string> paths)
		{
			if (paths == null || paths.Count == 0)
				return null;


			if (paths.Count == 1)
				return paths.First();

			List<string[]> allSplittedPaths = paths.Select(p => p.Split('\\')).ToList();

			int min = allSplittedPaths.Min(a => a.Length);
			int i = 0;
			for (i = 0; i < min; i++)
			{
				string reference = allSplittedPaths[0][i];
				if (allSplittedPaths.Any(a => !string.Equals(a[i], reference, StringComparison.OrdinalIgnoreCase)))
				{
					break;
				}
			}

			return string.Join("\\", allSplittedPaths[0].Take(i));
		}

		public static bool IsSameExtension(Object obj, string extension)
		{
			return Path.GetExtension(GetAssetPath(obj)) == extension;
		}

		public static bool IsAssetsInPath(string[] assets, string path)
		{
			foreach (string asset in assets)
			{
				if (!IsAssetInPath(asset, path)) continue;
				return true;
			}
			return false;
		}

		public static bool IsAssetInPath(string assetPath, string path)
		{
			return assetPath.Contains(path);
		}

		public static int RoundToNextPowerOfTwo(int a)
		{
			int next = CeilToNextPowerOfTwo(a);
			int prev = next >> 1;
			return next - a <= a - prev ? next : prev;
		}

		public static int CeilToNextPowerOfTwo(int number)
		{
			int a = number;
			int powOfTwo = 1;

			while (a > 1)
			{
				a = a >> 1;
				powOfTwo = powOfTwo << 1;
			}
			if (powOfTwo != number)
			{
				powOfTwo = powOfTwo << 1;
			}
			return powOfTwo;
		}

		public static async void WriteFileAsync(byte[] data, string path, Action callback = null)
		{
			await File.WriteAllBytesAsync(path, data);
			callback?.Invoke();
		}

		public static void ApplyScriptDefine(BuildTargetGroup activeBuildTarget, string[] extraScriptDefineList, string[] removeScriptDefineList)
		{

			PlayerSettings.GetScriptingDefineSymbolsForGroup(activeBuildTarget, out string[] currentDefines);
			List<string> defines = new(currentDefines ?? Array.Empty<string>());

			defines.AddRange(extraScriptDefineList);
			defines = defines.Except(removeScriptDefineList).ToList();
			PlayerSettings.SetScriptingDefineSymbolsForGroup(activeBuildTarget, defines.ToArray());
		}

		public static void ApplyPackageChanges(List<string> packageToAddList, List<string> packageDeleteAddList, Action callback)
		{
			EditorCoroutineUtility.StartCoroutineOwnerless(ApplyPackageChangesRoutine(packageToAddList, packageDeleteAddList, callback));
		}

		private static IEnumerator ApplyPackageChangesRoutine(List<string> packageToAddList, List<string> packageDeleteAddList, Action callback)
		{
			packageToAddList ??= new List<string>();
			packageDeleteAddList ??= new List<string>();
			if (packageToAddList.Count == 0 && packageDeleteAddList.Count == 0)
			{
				callback?.Invoke();
				yield break;
			}

			AddAndRemoveRequest installedRequest = Client.AddAndRemove(packageToAddList.ToArray(), packageDeleteAddList.ToArray());
			while (!installedRequest.IsCompleted)
			{
				yield return null;
			}

			if (installedRequest.Status == StatusCode.Failure)
			{
				Debug.LogError($"Failed to add/remove packages: {installedRequest.Error.message}");
			}
			else
			{
				Debug.Log("Successfully applied package changes.");
			}
			callback?.Invoke();
		}
	}
}
#endif