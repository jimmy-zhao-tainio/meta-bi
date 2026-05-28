MERGE INTO dbo.MergeCustomer AS tgt
USING dbo.MergeSourceCustomer AS src
ON tgt.CustomerId = src.CustomerId
WHEN MATCHED THEN
    UPDATE SET
        tgt.CustomerName = src.CustomerName,
        tgt.TotalAmount = src.TotalAmount
WHEN NOT MATCHED BY TARGET THEN
    INSERT
    (
        CustomerId,
        CustomerName,
        TotalAmount
    )
    VALUES
    (
        src.CustomerId,
        src.CustomerName,
        src.TotalAmount
    )
WHEN NOT MATCHED BY SOURCE THEN
    DELETE;
