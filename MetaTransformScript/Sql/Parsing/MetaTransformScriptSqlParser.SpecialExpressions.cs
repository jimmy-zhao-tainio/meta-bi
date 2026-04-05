using static MetaTransformScript.Sql.Parsing.MetaTransformScriptSqlModelBuilder;

namespace MetaTransformScript.Sql.Parsing;

public sealed partial class MetaTransformScriptSqlParser
{
    private sealed partial class Parser
    {
        private BuiltNode ParseFunctionLikeExpression(IReadOnlyList<MetaTransformScriptSqlToken> identifiers, BuiltNode? callTarget)
        {
            if (identifiers.Count == 0)
            {
                throw new InvalidOperationException("Function-like expression parsing requires at least one identifier.");
            }

            var functionNameToken = identifiers[^1];
            if (callTarget is null && identifiers.Count > 1)
            {
                callTarget = builder.CreateMultiPartIdentifierCallTarget(
                    builder.CreateMultiPartIdentifier(
                        identifiers.Take(identifiers.Count - 1)
                            .Select(token => builder.CreateIdentifier(token.Value, token.QuoteType))
                            .ToArray()));
            }

            var functionNameValue = functionNameToken.Value;

            if (callTarget is not null)
            {
                return ParseTrailingScalarSuffixes(ParseGenericFunctionCall(functionNameToken, callTarget));
            }

            return functionNameValue.ToUpperInvariant() switch
            {
                "COALESCE" => ParseTrailingScalarSuffixes(ParseCoalesceExpression()),
                "NULLIF" => ParseTrailingScalarSuffixes(ParseNullIfExpression()),
                "IIF" => ParseTrailingScalarSuffixes(ParseIIfCall()),
                "PARSE" => ParseTrailingScalarSuffixes(ParseParseCall()),
                "TRY_PARSE" => ParseTrailingScalarSuffixes(ParseTryParseCall()),
                "CAST" => ParseTrailingScalarSuffixes(ParseCastCall()),
                "TRY_CAST" => ParseTrailingScalarSuffixes(ParseTryCastCall()),
                "CONVERT" => ParseTrailingScalarSuffixes(ParseConvertCall()),
                "TRY_CONVERT" => ParseTrailingScalarSuffixes(ParseTryConvertCall()),
                _ => ParseTrailingScalarSuffixes(ParseGenericFunctionCall(functionNameToken, callTarget: null))
            };
        }

        private BuiltNode ParseGenericFunctionCall(MetaTransformScriptSqlToken functionNameToken, BuiltNode? callTarget)
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var uniqueRowFilter =
                MatchKeyword("DISTINCT") ? "Distinct" :
                string.Empty;
            var parameters = new List<BuiltNode>();
            if (!Match(MetaTransformScriptSqlTokenKind.CloseParen))
            {
                if (Match(MetaTransformScriptSqlTokenKind.Star))
                {
                    parameters.Add(builder.CreateWildcardColumnReferenceExpression());
                    Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                }
                else
                {
                    parameters.Add(ParseScalarExpression());
                    while (Match(MetaTransformScriptSqlTokenKind.Comma))
                    {
                        parameters.Add(ParseScalarExpression());
                    }

                    Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                }
            }

            var functionName = builder.CreateIdentifier(functionNameToken.Value, functionNameToken.QuoteType);
            var functionCall = builder.CreateFunctionCall(functionName, parameters, uniqueRowFilter);
            if (callTarget is not null)
            {
                functionCall = builder.AttachFunctionCallCallTarget(functionCall, callTarget);
            }

            return PeekKeyword("OVER")
                ? builder.AttachOverClause(functionCall, ParseOverClause())
                : functionCall;
        }

        private BuiltNode ParseCoalesceExpression()
        {
            var arguments = ParseScalarArgumentList();
            return builder.CreateCoalesceExpression(arguments);
        }

        private BuiltNode ParseNullIfExpression()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var firstExpression = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var secondExpression = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateNullIfExpression(firstExpression, secondExpression);
        }

        private BuiltNode ParseIIfCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var predicate = ParseBooleanExpression();
            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var thenExpression = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var elseExpression = ParseScalarExpression();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateIIfCall(predicate, thenExpression, elseExpression);
        }

        private BuiltNode ParseCastCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var parameter = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateCastCall(parameter, dataTypeReference);
        }

        private BuiltNode ParseTryCastCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var parameter = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateTryCastCall(parameter, dataTypeReference);
        }

        private BuiltNode ParseConvertCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var parameter = ParseScalarExpression();
            BuiltNode? style = null;
            if (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                style = ParseScalarExpression();
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateConvertCall(dataTypeReference, parameter, style);
        }

        private BuiltNode ParseTryConvertCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var dataTypeReference = ParseDataTypeReference();
            Expect(MetaTransformScriptSqlTokenKind.Comma);
            var parameter = ParseScalarExpression();
            BuiltNode? style = null;
            if (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                style = ParseScalarExpression();
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateTryConvertCall(dataTypeReference, parameter, style);
        }

        private BuiltNode ParseParseCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var stringValue = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            BuiltNode? culture = null;
            if (MatchKeyword("USING"))
            {
                culture = ParseScalarExpression();
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateParseCall(stringValue, dataTypeReference, culture);
        }

        private BuiltNode ParseTryParseCall()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var stringValue = ParseScalarExpression();
            ExpectKeyword("AS");
            var dataTypeReference = ParseDataTypeReference();
            BuiltNode? culture = null;
            if (MatchKeyword("USING"))
            {
                culture = ParseScalarExpression();
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return builder.CreateTryParseCall(stringValue, dataTypeReference, culture);
        }

        private BuiltNode ParseDataTypeReference()
        {
            var typeNameToken = Current;
            if (typeNameToken.Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                throw ParseError($"Expected an identifier but found '{typeNameToken.Text}'.");
            }

            Advance();
            if (!MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes.TryMapSqlName(typeNameToken.Value, out var mappedType))
            {
                throw Unsupported($"Data type '{typeNameToken.Value}' is not supported yet.");
            }

            List<BuiltNode>? parameters = null;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                parameters = new List<BuiltNode> { ParseDataTypeLiteralParameter() };
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    parameters.Add(ParseDataTypeLiteralParameter());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }

            return builder.CreateSqlDataTypeReference(mappedType, parameters);
        }

        private BuiltNode ParseDataTypeLiteralParameter()
        {
            if (Current.Kind == MetaTransformScriptSqlTokenKind.NumberLiteral)
            {
                var token = Advance();
                return builder.CreateNumberLiteral(token.Value);
            }

            if (PeekKeyword("MAX"))
            {
                Advance();
                return builder.CreateMaxLiteral();
            }

            throw ParseError($"Expected a data type parameter but found '{Current.Text}'.");
        }

        private List<BuiltNode> ParseScalarArgumentList()
        {
            Expect(MetaTransformScriptSqlTokenKind.OpenParen);
            var arguments = new List<BuiltNode> { ParseScalarExpression() };
            while (Match(MetaTransformScriptSqlTokenKind.Comma))
            {
                arguments.Add(ParseScalarExpression());
            }

            Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            return arguments;
        }

        private BuiltNode ParseTrailingCollation(BuiltNode expression)
        {
            while (MatchKeyword("COLLATE"))
            {
                expression = builder.AttachPrimaryExpressionCollation(expression, ParseIdentifier().Node);
            }

            return expression;
        }
    }
}
