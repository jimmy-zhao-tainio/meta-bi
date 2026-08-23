CREATE VIEW [dq].[v_customer_campaign_consensus_03]
AS
SELECT
    c.CompanyId,
    c.CustomerId,
    g.CampaignId,
    g.CampaignName
FROM dqdemo.Customer c
INNER JOIN dqdemo.Campaign g
    ON c.CompanyId = g.CompanyId
   AND c.CustomerId = g.CampaignId;
GO
