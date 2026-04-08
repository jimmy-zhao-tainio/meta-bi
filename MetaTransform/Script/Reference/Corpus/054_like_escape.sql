CREATE VIEW dbo.v_like_escape AS
SELECT
    s.CustomerId
FROM dbo.Source AS s
WHERE s.CustomerCode LIKE 'A!_%' ESCAPE '!'
    AND s.CustomerName NOT LIKE 'B!%%' ESCAPE '!';
