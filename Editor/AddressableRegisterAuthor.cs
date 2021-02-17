// Copyright (c) AIR Pty Ltd. All rights reserved.

using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.Assertions;

namespace AIR.AddressableRegister.Editor
{
    public class AddressableRegisterAuthor
    {
        const string ADDRESSABLES_CONSTS_TEMPLATE = @"// Copyright (c) AIR Pty Ltd. All rights reserved.

/*This file is auto generated, no not make manual edits.*/

using {3};

[{2}]
public class {1} {{
{0}
}}";

        private const string ADDRESSABLES_ENUM_BODY = @"{0}public enum {1}
{0}{{{2}
{0}}}";

        private const string ADDRESSABLES_ENUM_VALUE_MIDDLE_LINE = @"{0}{0}{1} = {2},";
        private const string ADDRESS_PARTS_MSG = "Addressable names cannot have more than 2 parts to them. Only one slash.";

        private readonly string _spacing;
        private readonly string _staticClassName;
        private int _currentEnumEntryCount;
        readonly Dictionary<string, List<string>> _itemsToGenerate 
            = new Dictionary<string, List<string>>();
        
        public AddressableRegisterAuthor(string staticClassName, string leadingSpacing = "    ")
        {
            _staticClassName = staticClassName;
            _spacing = leadingSpacing;
        }

        internal string Generate()
        {
            var innerEnumContentSb = new StringBuilder();

            _currentEnumEntryCount = 0;
            foreach (var item in _itemsToGenerate)
            {
                innerEnumContentSb.AppendLine();
                innerEnumContentSb.AppendLine(GenerateEnum(item.Key, item.Value));
            }

            return string.Format(
                ADDRESSABLES_CONSTS_TEMPLATE,
                innerEnumContentSb,
                _staticClassName,
                nameof(AddressableRegisterAttribute).Replace("Attribute", string.Empty),
                typeof(AddressableRegisterAttribute).Namespace
            );
        }

        private string GenerateEnum(string enumName, List<string> enumContents)
        {
            var enumSb = new StringBuilder();
            foreach (var enumItem in enumContents)
            {
                enumSb.AppendLine();
                enumSb.AppendFormat(
                    ADDRESSABLES_ENUM_VALUE_MIDDLE_LINE, 
                    _spacing, 
                    enumItem,
                    _currentEnumEntryCount.ToString());

                _currentEnumEntryCount++;
            }

            return string.Format(ADDRESSABLES_ENUM_BODY, _spacing, enumName, enumSb.ToString());
        }

        internal void AddEntry(IAssetEntry entry, string enumName)
        {
         
            var provider = CodeDomProvider.CreateProvider("C#");
            bool IsValidIdentifier(string id) => provider.IsValidIdentifier(id);
            
            var addressableNameSlices = entry.Address.Split('/', '\\');
            var itemGroupName = addressableNameSlices.First();

            Assert.IsTrue(addressableNameSlices.Length < 3, ADDRESS_PARTS_MSG);

            if (addressableNameSlices.Length == 1) {
                Debug.LogWarning(
                    $"Addressable item did not have a group name, this is attempting to be automatically corrected. Was {entry.Address} will have {enumName} prepended.");
                itemGroupName = enumName;
            }

            if (itemGroupName != enumName) {
                Debug.LogWarning(
                    $"Addressable item groupname did not match, this is attempting to be automatically corrected. Was {itemGroupName} should be {enumName}.");
                itemGroupName = enumName;
            }

            var itemName = addressableNameSlices.Last();
            if (!IsValidIdentifier(itemName)) {
                // remove spaces and dots
                var safeItemName = string.Join("_", itemName.Split(' ', '.'));

                Assert.IsTrue(IsValidIdentifier(safeItemName),
                    $"Addressable {itemName} did not have a valid identifier for a name and could not be automatically correctled.");
                Debug.LogWarning(
                    $"Addressable item name is not a valid indentifier. Was {itemName} will be {safeItemName}.");
                itemName = safeItemName;
            }

            entry.Address = $"{itemGroupName}/{itemName}";
            AddEntry(itemGroupName, itemName);
        }

        private void AddEntry(string group, string itemName)
        {
            if (_itemsToGenerate.TryGetValue(group, out var list)) {
                list.Add(itemName);
                return;
            }

            var newList = new List<string>();
            _itemsToGenerate.Add(@group, newList);
            newList.Add(itemName);
        }

        public bool ValidateEnumName(string enumName)
        {
            var isValid = CodeDomProvider.CreateProvider("C#").IsValidIdentifier(enumName);
            return isValid;
        }
    }
}