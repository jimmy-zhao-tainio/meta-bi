namespace MetaSql;

/// <summary>
/// Declares high-level ordering phases for deploy actions.
/// </summary>
internal enum DeployActionOrdering
{
    Drop = 0,
    Truncate = 1,
    Alter = 2,
    Add = 3,
}
