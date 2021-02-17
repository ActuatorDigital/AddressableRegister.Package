using System;
using System.Linq;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AIR.AddressableRegister
{
    public static class AddressableRegister
    {
        public static AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(Enum assetId)
        {
            // TODO: Clear and re-index all of the generated types, then use assetId for fast lookup.
            #if UNITY_EDITOR
            var assetType = assetId.GetType().DeclaringType;
            Debug.Assert(
                 Attribute.GetCustomAttributes(assetType).Any( a => a is AddressableRegisterAttribute), 
                "Asset enum was not a known indexed value. " +
                "Use generated enum values from the AddressableRegister.");
            #endif
            
            string group = assetId.GetType().Name;
            string name = assetId.ToString();

            string resourceAddress = $"{group}/{name}";
            return UnityEngine.AddressableAssets.Addressables
                .LoadAssetAsync<TObject>(resourceAddress);
        }
    }
}