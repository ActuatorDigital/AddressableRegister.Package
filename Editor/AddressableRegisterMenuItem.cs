using System;
using UnityEditor;
using UnityEngine;

namespace AIR.AddressableRegister.Editor
{
    public static class AddressableRegisterMenuItem
    {
        [MenuItem("Window/Asset Management/Addressables/Update Register")]
        public static void RegenerateAssetIndex()
        {
            var locator = new AddressableRegisterLocator();
            var outputFileName = locator.FindOutputFile();

            var author = new AddressableRegisterTranscriber();
            author.GenerateAddressableConsts(outputFileName);
            
            Debug.Log("Updated assets registered in " + outputFileName);
        }
    }
}