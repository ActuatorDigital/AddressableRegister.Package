using System.Collections.Generic;
using System.Linq;
using UnityEditor.AddressableAssets.Settings;

namespace AIR.AddressableRegister.Editor
{
    public interface IAssetProvider
    {
        IAssetGroup[] GetAssetIds();
    }

    public class ProvidedAddressableAssetGroup : IAssetGroup
    {
        private AddressableAssetGroup _addressableAssetGroup;
        public ProvidedAddressableAssetGroup(AddressableAssetGroup addressableAssetGroup) => 
            _addressableAssetGroup = addressableAssetGroup;

        public string Name => _addressableAssetGroup.Name;
        public IEnumerable<IAssetEntry> Entries => _addressableAssetGroup.entries
            .Select(e => new ProvidedAssetEntry(e));
    }

    public class ProvidedAssetEntry : IAssetEntry
    {
        private AddressableAssetEntry _addressableAssetEntry;

        public ProvidedAssetEntry(AddressableAssetEntry addressableAssetEntry) =>
            _addressableAssetEntry = addressableAssetEntry;

        public string Address {
            get => _addressableAssetEntry.address;
            set => _addressableAssetEntry.address = value;
        }
    }

    public interface IAssetGroup
    {
        string Name { get; }
        IEnumerable<IAssetEntry> Entries { get; }
    }

    public interface IAssetEntry
    {
        string Address { get; set; }
    }
}