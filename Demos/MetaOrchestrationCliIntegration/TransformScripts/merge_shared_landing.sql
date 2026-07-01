MERGE dbo.SharedLanding AS target
USING dbo.RawCustomerDelta AS source
ON target.CustomerId = source.CustomerId
WHEN MATCHED THEN
    UPDATE SET
        CustomerName = source.CustomerName
WHEN NOT MATCHED BY TARGET THEN
    INSERT
        (
            CustomerId,
            CustomerName
        )
    VALUES
        (
            source.CustomerId,
            source.CustomerName
        );
