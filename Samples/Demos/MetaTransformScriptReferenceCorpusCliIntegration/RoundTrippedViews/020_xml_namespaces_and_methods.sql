CREATE VIEW dbo.v_xml_namespaces_and_methods
AS
WITH XMLNAMESPACES ('urn:test' AS ns)
SELECT
    s.Id,
    s.XmlPayload.value('(/ns:Root/ns:Id/text())[1]', 'int') AS XmlId,
    s.XmlPayload.query('/ns:Root') AS XmlFragment,
    s.XmlPayload.exist('/ns:Root') AS HasRoot
FROM dbo.XmlSource AS s
GO
