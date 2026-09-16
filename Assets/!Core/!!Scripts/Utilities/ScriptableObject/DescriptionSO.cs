using UnityEngine;

namespace Lucielle
{
	public class DescriptionSO : ScriptableObject
	{
#if UNITY_EDITOR
		[SerializeField, TextArea] private string developmentDesc;
#endif
	}
}