CREATE VIEW [dq].[v_customer_campaign_subset_outlier_01]
AS
SELECT
    c.CompanyId,
    c.CustomerId,
    g.CampaignId,
    g.CampaignName
FROM dqdemo.Customer c
INNER JOIN dqdemo.Campaign g
    ON c.CustomerId = g.CampaignId;
GO
