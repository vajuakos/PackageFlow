using PackageFlow.Shared.Enums;

namespace PackageFlow.API.Tests.Extensions
{
    public class PackageSizeExtensionsTests
    {
        [Theory]
        [InlineData(PackageSize.ExtraSmall, 1.0)]
        [InlineData(PackageSize.Small, 5.0)]
        [InlineData(PackageSize.Medium, 15.0)]
        [InlineData(PackageSize.Large, 25.0)]
        [InlineData(PackageSize.ExtraLarge, 40.0)]
        public void GetMaxWeight_ForEachPackageSize_ReturnsExpectedLimit(PackageSize size, decimal expectedMaxWeight)
        {
            var actualMaxWeight = size.GetMaxWeight();

            Assert.Equal(expectedMaxWeight, actualMaxWeight);
        }

        [Fact]
        public void ToDisplayName_ForAllDefinedEnumValues_ReturnsFormattedHungarianDescription()
        {
            foreach (var size in Enum.GetValues<PackageSize>())
            {
                var displayName = size.ToDisplayName();

                Assert.False(string.IsNullOrWhiteSpace(displayName));
                Assert.NotEqual(size.ToString(), displayName);
                Assert.Contains("kg", displayName);
            }
        }
    }
}
