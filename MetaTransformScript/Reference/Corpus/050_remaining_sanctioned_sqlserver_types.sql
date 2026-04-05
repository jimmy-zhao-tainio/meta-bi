CREATE VIEW dbo.v_remaining_sanctioned_sqlserver_types AS
SELECT
    CAST(s.Payload AS binary(16)) AS FixedBinaryPayload,
    TRY_CONVERT(money, s.AmountText) AS AmountMoney,
    TRY_CONVERT(smallmoney, s.AmountText) AS AmountSmallMoney,
    TRY_CONVERT(sql_variant, s.VariantText) AS VariantValue,
    TRY_CONVERT(xml, s.XmlText) AS XmlValue,
    TRY_CONVERT(geography, s.GeoText) AS GeoValue,
    TRY_CONVERT(geometry, s.GeoText) AS GeometryValue,
    TRY_CONVERT(hierarchyid, s.HierarchyText) AS HierarchyValue
FROM dbo.TypeSource AS s;
