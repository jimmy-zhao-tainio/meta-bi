CREATE VIEW dbo.v_arithmetic_operators AS
SELECT
    s.Amount - s.Discount AS NetAmount,
    s.Quantity * s.UnitPrice AS GrossAmount,
    s.TotalAmount / s.Divisor AS RatioAmount,
    s.SequenceNumber % 10 AS SequenceBucket,
    (s.BaseAmount + 5) * 2 AS WeightedAmount
FROM dbo.Source AS s
WHERE (s.Quantity * s.UnitPrice) - s.Discount > 0;
