using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Lucielle
{
    public static class Utils
	{
		/// <summary>
		/// Add quote (') to text.
		/// </summary>
		/// <param name="text">The text to be quoted.</param>
		/// <returns></returns>
		public static string QuoteText(string text)
		{
			return "'" + text + "'";
		}

		public static List<string> GetParameterListOnText(string text)
		{
			List<string> list = new List<string>();
			if (string.IsNullOrEmpty(text)) return list;

			foreach (Match match in Regex.Matches(text, @"{\w+}")) list.Add(match.Value);
			return list;
		}

		/// <summary>
		/// Get text with html tag of color as string. Eg. "<color=red>".
		/// </summary>
		/// <param name="message">Text to be colored.</param>
		/// <param name="color">Text color</param>
		/// <returns></returns>
		public static string GetColoredString(object message, Color color)
		{
			return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
		}

		public static string RemoveColorStringTag(string message)
		{
			return Regex.Replace(message, @"</*color[^>]*>", "");
		}

		public static string RemoveIconStringTag(string message)
		{
			return Regex.Replace(message, @"<size[^>]*><sprite name=[^>]*></size>", "");
		}

		/// <summary>
		/// Use this for shorthand to use Ienumerator WaitForSeconds
		/// </summary>
		/// <param name="time">Wait time.</param>
		/// <param name="Callback"></param>
		/// <returns></returns>
		public static IEnumerator WaitRoutine(float time, System.Action Callback)
		{
			yield return new WaitForSeconds(time);
			Callback?.Invoke();
		}

		/// <summary>
		/// Use this for shorthand to use Ienumerator WaitForEndOfFrame
		/// </summary>
		/// <param name="Callback">Callback Function that trigger when routine finish</param>
		/// <returns></returns>
		public static IEnumerator WaitForEndOfFrameRoutine(Action Callback)
		{
			yield return new WaitForEndOfFrame();
			Callback?.Invoke();
		}

		/// <summary>
		/// Return a random within minValueInclusive..maxValueInclusive
		/// </summary>
		/// <param name="minValueInclusive"></param>
		/// <param name="maxValueInclusive"></param>
		/// <returns></returns>
		public static int RandomInclusive(int minValueInclusive, int maxValueInclusive)
		{
			return Random.Range(minValueInclusive, maxValueInclusive + 1);
		}

		public static float RandomInclusive(float minValueInclusive, float maxValueInclusive)
		{
			return Random.Range(minValueInclusive, maxValueInclusive);
		}

		public static bool IsInRange(float value, float lower, float upper, bool isInclusive = false)
		{
			return isInclusive ? lower <= value && value <= upper : lower < value && value < upper;
		}


		/// <summary>
		/// Use this for shorthand to select an item from array
		/// </summary>
		/// <typeparam name="T">Type of array</typeparam>
		/// <param name="items"></param>
		/// <returns>a random item from array</returns>
		public static T SelectRandomItem<T>(params T[] items)
		{
			int randomIndex = Random.Range(0, items.Length);
			return items[randomIndex];
		}

		public static T SelectRandomItem<T>(List<T> items)
		{
			int randomIndex = Random.Range(0, items.Count);
			return items[randomIndex];
		}

		public static T SelectRandomItem<T>(T[] items, float[] weights)
		{
			if (items.Length != weights.Length)
			{
				Debug.LogError($"Length items and weights not matchs. item:{items.Length} - weights:{weights.Length}");
			}

			float totalWeight = weights.Sum();
			float randomWeight = Random.Range(0, totalWeight);
			for (int i = 0; i < items.Length; i++)
			{
				randomWeight -= weights[i];
				if (randomWeight <= 0)
				{
					return items[i];
				}
			}
			return items[0];
		}

		public static List<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
		{
			return listToClone.Select(item => (T)item.Clone()).ToList();
		}

		public static void ToggleGameObjectState(GameObject targetGameObject, bool newState)
		{
			targetGameObject.SetActive(newState);
		}

		/// <summary>
		/// use this function to get Round value same as in excel formula
		/// </summary>
		public static float RoundAwayFromZero(float value)
		{
			return (float)System.Math.Round(value, System.MidpointRounding.AwayFromZero);
		}

		public static string NumberFormating(this float value, bool useSign, bool zeroIsEmpty = false)
		{
			if (value == 0 && zeroIsEmpty) return "";
			string builder = value.ToString(useSign ? "+#;-#;0" : "");
			if (value > -1 && value < 1 && value != 0)
			{
				builder = $"{(value >= 0 ? "+" : "")}{value}";
			}
			if (!useSign)
			{
				builder = builder.Replace("-", string.Empty).Replace("+", string.Empty);
			}
			return builder;
		}

		public static string NumberFormating(this int value, bool useSign)
		{
			return NumberFormating((float)value, useSign);
		}

		public static string GetText(this Enum en)
		{
			return en.ToString().FromCamelCase();
		}

		public static GameObject GetFirstSelectableElement(GameObject gameObject)
		{
			if (gameObject.GetComponent<Selectable>() != null) return gameObject;
			GameObject firstSelectable = null;
			int counter = 0;
			foreach (Selectable selectable in gameObject.GetComponentsInChildren<Selectable>())
			{
				if (selectable.navigation.mode == Navigation.Mode.None) continue;
				if (counter == 0) firstSelectable = selectable.gameObject;
				counter += 1;
			}
			if (counter > 1) DebugManager.LogColored($"{gameObject.name} contains {counter} selectable element", gameObject, Color.yellow);
			return firstSelectable;
		}

		public static float GetAnimatorClipLength(Animator animator)
		{
			AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
			return clipInfo[0].clip.length;
		}

		public static string FromCamelCase(this string text)
		{
			//Strip leading "_" character
			text = Regex.Replace(text, "^_", "").Trim();
			//Add a space between each lower case character and upper case character
			text = Regex.Replace(text, "([a-z])([A-Z])", "$1 $2").Trim();
			//Add a space between 2 upper case characters when the second one is followed by a lower space character
			text = Regex.Replace(text, "([A-Z])([A-Z][a-z])", "$1 $2").Trim();
			return text;
		}

		public static string GetTimeNow(string format = "dd/MM/yyyy HH:mm:ss")
		{
			return GetTimeString(DateTime.Now, format);
		}

		public static CultureInfo enCulture => new CultureInfo("en-US");
		public static string GetTimeString(DateTime dateTime, string format = "dd/MM/yyyy HH:mm:ss")
		{
#if UNITY_ANDROID
			if (dateTime == new DateTime()) dateTime = DateTime.UtcNow;
#endif
			return dateTime.ToString(format, enCulture);
		}

		public static bool ValidateTime(string time, string format = "dd/MM/yyyy HH:mm:ss")
		{
			if (string.IsNullOrEmpty(time)) return false;
			return DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime result);
		}

		public static DateTime GetDateTime(string time, string format = "dd/MM/yyyy HH:mm:ss")
		{
			if (string.IsNullOrEmpty(time)) return DateTime.Now;
			bool success = DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out DateTime result);
			if (!success) success = DateTime.TryParse(time, out result);
			if (!success) success = DateTime.TryParseExact(time, "dd/MM/yyyy HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result);
			if (!success) success = DateTime.TryParseExact(time, "dd.MM.yyyy HH.mm.ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result);
			if (!success) success = DateTime.TryParseExact(time, "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out result);
			return result;
		}

		public static string GetValueWithPostfix(float value, string postfix = "")
		{
			string sb = "";
			if (value == 0 && postfix == "") return sb;

			if (value > 0) sb += "+";

			sb += value.ToString() + postfix + " ";
			return sb;
		}

		public static void ChangeLayerRecursively(GameObject gameObject, string layerName)
		{
			ChangeLayerRecursively(gameObject, LayerMask.NameToLayer(layerName));
		}

		public static void ChangeLayerRecursively(GameObject gameObject, int layer)
		{
			gameObject.layer = layer;
			foreach (Transform child in gameObject.transform)
			{
				ChangeLayerRecursively(child.gameObject, layer);
			}
		}

		public static Color Clone(this Color color, float alpha)
		{
			return new Color(color.r, color.g, color.b, alpha);
		}

		public static Color Clone(this Color color)
		{
			return new Color(color.r, color.g, color.b, color.a);
		}

		public static void SetAlpha(this SpriteRenderer spriteRenderer, float alpha)
		{
			spriteRenderer.color = spriteRenderer.color.Clone(alpha);
		}

		public enum Orientation
		{
			Horizontal,
			Vertical
		}

		public enum DiagonalDirection
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight,
		}

		public enum OrdinalDirection
		{
			North,
			NorthEast,
			East,
			SouthEast,
			South,
			SouthWest,
			West,
			NorthWest,
		}

		public static string ConvertToStringTime(int seconds, string timeFormat = "{0:00}:{1:00}:{2:00}")
		{
			int minute = seconds / 60;
			int hour = minute / 60;
			return string.Format(timeFormat, hour, minute % 60, seconds % 60);
		}

		public enum NumberOperation
		{
			LowerThan,
			Equals,
			MoreThan,
			LowerThanOrEqual,
			NotEqual,
			MoreThanOrEqual
		}

		public static bool EvaluateNumberOperation(float a, NumberOperation operation, float b)
		{
			return operation switch
			{
				NumberOperation.LowerThan => a < b,
				NumberOperation.Equals => a == b,
				NumberOperation.MoreThan => a > b,
				NumberOperation.LowerThanOrEqual => a <= b,
				NumberOperation.NotEqual => a != b,
				NumberOperation.MoreThanOrEqual => a >= b,
				_ => false,
			};
		}

		public enum NumberOperationBetween
		{
			[InspectorName("Full Inclusive")] In_In,
			[InspectorName("Semi (In - Ex)")] In_Ex,
			[InspectorName("Semi (Ex - In)")] Ex_In,
			[InspectorName("Full Exclusive")] Ex_Ex,
		}

		public static bool EvaluateBetweenOperation(NumberOperationBetween operation, float value, float lowerBound, float upperBound)
		{
			return operation switch
			{
				NumberOperationBetween.In_In => (lowerBound <= value && value <= upperBound),
				NumberOperationBetween.In_Ex => (lowerBound <= value && value < upperBound),
				NumberOperationBetween.Ex_In => (lowerBound < value && value <= upperBound),
				NumberOperationBetween.Ex_Ex => (lowerBound < value && value < upperBound),
				_ => false,
			};
		}

		public enum BoolOperation
		{
			OR,
			AND
		}

		public static bool EvaluateBoolOperation(BoolOperation operation, bool a, bool b)
		{
			return operation switch
			{
				BoolOperation.OR => a || b,
				BoolOperation.AND => a && b,
				_ => false,
			};
		}

		public static float CalculateSignedAngle(Transform centerTransform, Transform targetTransform)
		{
			// Get the positions of centerTransform and targetTransform in the ZX plane (ignore their Y values).
			Vector3 centerPosition = new Vector3(centerTransform.position.x, 0f, centerTransform.position.z);
			Vector3 targetPosition = new Vector3(targetTransform.position.x, 0f, targetTransform.position.z);

			// Calculate the direction from centerTransform to targetTransform in the ZX plane.
			Vector3 direction = targetPosition - centerPosition;

			// Calculate the signed angle between the direction vector and the forward vector of centerTransform.
			float signedAngle = Vector3.SignedAngle(centerTransform.forward, direction, Vector3.up);

			return signedAngle;
		}

		public static float CalculateSignedAngle(GameObject centerObject, GameObject targetObject)
		{
			return CalculateSignedAngle(centerObject.transform, targetObject.transform);
		}

		public static bool CheckContains(List<string> list, params string[] items)
		{
			foreach (string item in items)
			{
				if (list.Contains(item)) return true;
			}
			return false;
		}

		public static bool IsSameResolution(Resolution a, Resolution b)
		{
			return a.width == b.width && a.height == b.height && a.refreshRate == b.refreshRate;
		}

		public static Vector3 GetRandomPositionInRect(Rect spawnAreaRect, Rect spawnedObjectRect)
		{
			float minX = spawnAreaRect.min.x;
			float maxX = spawnAreaRect.max.x;
			float minY = spawnAreaRect.min.y;
			float maxY = spawnAreaRect.max.y;

			float objWidth = spawnedObjectRect.width;
			float objHeight = spawnedObjectRect.height;

			float randomX = UnityEngine.Random.Range(minX + objWidth / 2, maxX - objWidth / 2);
			float randomY = UnityEngine.Random.Range(minY + objHeight / 2, maxY - objHeight / 2);

			return new Vector2(randomX, randomY);
		}

		public static void Swap<T>(ref T a, ref T b)
		{
			T tmp = a;
			a = b;
			b = tmp;
		}

#if UNITY_EDITOR
		public static List<T> LoadAssets<T>(string assetPath, string filter = "") where T : UnityEngine.Object
		{
			bool isUseFilter = !string.IsNullOrEmpty(filter);
			string[] guids = UnityEditor.AssetDatabase.FindAssets(isUseFilter ? $"t:{filter}" : "", new string[] { assetPath });
			List<T> objects = new List<T>();
			for (int i = 0; i < guids.Length; i++)
			{
				string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
				T obj = LoadAsset<T>(path);

				if (obj is not T) continue;
				objects.Add(obj);
			}
			return objects;
		}

		public static T LoadAsset<T>(string assetPath) where T : UnityEngine.Object
		{
			return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
		}
#endif
	}

	[System.Serializable]
	public class FloatArray
	{
		public int Length
		{
			get { return values.Length; }
		}
		public float[] values;
	}

	[System.Serializable]
	public class IntArray
	{
		public int Length
		{
			get { return values.Length; }
		}
		public int[] values;
	}

	[System.Serializable]
	public class StringArray
	{
		public int Length
		{
			get { return values.Length; }
		}
		public string[] values;
	}

	[Serializable]
	public class Pair<T, V>
	{
		public T key;
		public V value;

		public void Deconstruct(out T a, out V b)
		{
			a = key;
			b = value;
		}
	}

	[Serializable]
	public class Pair<T, U, V>
	{
		public T key1;
		public U key2;
		public V value;

		public void Deconstruct(out T a, out U b, out V c)
		{
			a = key1;
			b = key2;
			c = value;
		}
	}

	[Serializable]
	public class Pair<T, U, V, W>
	{
		public T key1;
		public U key2;
		public V key3;
		public W value;

		public void Deconstruct(out T a, out U b, out V c, out W d)
		{
			a = key1;
			b = key2;
			c = key3;
			d = value;
		}
	}

	[Serializable]
	public class Range<T>
	{
		public T lower;
		public T upper;

		public Range(T lower, T upper)
		{
			this.lower = lower;
			this.upper = upper;
		}
	}

	[Serializable]
	public class IntRange : Range<int>
	{
		public bool inclusive;
		public IntRange(int lower, int upper) : base(lower, upper) { }

		public int GetRandomInt()
		{
			return Random.Range(lower, inclusive ? upper + 1 : upper);
		}

		public float GetRelativeDeviation(int value)
		{
			float median = (upper - lower + 1) / 2;
			return (value - median) / median;
		}
	}

	[Serializable]
	public class FloatRange : Range<float>
	{
		public bool inclusive;
		public FloatRange(float lower, float upper) : base(lower, upper) { }

		public float GetRandomFloat()
		{
			return Random.Range(lower, inclusive ? upper + 1 : upper);
		}
	}

	[Serializable]
	public class Timer
	{
		public float duration;
		
		public bool isRunning { get; private set; }
		public float time { get; private set; }

		public Action<float> onUpdate;
		public Action onFinish;

		public Timer(float duration)
		{
			this.duration = duration;			
			time = 0f;
			isRunning = false;
		}

		public void Start(Action<float> onUpdate = null, Action onFinish = null)
		{
			this.onUpdate = onUpdate;
			this.onFinish = onFinish;
			
			isRunning = true;
			time = 0f;
		}

		public void Stop()
		{
			isRunning = false;
			time = 0f;
		}

		public void Update(float deltaTime)
		{
			if (!isRunning) return;
			time += deltaTime;
			onUpdate?.Invoke(time);

			if (time >= duration)
			{
				onFinish?.Invoke();
				Stop();
			}
		}
	}

	public class MultiDimensionArray : PropertyAttribute { }

	public static class ExtensionMethods
	{
		public static Bounds OrthographicBounds(this Camera camera)
		{
			if (!camera.orthographic)
			{
				Debug.Log(string.Format("The camera {0} is not Orthographic!", camera.name), camera);
				return new Bounds();
			}

			Transform camTrans = camera.transform;
			float scaleWidthFactor = Screen.currentResolution.width / (float)Screen.currentResolution.height;
			float height = camera.orthographicSize * 2f;
			float width = height * scaleWidthFactor;
			float viewDepth = camera.farClipPlane - camera.nearClipPlane;

			return new Bounds(new Vector3(camTrans.position.x, camTrans.position.y, camTrans.position.z), new Vector3(width, height, viewDepth));
		}

		public static Rect OrthographicRect(this Camera camera)
		{
			if (!camera.orthographic)
			{
				Debug.Log(string.Format("The camera {0} is not Orthographic!", camera.name), camera);
				return new Rect();
			}

			Transform camTrans = camera.transform;
			float scaleWidthFactor = Screen.currentResolution.width / (float)Screen.currentResolution.height;
			float height = camera.orthographicSize * 2f;
			float width = height * scaleWidthFactor;

			return new Rect(new Vector2(camTrans.position.x - (width * 0.5f), camTrans.position.y - (height * 0.5f)), new Vector2(width, height));
		}

		public static void SetRectTransformSameAs(this RectTransform original, RectTransform target)
		{
			original.sizeDelta = target.sizeDelta;
			original.pivot = target.pivot;
			original.anchorMax = target.anchorMax;
			original.anchorMin = target.anchorMin;

			original.anchoredPosition = target.anchoredPosition;
			original.localScale = target.localScale;
		}

		public static void ResetTransformation(this Transform trans)
		{
			trans.position = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = new Vector3(1, 1, 1);
		}

		public static void SetAnchoredPosition(this RectTransform rectTransform, Vector2 position)
		{
			rectTransform.anchoredPosition = position;
		}

		/// <summary>
		/// Get Hierarchy Path from root GameObject
		/// </summary>
		/// <param name="current"></param>
		/// <returns></returns>
		public static string GetHierarchyPath(this Transform current)
		{
			if (current.parent == null)
				return "/" + current.name;
			return current.parent.GetHierarchyPath() + "/" + current.name;
		}

		public static string GetHierarchyPath(this Component component)
		{
			return component.transform.GetHierarchyPath() + "/" + component.GetType().ToString();
		}

		public static string ColoredText(this string component, Color color)
		{
			return Utils.GetColoredString(component, color).ToString();
		}
		public static string ColorRed(this string component)
		{
			return Utils.GetColoredString(component, Color.red).ToString();
		}
		public static string ColorBlue(this string component)
		{
			return Utils.GetColoredString(component, Color.blue).ToString();
		}
		public static string ColorYellow(this string component)
		{
			return Utils.GetColoredString(component, Color.yellow).ToString();
		}
		public static string ColorGreen(this string component)
		{
			return Utils.GetColoredString(component, Color.green).ToString();
		}

		public static void SetInteractable(this Button button, bool interactable)
		{
			button.interactable = interactable;
		}

		public static void SetSprite(this Image image, Sprite sprite)
		{
			image.sprite = sprite;
		}

		public static void Swap<T>(this IList<T> list, int i, int j)
		{
			T temporary = list[i];
			list[i] = list[j];
			list[j] = temporary;
		}

		/// <summary>
		/// Shuffles a list randomly
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		public static List<T> Shuffle<T>(this IList<T> list)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list.Swap(i, Random.Range(i, list.Count));
			}
			return new List<T>(list);
		}

		public static bool IsTextOverflow(this TMPro.TextMeshProUGUI textMeshPro)
		{
			return textMeshPro.isTextOverflowing;
		}

		public static string GetTextOverflow(this TMPro.TextMeshProUGUI textMeshPro)
		{
			if (!textMeshPro.IsTextOverflow()) return string.Empty;
			return textMeshPro.text.Substring(textMeshPro.firstOverflowCharacterIndex);
		}

		public static string GetFitText(this TMPro.TextMeshProUGUI textMeshPro)
		{
			if (!textMeshPro.IsTextOverflow()) return textMeshPro.text;
			return textMeshPro.text.Substring(0, textMeshPro.firstOverflowCharacterIndex);
		}
		
		public static void SetNavigationNone(this Selectable selectable)
		{
			Navigation navigation = new()
			{
				mode = Navigation.Mode.None
			};
			selectable.navigation = navigation;
		}

		public static void SetNavigationDefault(this Selectable selectable)
		{
			selectable.navigation = Navigation.defaultNavigation;
		}
	}
}
