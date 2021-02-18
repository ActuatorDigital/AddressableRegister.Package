// Copyright (c) AIR Pty Ltd. All rights reserved.

using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace AIR.AddressableRegister.Editor
{
    public class AddressableRegisterTranscriber
    {
        private readonly IAssetProvider _assetProvider;

        public AddressableRegisterTranscriber(IAssetProvider assetProvider = null) =>
            _assetProvider = assetProvider ?? new AddressableAssetProvider();

        public void WriteAddressableRegisterTo(string outputFile)
        {
            string codeStr;
            EditorUtility.DisplayProgressBar("GenerateAddressableConsts", "Gathering all AddressableAssetGroups.", 0);
            string className = Path.GetFileNameWithoutExtension(outputFile);

            try {
                codeStr = AuthorCode(className);
            } catch (System.Exception) {
                EditorUtility.ClearProgressBar();
                throw;
            }

            var outputFileDir = Path.GetDirectoryName(outputFile);
            if (outputFileDir != null) {
                Directory.CreateDirectory(outputFileDir);
                File.WriteAllText(outputFile, codeStr);
                AssetDatabase.ImportAsset(outputFile);
            }

            EditorUtility.ClearProgressBar();
        }

        public string AuthorCode(string className)
        {
            var author = new AddressableRegisterAuthor(className);
            var assetIDs = _assetProvider.GetAssetIds();

            foreach (var asset in assetIDs) {
                if (asset.Name == "Built In Data" || asset.Name == "Default Local Group") continue;
                if (!author.ValidateEnumName(asset.Name)) {
                    Debug.LogError($"Addressable Asset Group name {asset.Name} could not be converted to an enum.");
                    continue;
                }

                var orderedEntries = asset.Entries
                    .OrderBy(x => x.Address);
                foreach (var addressableItem in orderedEntries)
                    author.AddEntry(addressableItem, asset.Name);
            }

            return author.Generate();
        }
    }
}