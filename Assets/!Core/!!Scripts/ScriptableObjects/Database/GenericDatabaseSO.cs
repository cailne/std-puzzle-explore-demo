using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Database/Generic")]
    public class GenericDatabaseSO : BaseDatabaseSO
    {
        public List<DatabaseableSO> objectAssetList = new();
        public string objectsPath = "Assets/!Core/ScriptableObjects/";
        public DatabaseableSO objectTypeSO;

        public override List<DatabaseableSO> GetObjectAssetList()
        {
            return objectAssetList;
        }

        public List<T> GetObjectAssetList<T>() where T : DatabaseableSO
        {
            return objectAssetList.OfType<T>().ToList();
        }

#if UNITY_EDITOR
        [Button]
        private void LoadAllAssetOnPaths()
        {
            string filter = objectTypeSO != null ? objectTypeSO.GetType().Name : string.Empty;
            List<DatabaseableSO> assets = EditorUtils.LoadAssets<DatabaseableSO>(objectsPath, filter);

            objectAssetList = objectAssetList.Where(asset => objectTypeSO == null || asset.GetType() == objectTypeSO.GetType()).ToList();
            bool hasChange = objectAssetList.Count != assets.Count || objectAssetList.Except(assets).Count() > 0;
            if (hasChange)
            {
                objectAssetList.Clear();
                objectAssetList.AddRange(assets);

                EditorUtils.MarkDirty(this);
                DebugManager.Log($"Loaded {objectAssetList.Count} {filter} assets on {name}", Color.green);
            }
        }
#endif
    }
}


