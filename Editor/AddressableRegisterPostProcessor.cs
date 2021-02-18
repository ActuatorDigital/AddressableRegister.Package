using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine.PlayerLoop;

namespace AIR.AddressableRegister.Editor
{
    [InitializeOnLoad]
    public class AddressableRegisterPostProcessor
    {
        private static readonly AddressableRegisterTranscriber Author =
            new AddressableRegisterTranscriber();
        private static readonly AddressableRegisterLocator Finder =
            new AddressableRegisterLocator();
        private static readonly Dictionary<string, FileSystemWatcher> Watchers =
            new Dictionary<string, FileSystemWatcher>();
        private static readonly Queue<Action> ReAuthorActions
            = new Queue<Action>();

        static AddressableRegisterPostProcessor()
        {
            WatchAddressableChanges();
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            while (ReAuthorActions.Any())
                ReAuthorActions.Dequeue()?.Invoke();
        }

        private static void WatchAddressableChanges()
        {
            Watchers.Clear();
            string filter = $"t:{nameof(AddressableAssetGroup)}";
            var addressableAssetIds = AssetDatabase.FindAssets(filter);
            foreach (var assetId in addressableAssetIds) {
                var assetPath = AssetDatabase.GUIDToAssetPath(assetId);

                var fileInfo = new FileInfo(assetPath);
                if (fileInfo.Directory == null) continue;

                var fullPath = fileInfo.FullName;
                if (Watchers.ContainsKey(fullPath)) continue;

                var watcher = new FileSystemWatcher {
                    EnableRaisingEvents = true,
                    Path = Path.GetDirectoryName(fullPath),
                    Filter = Path.GetFileName(fullPath),
                };

                watcher.Changed += QueueReAuthor;
                // watcher.Renamed += QueueReAuthor;
                // watcher.Deleted += QueueReAuthor;

                Watchers.Add(fullPath, watcher);
            }
        }

        private static void QueueReAuthor(object sender, FileSystemEventArgs e)
        {
            var addressableRegisterPath = Finder.FindOutputFile();
            ReAuthorActions.Enqueue(() => Author.WriteAddressableRegisterTo(addressableRegisterPath));
        }
    }
}