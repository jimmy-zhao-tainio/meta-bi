CREATE OR ALTER PROCEDURE [dq].[Explain]
    @CheckNo bigint,
    @RunId bigint = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SourceDatabase sysname = DB_NAME();
    IF @RunId IS NULL
        SELECT TOP (1) @RunId = [RunId]
        FROM [MetaDQ].[dbo].[RunLog]
        WHERE [SourceDatabaseName] = @SourceDatabase
        ORDER BY [RunId] DESC;

    DECLARE @Kind nvarchar(128), @Mode nvarchar(64), @Outcome nvarchar(64),
            @Title nvarchar(256), @Relationship nvarchar(512), @Transforms nvarchar(max),
            @DetailView sysname, @Evidence nvarchar(max);

    SELECT @Kind = r.[CandidateKind], @Mode = r.[OutputMode], @Outcome = r.[Outcome],
           @Title = COALESCE(r.[FindingTitle], c.[CheckName], r.[CandidateId]),
           @Relationship = r.[CheckedRelationship], @Transforms = r.[Transforms],
           @DetailView = c.[DetailView], @Evidence = r.[Explanation]
    FROM [MetaDQ].[dbo].[CheckReview] AS r
    LEFT JOIN [dq].[v_ReviewChecks] AS c ON c.[CandidateId] = r.[CandidateId] COLLATE DATABASE_DEFAULT
    WHERE r.[RunId] = @RunId AND r.[SourceDatabaseName] = @SourceDatabase AND r.[CheckNo] = @CheckNo;

    IF @Outcome IS NULL
        THROW 51000, N'Check not found for this run and database. Copy the Investigate command from dq.Run or dq.v_DataQualityReview.', 1;

    DECLARE @Example nvarchar(400), @HowToDecide nvarchar(400), @Template nvarchar(max);
    SELECT @Example = g.[Example], @HowToDecide = g.[HowToDecide]
    FROM (VALUES
        (N'JoinMultiplicityExplosion',
         N'One order can match three order lines. In a line report, three rows are expected. In an order report, adding the order total from all three rows triples it.',
         N'Decide what one output row represents. If you expect one row per order, count rows per OrderId in the actual output and investigate keys with more than one row.'),
        (N'OutputDuplicateRisk',
         N'An order summary should list order 42 once. If order 42 appears twice, a count of orders is too high. In an order-line report, the same OrderId can appear on several distinct lines.',
         N'Group the actual output by every column that identifies one expected row. For order lines that might be OrderId and LineNo. A repeated join key alone does not prove duplicate output.'),
        (N'OuterJoinNullExpansion',
         N'A left join can keep an order when no customer matches. The customer fields are then NULL. Keeping an order with no assigned salesperson may be expected.',
         N'Check whether the missing match is allowed. In the actual output, look for NULL in a column from the matched side that is always filled when a match exists.'),
        (N'JoinOrphan',
         N'An order may point to customer 42, but customer 42 is absent. A customer who has never placed an order is a different, often valid case.',
         N'Identify which side must have a match. Check for missing matches in that direction and handle blank reference keys according to the business rule.'),
        (N'ImpliedForeignKeyMissingReference',
         N'If each order needs an existing customer, an order with a CustomerId that is absent from Customers needs investigation.',
         N'Confirm the relationship direction and compare all the reference key columns against the lookup. Decide whether blank references are allowed.'),
        (N'ImpliedUniqueKeyViolation',
         N'Two customers with the same lookup key can cause a single order to match both.',
         N'Find the columns that should identify one lookup row. Group the lookup by those columns and review repeated keys. An account number may only be unique within a company.'),
        (N'MinorityJoinPattern',
         N'Most reports join orders by customer and company. One report joins only by customer and may mix companies.',
         N'Compare the two join conditions and run them on a known customer. Check which rows each condition includes.'),
        (N'IncompleteCompositeJoin',
         N'Order number 42 exists in two companies. Joining only on OrderId can match the other company too.',
         N'Compare the join with the full key. Try the join with both OrderId and CompanyId for a known order and check whether the extra match disappears.'),
        (N'SuspiciousExtraJoinPredicate',
         N'One report joins by CustomerId and City. A customer who has moved may now be excluded.',
         N'Compare the output for a known customer with and without the extra condition. Confirm whether that condition belongs in this report.'),
        (N'MissingCommonFilter',
         N'Most reports exclude cancelled orders. One report includes them, which can be valid for an audit but can distort a sales total.',
         N'Compare counts with and without the filter on the same data. Confirm which rows this transform is expected to include.'),
        (N'MinorityColumnEquivalence',
         N'Most reports join on CustomerId. This one uses BillingCustomerId, which can designate someone else.',
         N'Compare the joined results for records where the two IDs differ. Confirm which customer role the output should use.'),
        (N'InnerJoinAgainstUsuallyOptionalRelationship',
         N'An inner join can remove orders that have no assigned salesperson.',
         N'Check which orders disappear when using the inner join. Confirm whether the output should keep unassigned orders.'),
        (N'LeftJoinAgainstUsuallyMandatoryRelationship',
         N'A left join can keep orders with no matching customer, even when every order is expected to have one.',
         N'Look at unmatched orders and confirm whether they should be kept, rejected or corrected upstream.'),
        (N'ImpliedJoinFanoutRisk',
         N'Joining one order to many order lines can repeat the order total.',
         N'Check the real output at the level it promises. Repeated OrderId values are expected in a line report and unexpected in a one-row-per-order report.'),
        (N'ImpliedOutputDuplicateRisk',
         N'An order summary is supposed to have one row for each order. The join may cause the same order key to appear twice.',
         N'Group the actual output by the columns that should identify one row and review repeated keys.')
    ) AS g([Kind], [Example], [HowToDecide])
    WHERE g.[Kind] = @Kind;

    SET @Template = CASE
        WHEN @Kind IN (N'JoinMultiplicityExplosion', N'OutputDuplicateRisk',
                       N'ImpliedJoinFanoutRisk', N'ImpliedOutputDuplicateRisk')
            THEN N'SELECT OrderId, COUNT_BIG(*) AS RowsPerOrder FROM dbo.OrderOutput GROUP BY OrderId HAVING COUNT_BIG(*) > 1;'
        WHEN @Kind IN (N'OuterJoinNullExpansion', N'LeftJoinAgainstUsuallyMandatoryRelationship')
            THEN N'SELECT TOP (20) OrderId FROM dbo.OrderOutput WHERE MatchedCustomerId IS NULL ORDER BY OrderId;'
        WHEN @Kind IN (N'JoinOrphan', N'ImpliedForeignKeyMissingReference')
            THEN N'SELECT TOP (20) o.OrderId, o.CustomerId FROM dbo.Orders AS o WHERE o.CustomerId IS NOT NULL AND NOT EXISTS (SELECT 1 FROM dbo.Customers AS c WHERE c.CustomerId = o.CustomerId) ORDER BY o.OrderId;'
        WHEN @Kind = N'InnerJoinAgainstUsuallyOptionalRelationship'
            THEN N'SELECT TOP (20) o.OrderId, o.SalespersonId FROM dbo.Orders AS o LEFT JOIN dbo.Salespeople AS s ON s.SalespersonId = o.SalespersonId WHERE s.SalespersonId IS NULL ORDER BY o.OrderId;'
        WHEN @Kind = N'ImpliedUniqueKeyViolation'
            THEN N'SELECT CustomerId, COUNT_BIG(*) AS Matches FROM dbo.Customers GROUP BY CustomerId HAVING COUNT_BIG(*) > 1;'
    END;

    SELECT [Topic], [WhatToKnow]
    FROM (VALUES
        (1, N'Check', CONVERT(nvarchar(max), @Title)),
        (2, N'Saved result', @Outcome),
        (3, N'Transform', @Transforms),
        (4, N'Relationship', @Relationship),
        (5, N'Example', @Example),
        (6, N'How to decide', @HowToDecide),
        (7, N'SQL error', CASE WHEN @Outcome = N'Check failed' THEN @Evidence END)
    ) AS notes([Position], [Topic], [WhatToKnow])
    WHERE NULLIF([WhatToKnow], N'') IS NOT NULL
    ORDER BY [Position];

    DECLARE @DetailSource nvarchar(400) = QUOTENAME(@SourceDatabase) + N'.[dq].' + QUOTENAME(@DetailView);
    SELECT [Query], [SQL]
    FROM (VALUES
        (1, N'Inspect up to 20 rows from this check',
            CASE WHEN @Mode = N'RuntimeCheck' THEN
                N'SELECT TOP (20) [Relationship], [SuspectSide], [KeyValues], [SuspectCount] FROM ' +
                @DetailSource + N' ORDER BY [SuspectCount] DESC, [KeyValues];'
            WHEN @Mode = N'SemanticReviewFinding' THEN
                N'SELECT TOP (20) [Relationship], [DominantPattern], [OutlierPattern] FROM ' +
                @DetailSource + N' ORDER BY [Relationship], [OutlierPattern];'
            END),
        (2, N'Confirm the issue (replace example tables and keys)', @Template)
    ) AS queries([Position], [Query], [SQL])
    WHERE [SQL] IS NOT NULL
    ORDER BY [Position];
END;
GO
