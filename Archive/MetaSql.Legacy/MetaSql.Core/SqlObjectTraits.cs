namespace MetaSql.Core;

public enum SqlObjectStateClass
{
    Persistent,
    Replaceable
}

public enum SqlObjectAutoPolicy
{
    AdditiveOnly,
    AdditivePlusEmptyDrop
}

public sealed record SqlObjectTraits(
    SqlObjectStateClass StateClass,
    SqlObjectAutoPolicy AutoPolicy,
    string? ValidationProfile = null,
    string? DependencyGroup = null)
{
    public static readonly SqlObjectTraits Default = new(
        SqlObjectStateClass.Persistent,
        SqlObjectAutoPolicy.AdditiveOnly);
}
