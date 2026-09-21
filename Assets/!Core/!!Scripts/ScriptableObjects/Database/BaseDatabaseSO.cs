using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public abstract class BaseDatabaseSO : ScriptableObject
    {
        public abstract List<DatabaseableSO> GetObjectAssetList();

        public T GetObjectAsset<T>(string id) where T : DatabaseableSO
        {
            return DatabaseableSO.GetSOAsset(GetObjectAssetList(), id) as T;
        }

        public T GetObjectAsset<T>(int index) where T : DatabaseableSO
        {
            if (index < 0 || index >= GetObjectAssetList().Count)
            {
                DebugManager.LogWarning($"Index {index} is out of range for objectAssetList.");
                return null;
            }
            return GetObjectAssetList()[index] as T;
        }

#if UNITY_EDITOR
        [Button]
        protected void ValidateObjectId(bool isWriteAsset = false)
        {
            Dictionary<string, List<DatabaseableSO>> objectIdDictionary;
            CheckEmptyObjectID(GetObjectAssetList(), out objectIdDictionary);
            CheckDuplicateObjectID(objectIdDictionary);
            if (isWriteAsset) WriteIdOnAsset(GetObjectAssetList());
        }

        protected void WriteIdOnAsset(List<DatabaseableSO> scriptableObjects)
        {
            foreach (DatabaseableSO scriptableObject in scriptableObjects)
            {
                UnityEditor.EditorUtility.SetDirty(scriptableObject);
            }
            UnityEditor.AssetDatabase.SaveAssets();
        }

        protected void CheckEmptyObjectID(List<DatabaseableSO> scriptableObjects, out Dictionary<string, List<DatabaseableSO>> objectIdDictionary)
        {
            objectIdDictionary = new Dictionary<string, List<DatabaseableSO>>();
            foreach (DatabaseableSO scriptableObject in scriptableObjects)
            {
                string id = scriptableObject.objectId;
                if (string.IsNullOrEmpty(scriptableObject.objectId))
                {
                    DebugManager.Log($"{Utils.GetColoredString("MISSING OBJECT ID:", Color.red)} {Utils.GetColoredString(scriptableObject.name, Color.cyan)} has no objectId", scriptableObject);
                }
                else
                {
                    if (!objectIdDictionary.ContainsKey(id)) objectIdDictionary.Add(id, new List<DatabaseableSO>());
                    objectIdDictionary[id].Add(scriptableObject);
                }
            }
        }

        protected void CheckDuplicateObjectID(Dictionary<string, List<DatabaseableSO>> objectIdDictionary)
        {
            foreach (KeyValuePair<string, List<DatabaseableSO>> idObjectPair in objectIdDictionary)
            {
                if (idObjectPair.Value.Count <= 1) continue;
                foreach (DatabaseableSO scriptableObject in idObjectPair.Value)
                {
                    DebugManager.Log($"{Utils.GetColoredString("DUPLICATE OBJECT ID:", Color.red)} {Utils.GetColoredString(scriptableObject.name, Color.cyan)} with id : {scriptableObject.objectId}", scriptableObject);
                }
            }
        }
#endif
    }
}
