CREATE VIEW dbo.v_read_shared_stage
AS
SELECT
    CustomerId,
    CustomerName
FROM MetaOrchestrationCliIntegration.dbo.StageCustomer;
GO
