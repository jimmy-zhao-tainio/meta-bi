CREATE VIEW [dq].[v_order_campaign_inner_outlier_01]
AS
SELECT
    o.CompanyId,
    o.OrderId,
    o.CampaignId,
    g.CampaignName
FROM dqdemo.OrderHeader o
INNER JOIN dqdemo.Campaign g
    ON o.CompanyId = g.CompanyId
   AND o.CampaignId = g.CampaignId;
GO
