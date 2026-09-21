using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NaughtyAttributes;
using UnityEngine;

namespace Lucielle
{
    public class DatabaseableSO : DescriptionSO
    {
        public string objectId
        {
            get
            {
                if (!isObjectIdValid) SetDefaultId();
				return $"{_objectId} |({GetType().Name})|";
            }
			protected set => _objectId = value;
        }

        public string shortObjectId
        {
            get
            {
                if (!isObjectIdValid) SetDefaultId();
                return _objectId;
            }
        }

        [SerializeField, HideIf(nameof(isIdOverridden))]
        private string _objectId;
        protected bool isObjectIdValid => !string.IsNullOrEmpty(_objectId);
        protected virtual bool isIdOverridden => false;

        protected virtual void SetDefaultId()
        {
            objectId = this.name;
        }

        protected virtual void OnValidate()
        {
            if (!isObjectIdValid) SetDefaultId();
        }

        private static Regex regex = new Regex(@"\|\(.*?\)\|", RegexOptions.Compiled);
        public static bool IsContainClassType(string text)
        {
            return regex.IsMatch(text);
        }

        public static T GetSOAsset<T>(T[] assetList, string id) where T : DatabaseableSO
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (IsContainClassType(id))
            {
                T asset = assetList.FirstOrDefault(asset => asset.objectId == id);
                if (asset != default) return asset;

                // Try to get asset without class type
                string shortObjectId = id.Replace(regex.Match(id).Value, "").Trim();
                return GetSOAsset(assetList, shortObjectId);
            }

            return assetList.FirstOrDefault(asset => asset.shortObjectId == id);
        }

        public static T GetSOAsset<T>(List<T> assetList, string id) where T : DatabaseableSO
        {
            if (string.IsNullOrEmpty(id)) return null;

            if (IsContainClassType(id))
            {
                T asset = assetList.Find(asset => asset.objectId == id);
                if (asset != default) return asset;

                // Try to get asset without class type
                string shortObjectId = id.Replace(regex.Match(id).Value, "").Trim();
                return GetSOAsset(assetList, shortObjectId);
            }
            return assetList.Find(asset => asset.shortObjectId == id);
        }
    }
}
