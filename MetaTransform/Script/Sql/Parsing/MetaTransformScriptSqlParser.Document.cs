using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        public TopLevelStatementShape ParseDocument()
        {
            if (MatchKeyword("CREATE"))
            {
                if (PeekKeyword("VIEW"))
                {
                    ParseCreateViewScript();
                    return TopLevelStatementShape.CreateWrappedSelect;
                }

                if (PeekKeyword("FUNCTION"))
                {
                    ParseCreateFunctionScript();
                    return TopLevelStatementShape.CreateWrappedSelect;
                }

                throw Unsupported($"CREATE wrapper '{Current.Value.ToUpperInvariant()}' is not supported yet.");
            }

            var statement = ParseStatement();
            SkipSemicolons();
            ExpectEndOfFile();

            var isSelect = statement.TryGetId(nameof(SelectStatement), out _);
            var scriptName = ResolveBareStatementName(isSelect);
            if (string.IsNullOrWhiteSpace(scriptName))
            {
                throw Unsupported(isSelect
                    ? "Bare SELECT input requires an explicit script name."
                    : "Bare mutation statement input requires an explicit script name or source path.");
            }

            builder.AddTransformScript(scriptName, string.Empty, sourcePath, statement, schemaIdentifier: null, objectIdentifier: null);
            return isSelect ? TopLevelStatementShape.BareSelect : TopLevelStatementShape.BareMutation;
        }

        private void ParseCreateViewScript()
        {
            ExpectKeyword("VIEW");

            var (schemaIdentifier, objectIdentifier, renderedName) = ParseCreateViewName();
            List<BuiltNode>? viewColumns = null;
            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                viewColumns = ParseCreateViewColumnList();
            }

            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateViewOptions();
            }

            ExpectKeyword("AS");
            var selectStatement = ParseSelectStatement();

            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateViewTailClause();
            }

            SkipSemicolons();
            ExpectEndOfFile();

            builder.AddTransformScript(
                renderedName,
                string.Empty,
                sourcePath,
                selectStatement,
                schemaIdentifier?.Node,
                objectIdentifier.Node,
                viewColumns,
                scriptObjectKind: "View");
        }

        private void ParseCreateFunctionScript()
        {
            ExpectKeyword("FUNCTION");

            var (schemaIdentifier, objectIdentifier, renderedName) = ParseCreateFunctionName();
            var functionParameters = ParseCreateFunctionParameters();

            ExpectKeyword("RETURNS");
            if (MatchKeyword("TABLE"))
            {
                if (MatchKeyword("WITH"))
                {
                    ParseUnsupportedCreateFunctionOptions();
                }

                ExpectKeyword("AS");
                ExpectKeyword("RETURN");

                BuiltNode selectStatement;
                if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
                {
                    selectStatement = ParseSelectStatement();
                    Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                }
                else
                {
                    selectStatement = ParseSelectStatement();
                }

                SkipSemicolons();
                ExpectEndOfFile();

                builder.AddTransformScript(
                    renderedName,
                    string.Empty,
                    sourcePath,
                    selectStatement,
                    schemaIdentifier?.Node,
                    objectIdentifier.Node,
                    scriptObjectKind: "InlineTableValuedFunction",
                    functionParameters: functionParameters);
                return;
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.Identifier
                && Current.Value.StartsWith("@", StringComparison.Ordinal))
            {
                throw UnsupportedFunctionWrapper(GetUnsupportedCreateFunctionWrapperMessage());
            }

            var returnDataType = ParseDataTypeReference();
            if (MatchKeyword("WITH"))
            {
                ParseUnsupportedCreateFunctionOptions();
            }

            ExpectKeyword("AS");
            var returnExpression = ParseScalarFunctionBodyReturnExpression();

            SkipSemicolons();
            ExpectEndOfFile();

            builder.AddScalarFunctionScript(
                renderedName,
                sourcePath,
                returnDataType,
                returnExpression,
                schemaIdentifier?.Node,
                objectIdentifier.Node,
                functionParameters: functionParameters);
        }

        private BuiltNode ParseScalarFunctionBodyReturnExpression()
        {
            SkipSemicolons();
            if (MatchKeyword("RETURN"))
            {
                return ParseScalarFunctionReturnExpression();
            }

            ExpectKeyword("BEGIN");
            SkipSemicolons();
            if (!MatchKeyword("RETURN"))
            {
                throw UnsupportedFunctionWrapper("Scalar CREATE FUNCTION bodies are supported only when they reduce to a single RETURN scalar expression or RETURN SELECT query.");
            }

            var returnExpression = ParseScalarFunctionReturnExpression();
            SkipSemicolons();
            ExpectKeyword("END");
            return returnExpression;
        }

        private BuiltNode ParseScalarFunctionReturnExpression()
        {
            if (PeekKeyword("SELECT"))
            {
                return builder.CreateScalarSubquery(ParseQueryExpression());
            }

            return ParseScalarExpression();
        }

        private string GetUnsupportedCreateFunctionWrapperMessage()
        {
            if (Current.Kind == MetaTransformScriptSqlTokenKind.Identifier
                && Current.Value.StartsWith("@", StringComparison.Ordinal))
            {
                return "Multistatement table-valued CREATE FUNCTION wrappers are not supported by the MetaTransformScript SQL importer; only inline table-valued functions (RETURNS TABLE AS RETURN SELECT ...) are supported.";
            }

            return "Scalar CREATE FUNCTION wrappers are not supported by the MetaTransformScript SQL importer; only inline table-valued functions (RETURNS TABLE AS RETURN SELECT ...) are supported.";
        }

        private void ParseUnsupportedCreateViewOptions()
        {
            var optionNames = ParseCommaSeparatedOptionNames();
            var rendered = optionNames.Count == 0
                ? "WITH <view options>"
                : "WITH " + string.Join(", ", optionNames);
            throw Unsupported($"CREATE VIEW wrapper option clause '{rendered}' is not supported yet.");
        }

        private void ParseUnsupportedCreateFunctionOptions()
        {
            var optionNames = ParseCommaSeparatedOptionNames();
            var rendered = optionNames.Count == 0
                ? "WITH <function options>"
                : "WITH " + string.Join(", ", optionNames);
            throw Unsupported($"CREATE FUNCTION wrapper option clause '{rendered}' is not supported yet.");
        }

        private List<string> ParseCommaSeparatedOptionNames()
        {
            var optionNames = new List<string>();

            while (true)
            {
                optionNames.Add(ParseCreateViewOptionName());
                if (!Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    break;
                }
            }

            return optionNames;
        }

        private string ParseCreateViewOptionName()
        {
            return ParseIdentifierToken().Value.ToUpperInvariant();
        }

        private void ParseUnsupportedCreateViewTailClause()
        {
            if (MatchKeyword("CHECK"))
            {
                ExpectKeyword("OPTION");
                throw Unsupported("CREATE VIEW tail clause 'WITH CHECK OPTION' is not supported yet.");
            }

            throw Unsupported($"Unsupported CREATE VIEW tail clause beginning with 'WITH {Current.Value.ToUpperInvariant()}'.");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateViewName()
        {
            return ParseCreateObjectName("VIEW");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateFunctionName()
        {
            return ParseCreateObjectName("FUNCTION");
        }

        private (ParsedIdentifier? SchemaIdentifier, ParsedIdentifier ObjectIdentifier, string RenderedName) ParseCreateObjectName(string objectType)
        {
            var first = ParseIdentifier();
            if (!Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                return (null, first, RenderIdentifier(first.Token));
            }

            var second = ParseIdentifier();
            if (Match(MetaTransformScriptSqlTokenKind.Dot))
            {
                throw Unsupported($"CREATE {objectType} names with more than two identifier parts are not supported.");
            }

            return (first, second, $"{RenderIdentifier(first.Token)}.{RenderIdentifier(second.Token)}");
        }

        private List<BuiltNode> ParseCreateViewColumnList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var identifiers = new List<BuiltNode> { ParseIdentifier().Node };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                identifiers.Add(ParseIdentifier().Node);
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return identifiers;
        }

        private List<(BuiltNode ParameterName, BuiltNode DataTypeReference)> ParseCreateFunctionParameters()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var parameters = new List<(BuiltNode ParameterName, BuiltNode DataTypeReference)>();
            if (Match(MetaTransformScriptSqlTokenKind.CloseParen))
            {
                return parameters;
            }

            while (true)
            {
                var parameterName = ParseIdentifier();
                if (!parameterName.Token.Value.StartsWith('@'))
                {
                    throw Unsupported("CREATE FUNCTION parameters must be declared as @variables.");
                }

                var dataTypeReference = ParseDataTypeReference();
                if (Match(MetaTransformScriptSqlTokenKind.Equals))
                {
                    throw Unsupported("CREATE FUNCTION parameter default values are not supported yet.");
                }

                if (MatchKeyword("READONLY"))
                {
                    throw Unsupported("CREATE FUNCTION READONLY parameters are not supported yet.");
                }

                parameters.Add((parameterName.Node, dataTypeReference));

                if (!Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    break;
                }
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return parameters;
        }

        private BuiltNode ParseStatement()
        {
            var prefix = ParseStatementPrefix(allowXmlNamespaces: true);

            if (PeekKeyword("SELECT") || Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                return ParseSelectStatement(prefix);
            }

            if (PeekKeyword("INSERT"))
            {
                return ParseInsertStatement(prefix);
            }

            if (PeekKeyword("UPDATE"))
            {
                return ParseUpdateStatement(prefix);
            }

            if (PeekKeyword("DELETE"))
            {
                return ParseDeleteStatement(prefix);
            }

            if (PeekKeyword("TRUNCATE"))
            {
                return ParseTruncateStatement(prefix);
            }

            if (PeekKeyword("MERGE"))
            {
                return ParseMergeStatement(prefix);
            }

            throw Unsupported($"Top-level statement '{Current.Value.ToUpperInvariant()}' is not supported yet.");
        }

        private BuiltNode ParseSelectStatement()
        {
            return ParseSelectStatement(ParseStatementPrefix(allowXmlNamespaces: true));
        }

        private BuiltNode ParseSelectStatement(StatementPrefix prefix)
        {
            var queryExpression = ParseQueryExpression();
            return builder.CreateSelectStatement(queryExpression, prefix.CommonTableExpressions, prefix.XmlNamespaces);
        }

        private BuiltNode ParseInsertStatement(StatementPrefix prefix)
        {
            RejectXmlNamespacesForMutation(prefix);
            ExpectKeyword("INSERT");
            MatchKeyword("INTO");

            var target = ParseSchemaObjectName();
            var columns = Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen
                ? ParseIdentifierList()
                : null;

            BuiltNode source;
            if (MatchKeyword("VALUES"))
            {
                var rowValues = new List<BuiltNode> { ParseRowValue() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    rowValues.Add(ParseRowValue());
                }

                source = builder.CreateInsertValuesSource(rowValues);
            }
            else if (MatchKeyword("DEFAULT"))
            {
                ExpectKeyword("VALUES");
                throw Unsupported("INSERT DEFAULT VALUES is not supported yet.");
            }
            else
            {
                source = builder.CreateInsertQuerySource(ParseQueryExpression());
            }

            return builder.CreateInsertStatement(target, source, columns, prefix.CommonTableExpressions);
        }

        private BuiltNode ParseUpdateStatement(StatementPrefix prefix)
        {
            RejectXmlNamespacesForMutation(prefix);
            ExpectKeyword("UPDATE");

            var target = ParseSchemaObjectName();
            var targetAlias = ParseOptionalTargetAlias();
            var setClause = ParseSetClause();

            BuiltNode? fromClause = null;
            if (MatchKeyword("FROM"))
            {
                fromClause = ParseFromClause();
            }

            BuiltNode? whereClause = null;
            if (MatchKeyword("WHERE"))
            {
                whereClause = builder.CreateWhereClause(ParseBooleanExpression());
            }

            return builder.CreateUpdateStatement(
                target,
                setClause,
                targetAlias,
                fromClause,
                whereClause,
                prefix.CommonTableExpressions);
        }

        private BuiltNode ParseDeleteStatement(StatementPrefix prefix)
        {
            RejectXmlNamespacesForMutation(prefix);
            ExpectKeyword("DELETE");
            MatchKeyword("FROM");

            var target = ParseSchemaObjectName();

            BuiltNode? fromClause = null;
            if (MatchKeyword("FROM"))
            {
                fromClause = ParseFromClause();
            }

            BuiltNode? whereClause = null;
            if (MatchKeyword("WHERE"))
            {
                whereClause = builder.CreateWhereClause(ParseBooleanExpression());
            }

            return builder.CreateDeleteStatement(target, fromClause, whereClause, prefix.CommonTableExpressions);
        }

        private BuiltNode ParseTruncateStatement(StatementPrefix prefix)
        {
            if (prefix.HasAny)
            {
                throw Unsupported("TRUNCATE statements do not support WITH clauses.");
            }

            ExpectKeyword("TRUNCATE");
            ExpectKeyword("TABLE");
            return builder.CreateTruncateStatement(ParseSchemaObjectName());
        }

        private BuiltNode ParseMergeStatement(StatementPrefix prefix)
        {
            RejectXmlNamespacesForMutation(prefix);
            ExpectKeyword("MERGE");

            BuiltNode? topRowFilter = null;
            if (MatchKeyword("TOP"))
            {
                topRowFilter = ParseMergeTopRowFilter();
            }

            MatchKeyword("INTO");

            var target = ParseSchemaObjectName();
            var targetHints = PeekKeyword("WITH")
                ? ParseHintList()
                : null;
            var targetAlias = ParseOptionalTargetAlias();

            ExpectKeyword("USING");
            var source = ParseTableReference();

            ExpectKeyword("ON");
            var searchCondition = ParseBooleanExpression();

            var whenClauses = new List<ParsedMergeWhenClause> { ParseMergeWhenClause() };
            while (PeekKeyword("WHEN"))
            {
                whenClauses.Add(ParseMergeWhenClause());
            }

            ValidateMergeWhenClauses(whenClauses);

            BuiltNode? outputClause = null;
            if (PeekKeyword("OUTPUT"))
            {
                outputClause = ParseOutputClause();
            }

            BuiltNode? optionClause = null;
            if (PeekKeyword("OPTION"))
            {
                optionClause = ParseOptionClause();
            }

            if (!Match(MetaTransformScriptSqlTokenKind.Semicolon))
            {
                throw ParseError("SQL Server MERGE statements must end with a semicolon.");
            }

            return builder.CreateMergeStatement(
                target,
                source,
                searchCondition,
                whenClauses.Select(static clause => clause.Node).ToArray(),
                targetAlias,
                prefix.CommonTableExpressions,
                topRowFilter,
                targetHints,
                outputClause,
                optionClause);
        }

        private BuiltNode ParseMergeTopRowFilter()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var expression = builder.CreateParenthesisExpression(ParseScalarExpression());
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            var percent = MatchKeyword("PERCENT");
            return builder.CreateTopRowFilter(expression, percent, withTies: false);
        }

        private ParsedMergeWhenClause ParseMergeWhenClause()
        {
            ExpectKeyword("WHEN");

            string matchKind;
            if (MatchKeyword("MATCHED"))
            {
                matchKind = "Matched";
            }
            else
            {
                ExpectKeyword("NOT");
                ExpectKeyword("MATCHED");
                matchKind = "NotMatchedByTarget";
                if (MatchKeyword("BY"))
                {
                    if (MatchKeyword("SOURCE"))
                    {
                        matchKind = "NotMatchedBySource";
                    }
                    else
                    {
                        ExpectKeyword("TARGET");
                    }
                }
            }

            BuiltNode? searchCondition = null;
            if (MatchKeyword("AND"))
            {
                searchCondition = ParseBooleanExpression();
            }

            ExpectKeyword("THEN");
            var action = ParseMergeAction(matchKind);
            var actionKind = ResolveMergeActionKind(action);
            return new ParsedMergeWhenClause(
                builder.CreateMergeWhenClause(matchKind, action, searchCondition),
                matchKind,
                actionKind,
                searchCondition is not null);
        }

        private void ValidateMergeWhenClauses(IReadOnlyList<ParsedMergeWhenClause> whenClauses)
        {
            ValidateMergeRepeatedActionClauses(
                whenClauses.Where(static clause => string.Equals(clause.MatchKind, "Matched", StringComparison.Ordinal)).ToArray(),
                "WHEN MATCHED");
            ValidateMergeRepeatedActionClauses(
                whenClauses.Where(static clause => string.Equals(clause.MatchKind, "NotMatchedBySource", StringComparison.Ordinal)).ToArray(),
                "WHEN NOT MATCHED BY SOURCE");

            var notMatchedByTargetCount = whenClauses.Count(static clause =>
                string.Equals(clause.MatchKind, "NotMatchedByTarget", StringComparison.Ordinal));
            if (notMatchedByTargetCount > 1)
            {
                throw Unsupported("SQL Server MERGE supports at most one WHEN NOT MATCHED BY TARGET clause.");
            }
        }

        private void ValidateMergeRepeatedActionClauses(IReadOnlyList<ParsedMergeWhenClause> clauses, string clauseName)
        {
            if (clauses.Count > 2)
            {
                throw Unsupported($"SQL Server MERGE supports at most two {clauseName} clauses.");
            }

            if (clauses.Count < 2)
            {
                return;
            }

            if (!clauses[0].HasSearchCondition)
            {
                throw Unsupported($"When two {clauseName} clauses are present, the first must include an AND search condition.");
            }

            if (string.Equals(clauses[0].ActionKind, clauses[1].ActionKind, StringComparison.Ordinal))
            {
                throw Unsupported($"When two {clauseName} clauses are present, one must UPDATE and one must DELETE.");
            }
        }

        private BuiltNode ParseMergeAction(string matchKind)
        {
            if (MatchKeyword("UPDATE"))
            {
                if (string.Equals(matchKind, "NotMatchedByTarget", StringComparison.Ordinal))
                {
                    throw Unsupported("WHEN NOT MATCHED BY TARGET supports INSERT actions only.");
                }

                return builder.CreateMergeUpdateAction(ParseSetClause());
            }

            if (MatchKeyword("DELETE"))
            {
                if (string.Equals(matchKind, "NotMatchedByTarget", StringComparison.Ordinal))
                {
                    throw Unsupported("WHEN NOT MATCHED BY TARGET supports INSERT actions only.");
                }

                return builder.CreateMergeDeleteAction();
            }

            if (MatchKeyword("INSERT"))
            {
                if (!string.Equals(matchKind, "NotMatchedByTarget", StringComparison.Ordinal))
                {
                    throw Unsupported("MERGE INSERT actions are only supported for WHEN NOT MATCHED BY TARGET.");
                }

                var columns = Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen
                    ? ParseIdentifierList()
                    : null;
                ExpectKeyword("VALUES");
                var values = ParseScalarExpressionList();
                return builder.CreateMergeInsertAction(columns, values);
            }

            throw Unsupported($"Unsupported MERGE action '{Current.Value.ToUpperInvariant()}'.");
        }

        private static string ResolveMergeActionKind(BuiltNode action)
        {
            if (action.TryGetId(nameof(MergeUpdateAction), out _))
            {
                return "Update";
            }

            if (action.TryGetId(nameof(MergeDeleteAction), out _))
            {
                return "Delete";
            }

            if (action.TryGetId(nameof(MergeInsertAction), out _))
            {
                return "Insert";
            }

            throw new InvalidOperationException("Unsupported MERGE action node.");
        }

        private BuiltNode ParseOutputClause()
        {
            ExpectKeyword("OUTPUT");

            var previousAllowMergeActionPseudoColumn = allowMergeActionPseudoColumn;
            allowMergeActionPseudoColumn = true;
            List<BuiltNode> selectElements;
            try
            {
                selectElements = new List<BuiltNode> { ParseSelectElement() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    selectElements.Add(ParseSelectElement());
                }
            }
            finally
            {
                allowMergeActionPseudoColumn = previousAllowMergeActionPseudoColumn;
            }

            BuiltNode? intoTarget = null;
            List<BuiltNode>? intoColumns = null;
            if (MatchKeyword("INTO"))
            {
                intoTarget = ParseSchemaObjectName();
                if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    intoColumns = ParseIdentifierList();
                }
            }

            return builder.CreateOutputClause(selectElements, intoTarget, intoColumns);
        }

        private BuiltNode ParseOptionClause()
        {
            ExpectKeyword("OPTION");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            var hints = new List<BuiltNode> { ParseQueryHint() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                hints.Add(ParseQueryHint());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateOptionClause(hints);
        }

        private List<BuiltNode> ParseHintList()
        {
            ExpectKeyword("WITH");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);

            var hints = new List<BuiltNode> { ParseTargetHint() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                hints.Add(ParseTargetHint());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return hints;
        }

        private BuiltNode ParseTargetHint()
        {
            var hintKeyword = ParseIdentifier();
            var hintName = hintKeyword.Token.Value;
            if (!SupportedTargetHintKeywords.Contains(hintName))
            {
                throw Unsupported($"MERGE target table hint '{hintName}' is not supported yet.");
            }

            if (string.Equals(hintName, "INDEX", StringComparison.OrdinalIgnoreCase))
            {
                if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
                {
                    return builder.CreateSqlHint([hintKeyword.Node], ParseScalarExpressionList(), "Parenthesized");
                }

                return builder.CreateSqlHint([hintKeyword.Node]);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                throw Unsupported($"MERGE target table hint '{hintName}' does not support arguments in the current modeled surface.");
            }

            if (Current.Kind is not (MetaTransformScriptSqlTokenKind.Comma or MetaTransformScriptSqlTokenKind.CloseParen))
            {
                throw ParseError($"Expected comma or ')' after MERGE target table hint '{hintName}' but found '{Current.Text}'.");
            }

            return builder.CreateSqlHint([hintKeyword.Node]);
        }

        private BuiltNode ParseQueryHint()
        {
            var firstKeyword = ParseIdentifier();
            var firstName = firstKeyword.Token.Value;

            if (string.Equals(firstName, "HASH", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstName, "ORDER", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstName, "LOOP", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(firstName, "MERGE", StringComparison.OrdinalIgnoreCase))
            {
                var joinKeyword = ParseIdentifier();
                if (!string.Equals(joinKeyword.Token.Value, "JOIN", StringComparison.OrdinalIgnoreCase))
                {
                    throw Unsupported($"MERGE OPTION query hint '{firstName} {joinKeyword.Token.Value}' is not supported yet.");
                }

                return builder.CreateSqlHint([firstKeyword.Node, joinKeyword.Node]);
            }

            if (string.Equals(firstName, "MAXDOP", StringComparison.OrdinalIgnoreCase))
            {
                return builder.CreateSqlHint([firstKeyword.Node], [ParseScalarExpression()], "Bare");
            }

            if (string.Equals(firstName, "RECOMPILE", StringComparison.OrdinalIgnoreCase) ||
                (string.Equals(firstName, "FORCE", StringComparison.OrdinalIgnoreCase) && MatchKeyword("ORDER")))
            {
                var keywords = string.Equals(firstName, "FORCE", StringComparison.OrdinalIgnoreCase)
                    ? new[] { firstKeyword.Node, builder.CreateIdentifier("ORDER", "NotQuoted") }
                    : [firstKeyword.Node];
                return builder.CreateSqlHint(keywords);
            }

            if (Current.Kind == MetaTransformScriptSqlTokenKind.OpenParen)
            {
                throw Unsupported($"MERGE OPTION query hint '{firstName}' with arguments is not supported yet.");
            }

            if (Current.Kind is not (MetaTransformScriptSqlTokenKind.Comma or MetaTransformScriptSqlTokenKind.CloseParen))
            {
                throw ParseError($"Expected comma or ')' after MERGE OPTION query hint '{firstName}' but found '{Current.Text}'.");
            }

            throw Unsupported($"MERGE OPTION query hint '{firstName}' is not supported yet.");
        }

        private sealed record ParsedMergeWhenClause(
            BuiltNode Node,
            string MatchKind,
            string ActionKind,
            bool HasSearchCondition);

        private BuiltNode ParseSetClause()
        {
            ExpectKeyword("SET");
            var assignments = new List<BuiltNode> { ParseSetAssignment() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                assignments.Add(ParseSetAssignment());
            }

            return builder.CreateSetClause(assignments);
        }

        private BuiltNode ParseSetAssignment()
        {
            var target = ParseColumnReferenceExpression();
            Expect(MetaTransformScriptSqlTokenKind.Equals);
            var value = ParseScalarExpression();
            return builder.CreateSetAssignment(target, value);
        }

        private StatementPrefix ParseStatementPrefix(bool allowXmlNamespaces)
        {
            BuiltNode? xmlNamespaces = null;
            List<BuiltNode>? commonTableExpressions = null;
            if (MatchKeyword("WITH"))
            {
                if (allowXmlNamespaces && PeekKeyword("XMLNAMESPACES"))
                {
                    xmlNamespaces = ParseXmlNamespacesClause();
                    if (Match(MetaTransformScriptSqlTokenKind.Comma))
                    {
                        commonTableExpressions = ParseCommonTableExpressions();
                    }
                }
                else
                {
                    commonTableExpressions = ParseCommonTableExpressions();
                }
            }

            return new StatementPrefix(commonTableExpressions, xmlNamespaces);
        }

        private BuiltNode? ParseOptionalTargetAlias()
        {
            if (MatchKeyword("AS"))
            {
                return ParseIdentifier().Node;
            }

            return CanStartAlias() ? ParseIdentifier().Node : null;
        }

        private List<BuiltNode> ParseIdentifierList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var identifiers = new List<BuiltNode> { ParseIdentifier().Node };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                identifiers.Add(ParseIdentifier().Node);
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return identifiers;
        }

        private List<BuiltNode> ParseScalarExpressionList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var values = new List<BuiltNode> { ParseScalarExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                values.Add(ParseScalarExpression());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return values;
        }

        private string? ResolveBareStatementName(bool isSelect)
        {
            if (!string.IsNullOrWhiteSpace(bareSelectName))
            {
                return bareSelectName;
            }

            if (isSelect || string.IsNullOrWhiteSpace(sourcePath))
            {
                return null;
            }

            var fileName = Path.GetFileNameWithoutExtension(sourcePath);
            return string.IsNullOrWhiteSpace(fileName) ? null : fileName;
        }

        private void RejectXmlNamespacesForMutation(StatementPrefix prefix)
        {
            if (prefix.XmlNamespaces is not null)
            {
                throw Unsupported("WITH XMLNAMESPACES is only supported for SELECT statements.");
            }
        }

        private sealed record StatementPrefix(
            IReadOnlyList<BuiltNode>? CommonTableExpressions,
            BuiltNode? XmlNamespaces)
        {
            public bool HasAny => XmlNamespaces is not null || (CommonTableExpressions?.Count ?? 0) > 0;
        }

        private List<BuiltNode> ParseCommonTableExpressions()
        {
            var commonTableExpressions = new List<BuiltNode> { ParseCommonTableExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                commonTableExpressions.Add(ParseCommonTableExpression());
            }

            return commonTableExpressions;
        }

        private BuiltNode ParseCommonTableExpression()
        {
            var expressionName = ParseIdentifier().Node;
            List<BuiltNode>? columns = null;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                columns = new List<BuiltNode> { ParseIdentifier().Node };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    columns.Add(ParseIdentifier().Node);
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            ExpectKeyword("AS");
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var queryExpression = ParseQueryExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateCommonTableExpression(expressionName, queryExpression, columns);
        }
    }
}
