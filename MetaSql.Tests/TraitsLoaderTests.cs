using MetaSql.Core;

public sealed class TraitsLoaderTests
{
    [Fact]
    public void NoTraitsFileReturnsEmptyDictionary()
    {
        var traits = new TraitsFileLoader().Load(null);

        Assert.Empty(traits);
    }

    [Fact]
    public void ValidTraitsFileLoadsValues()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var path = Path.Combine(root, "traits.json");
            File.WriteAllText(
                path,
                """
                {
                  "dbo.PIT_CustomerSnapshot": {
                    "stateClass": "replaceable",
                    "autoPolicy": "additive-plus-empty-drop",
                    "validationProfile": "pit",
                    "dependencyGroup": "customer"
                  }
                }
                """);

            var traits = new TraitsFileLoader().Load(path);

            var item = Assert.Single(traits);
            Assert.Equal("dbo.PIT_CustomerSnapshot", item.Key);
            Assert.Equal(SqlObjectStateClass.Replaceable, item.Value.StateClass);
            Assert.Equal(SqlObjectAutoPolicy.AdditivePlusEmptyDrop, item.Value.AutoPolicy);
            Assert.Equal("pit", item.Value.ValidationProfile);
            Assert.Equal("customer", item.Value.DependencyGroup);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MissingTraitsFileFails()
    {
        var path = Path.Combine(TestSupport.CreateTempDirectory(), "missing.json");

        var exception = Assert.Throws<InvalidOperationException>(() => new TraitsFileLoader().Load(path));

        Assert.Contains("does not exist", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UnsupportedStateClassFails()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var path = Path.Combine(root, "traits.json");
            File.WriteAllText(
                path,
                """
                {
                  "dbo.PIT_CustomerSnapshot": {
                    "stateClass": "mystery"
                  }
                }
                """);

            var exception = Assert.Throws<InvalidOperationException>(() => new TraitsFileLoader().Load(path));

            Assert.Contains("unsupported stateClass", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MissingObjectTraitDoesNotUnlockDestructiveBehavior()
    {
        var traits = new TraitsFileLoader().Load(null);

        Assert.False(traits.TryGetValue("dbo.PIT_CustomerSnapshot", out _));
        Assert.Equal(SqlObjectStateClass.Persistent, SqlObjectTraits.Default.StateClass);
        Assert.Equal(SqlObjectAutoPolicy.AdditiveOnly, SqlObjectTraits.Default.AutoPolicy);
    }
}
