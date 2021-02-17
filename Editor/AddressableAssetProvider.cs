using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;

namespace AIR.AddressableRegister.Editor
{
    public class AddressableAssetProvider : IAssetProvider
    {
        public IAssetGroup[] GetAssetIds()
        {
            var addressableAssetGroups = new List<IAssetGroup>();
            var addressableAssetIds = AssetDatabase.FindAssets($"t:{nameof(AddressableAssetGroup)}");
            foreach (var addressableAssetId in addressableAssetIds) {
                var asset = AssetDatabase.LoadAssetAtPath<AddressableAssetGroup>(
                    AssetDatabase.GUIDToAssetPath(addressableAssetId));
                var providedAsset = new ProvidedAddressableAssetGroup(asset);
                addressableAssetGroups.Add(providedAsset);
            }

            return addressableAssetGroups.ToArray();
        }
    }
}