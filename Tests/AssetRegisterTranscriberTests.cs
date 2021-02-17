using System;
using AIR.AddressableRegister;
using AIR.AddressableRegister.Editor;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class AddressableRegisterTranscriberTests
{
    [Test]
    public void AuthorCode_WheneverRun_MarksClassWithAssetProviderAttribute()
    {
        // Arrange
        var transcriber = new AddressableRegisterTranscriber();
        
        // Act
        var code = transcriber.AuthorCode("Test");
        
        // Assert
        var attributeStr = $"[{nameof(AddressableRegisterAttribute)}]";
        attributeStr = attributeStr.Replace("Attribute", String.Empty);
        Assert.That(code.Contains(attributeStr));
    }

    [Test]
    [TestCase("TestClassName")]
    [TestCase("DifferentClassName")]
    public void AuthorCode_WithClassName_CreatesClassForName(string className)
    {
        // Arrange
        var transcriber = new AddressableRegisterTranscriber();
        
        // Act
        var code = transcriber.AuthorCode(className);

        // Assert
        Assert.That(code.Contains($"public class {className}"));
    }

    [Test]
    [TestCase("GroupName", "AssetName")]
    [TestCase("Units", "HumanRouge")]
    public void AuthorCode_WithAssetGroup_CreatesEnumForGroup(string groupName, string assetName)
    {
        // Arrange
        var mockAssetProvider = SubstituteAssetProvider(groupName, assetName);
        var transcriber = new AddressableRegisterTranscriber(mockAssetProvider);
        
        // Act
        var code = transcriber.AuthorCode("AddressableRegister");
        
        // Assert
        Assert.That(code.Contains($"public enum {groupName}"));
        Assert.That(code.Contains($"{assetName} = 0,"));
    }

    private IAssetProvider SubstituteAssetProvider(string groupName, string assetname)
    {
        var mockAssetProvider = Substitute.For<IAssetProvider>();
        var mockGroup = Substitute.For<IAssetGroup>();
        mockGroup.Name.Returns(groupName);
        var assetEntry = Substitute.For<IAssetEntry>();
        assetEntry.Address.Returns($"{groupName}/{assetname}");
        mockGroup.Entries.Returns(new[] {assetEntry});
        mockAssetProvider.GetAssetIds().Returns(new[] {mockGroup});
        return mockAssetProvider;
    }
}