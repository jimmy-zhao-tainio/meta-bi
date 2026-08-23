CREATE VIEW dbo.v_xml_namespaces_default AS
WITH XMLNAMESPACES (DEFAULT 'urn:test', 'urn:other' AS ns)
SELECT
    s.Id,
    s.XmlPayload.exist('/Root') AS HasRoot,
    s.XmlPayload.query('/ns:OtherRoot') AS OtherFragment
FROM dbo.XmlSource AS s;
