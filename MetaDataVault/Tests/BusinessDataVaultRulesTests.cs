using MetaBusinessDataVault;
using MetaDataVault.Core;

namespace MetaDataVault.Tests;

public sealed class BusinessDataVaultRulesTests
{
    [Fact]
    public void ValidateSatelliteSpecializations_RejectsCommonSatelliteWithoutConcreteSpecialization()
    {
        var model = new MetaBusinessDataVaultModel();
        model.BusinessSatelliteList.Add(new BusinessSatellite
        {
            Id = "CustomerProfile",
            Name = "Profile",
        });

        var error = Assert.Throws<InvalidOperationException>(() =>
            BusinessDataVaultRules.ValidateSatelliteSpecializations(model));

        Assert.Contains("CustomerProfile", error.Message, StringComparison.Ordinal);
        Assert.Contains("exactly one concrete specialization", error.Message, StringComparison.Ordinal);
    }
}
