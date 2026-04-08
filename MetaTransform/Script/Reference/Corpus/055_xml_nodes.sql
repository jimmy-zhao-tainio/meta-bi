CREATE VIEW dbo.v_xml_nodes AS
SELECT
    s.Id,
    n.Item.value('(@Code)[1]', 'nvarchar(50)') AS ItemCode
FROM dbo.XmlSource AS s
CROSS APPLY s.XmlPayload.nodes('/Root/Item') AS n(Item);
