using UnityEngine;
using UnityEngine.AI;

namespace Lucielle
{
    public class DebugManager
	{
		/// <summary>
		/// Logs a message to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public static void Log(object message) { Debug.Log(message); }

		/// <summary>
		/// Logs a message to the Unity Console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void Log(object message, Object context) { Debug.Log(message, context); }

		/// <summary>
		/// Logs a colored text message to the Unity Console.
		/// The color only works on Editor.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="color">Color of the text message. Only support basic color.</param>
		public static void Log(object message, Color color) { Debug.Log(GetColoredString(message, color)); }

		/// <summary>
		/// Logs a colored text message to the Unity Console.
		/// The color only works on Editor.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="color">Color of the text message. Only support basic color.</param>
		public static void Log(object message, Object context, Color color) { Debug.Log(GetColoredString((string)message, color), context); }

		/// <summary>
		/// Logs a colored text message to the Unity Console.
		/// The color only works on Editor.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="color">Color of the text message. Only support basic color.</param>
		public static void LogColored(object message, Color color) { Debug.Log(GetColoredString(message, color)); }

		/// <summary>
		/// Logs a colored text message to the Unity Console.
		/// The color only works on Editor.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		/// <param name="color">Color of the text message. Only support basic color.</param>
		public static void LogColored(object message, Object context, Color color) { Debug.Log(GetColoredString((string)message, color), context); }


		/// <summary>
		/// A variant of Debug.Log that logs a warning message to the console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public static void LogWarning(object message) { Debug.LogWarning(message); }

		/// <summary>
		/// A variant of Debug.Log that logs a warning message to the console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogWarning(object message, Object context) { Debug.LogWarning(message, context); }

		public static void LogWarning(object headerMessage, object message, Object context)
		{
			Debug.LogWarning($"[{GetColoredString(headerMessage, Color.yellow)}]\n{message}", context);
		}

		/// <summary>
		/// A variant of Debug.Log that logs an error message to the console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		public static void LogError(object message) { Debug.LogError(message); }

		/// <summary>
		/// A variant of Debug.Log that logs an error message to the console.
		/// </summary>
		/// <param name="message">String or object to be converted to string representation for display.</param>
		/// <param name="context">Object to which the message applies.</param>
		public static void LogError(object message, Object context) { Debug.LogError(message, context); }

		public static void LogError(object headerMessage, object message, Object context)
		{
			Debug.LogError($"[{GetColoredString(headerMessage, Color.red)}] : \n{message}", context);
		}

		public static void Log(object headerMessage, object message, Color color, Object context)
		{
			Debug.Log($"[{GetColoredString(headerMessage, color)}] : \n{message}", context);
		}

		public static object GetColoredString(object message, Color color)
		{
#if UNITY_EDITOR
			return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>";
#else
			return message;
#endif
		}

		public static void DrawNavMeshPath(NavMeshAgent navMeshAgent, Color color)
		{
#if UNITY_EDITOR
			if (navMeshAgent == null || !navMeshAgent.hasPath) return;
			Vector3 lastPosition = navMeshAgent.transform.position;
			foreach (Vector3 corner in navMeshAgent.path.corners)
			{
				Debug.DrawLine(lastPosition, corner, color);
				lastPosition = corner;
			}
#endif
		}
	}
}
