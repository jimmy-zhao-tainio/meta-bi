CREATE VIEW dbo.v_data_type_variants AS
SELECT
    CAST(s.CustomerId AS bigint) AS CustomerIdBigInt,
    CAST(s.IsPreferred AS bit) AS IsPreferredBit,
    CAST(s.CreatedAt AS date) AS CreatedDate,
    CAST(s.CreatedAt AS datetime) AS CreatedDateTime,
    CAST(s.CreatedAt AS datetimeoffset) AS CreatedAtOffset,
    CAST(s.Amount AS float) AS AmountFloat,
    CONVERT(nvarchar(max), s.CustomerName) AS CustomerNameText,
    TRY_CONVERT(uniqueidentifier, s.ExternalId) AS ExternalIdGuid,
    CAST(s.Payload AS varbinary(max)) AS PayloadBinary,
    CAST(s.EventTime AS time) AS EventTimeValue
FROM dbo.TypeSource AS s;
