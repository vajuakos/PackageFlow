namespace PackageFlow.Shared.Enums
{
    public enum PackageSize
    {
        ExtraSmall,
        Small,
        Medium,
        Large,
        ExtraLarge
    }

    public static class PackageSizeExtensions
    {
        public static string ToDisplayName(this PackageSize size) => size switch
        {
            PackageSize.ExtraSmall => "Extra kicsi (XS) – max. 1 kg (pl. boríték, okmány)",
            PackageSize.Small => "Kicsi (S) – max. 5 kg (pl. könyv, ruha)",
            PackageSize.Medium => "Közepes (M) – max. 15 kg (pl. cipősdoboz)",
            PackageSize.Large => "Nagy (L) – max. 25 kg (pl. mikrohullámú sütő)",
            PackageSize.ExtraLarge => "Extra nagy (XL) – max. 40 kg (nagyméretű küldemény)",
            _ => size.ToString()
        };
    }
}
