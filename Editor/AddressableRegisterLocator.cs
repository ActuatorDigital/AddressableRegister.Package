using System;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;

namespace AIR.AddressableRegister.Editor
{
    internal class AddressableRegisterLocator : IAddressableRegisterLocator
    {
        public string FindOutputFile()
        {
            var idIndexClassName = FindIdIndexClassName();
            var idIndexFileName = FindIdIndexFileName(idIndexClassName);

            return idIndexFileName;
        }
        
        private string FindIdIndexFileName(string idIndexClassName)
        {
            var classFileName = idIndexClassName + ".cs";
            var assets = AssetDatabase.GetAllAssetPaths()
                .Where(a => new FileInfo(a).Name == classFileName)
                .ToArray()
                .FirstOrDefault();

            if (assets == null) {
                var errMsg = $"File {idIndexClassName} not found in project.";
                throw new FileNotFoundException(errMsg);
            }

            if (!assets.Any()) {
                var msg = 
                    "More than one Id Index file found.\n" +
                    $"({string.Join(",", assets)})";
                throw new DuplicateNameException(msg);                
            }

            return assets;
        }
        
        private string FindIdIndexClassName()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                var types = assembly.GetTypes();
                foreach (var type in types) {
                    foreach (var attr in Attribute.GetCustomAttributes(type)) {
                        if (attr is AddressableRegisterAttribute)
                            return type.Name;
                    }
                }
            }

            throw new FileNotFoundException($"No types in assembly have attribute {nameof(AddressableRegisterAttribute)}.");
        }
    }
}