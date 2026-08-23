CREATE VIEW [dq].[v_order_campaign_left_07]
AS
SELECT
    o.CompanyId,
    o.OrderId,
    o.CampaignId,
    g.CampaignName
FROM dqdemo.OrderHeader o
LEFT OUTER JOIN dqdemo.Campaign g
    ON o.CompanyId = g.CompanyId
   AND o.CampaignId = g.CampaignId;
GO
