namespace MetaDataQuality.Core;

public static class CandidateKinds
{
    public const string JoinOrphan = "JoinOrphan";
    public const string OuterJoinNullExpansion = "OuterJoinNullExpansion";
    public const string JoinMultiplicityExplosion = "JoinMultiplicityExplosion";
    public const string OutputDuplicateRisk = "OutputDuplicateRisk";
    public const string MinorityJoinPattern = "MinorityJoinPattern";
    public const string IncompleteCompositeJoin = "IncompleteCompositeJoin";
    public const string SuspiciousExtraJoinPredicate = "SuspiciousExtraJoinPredicate";
    public const string MissingCommonFilter = "MissingCommonFilter";
    public const string MinorityColumnEquivalence = "MinorityColumnEquivalence";
    public const string InnerJoinAgainstUsuallyOptionalRelationship = "InnerJoinAgainstUsuallyOptionalRelationship";
    public const string LeftJoinAgainstUsuallyMandatoryRelationship = "LeftJoinAgainstUsuallyMandatoryRelationship";
    public const string ImpliedForeignKeyMissingReference = "ImpliedForeignKeyMissingReference";
    public const string ImpliedUniqueKeyViolation = "ImpliedUniqueKeyViolation";
    public const string ImpliedJoinFanoutRisk = "ImpliedJoinFanoutRisk";
    public const string ImpliedOutputDuplicateRisk = "ImpliedOutputDuplicateRisk";
}
