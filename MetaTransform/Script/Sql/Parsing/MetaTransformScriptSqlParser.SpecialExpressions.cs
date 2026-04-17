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
                "LEFT" => ParseTrailingScalarSuffixes(ParseLeftFunctionCall()),
                "RIGHT" => ParseTrailingScalarSuffixes(ParseRightFunctionCall()),
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

            if (MatchKeyword("WITHIN"))
            {
                ExpectKeyword("GROUP");
                Expect(MetaTransformScriptSqlTokenKind.OpenParen);
                ExpectKeyword("ORDER");
                ExpectKeyword("BY");
                var orderByClause = ParseOrderByClause();
                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
                functionCall = builder.AttachWithinGroupOrderByClause(functionCall, orderByClause);
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

        private BuiltNode ParseLeftFunctionCall()
        {
            var arguments = ParseScalarArgumentList();
            return builder.CreateLeftFunctionCall(arguments);
        }

        private BuiltNode ParseRightFunctionCall()
        {
            var arguments = ParseScalarArgumentList();
            return builder.CreateRightFunctionCall(arguments);
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
            var (typeName, mappedType) = ParseSupportedDataTypeName();

            List<BuiltNode>? parameters = null;
            if (Match(MetaTransformScriptSqlTokenKind.OpenParen))
            {
                parameters = [ParseDataTypeLiteralParameter()];
                while (Match(MetaTransformScriptSqlTokenKind.Comma))
                {
                    parameters.Add(ParseDataTypeLiteralParameter());
                }

                Expect(MetaTransformScriptSqlTokenKind.CloseParen);
            }
            else
            {
                var defaultParameters = MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes
                    .GetDefaultParametersForSqlName(typeName);
                if (defaultParameters.Count > 0)
                {
                    parameters = defaultParameters
                        .Select(builder.CreateNumberLiteral)
                        .ToList();
                }
            }

            return builder.CreateSqlDataTypeReference(mappedType, parameters);
        }

        private (string TypeName, string MappedType) ParseSupportedDataTypeName()
        {
            if (Current.Kind != MetaTransformScriptSqlTokenKind.Identifier)
            {
                throw ParseError($"Expected an identifier but found '{Current.Text}'.");
            }

            var candidateParts = new List<string>();
            var bestMatchedTokenCount = 0;
            var bestMappedType = string.Empty;
            var scanPosition = position;

            while (scanPosition < tokens.Count &&
                   tokens[scanPosition].Kind == MetaTransformScriptSqlTokenKind.Identifier)
            {
                candidateParts.Add(tokens[scanPosition].Value);
                var candidateName = string.Join(" ", candidateParts);
                if (MetaTransformScript.Sql.MetaTransformScriptSqlServerDataTypes.TryMapSqlName(candidateName, out var mappedType))
                {
                    bestMatchedTokenCount = candidateParts.Count;
                    bestMappedType = mappedType;
                }

                scanPosition++;
            }

            var attemptedTypeName = string.Join(" ", candidateParts);
            if (bestMatchedTokenCount == 0)
            {
                throw Unsupported($"Data type '{attemptedTypeName}' is not supported yet.");
            }

            var matchedTypeName = string.Join(" ", candidateParts.Take(bestMatchedTokenCount));
            for (var ordinal = 0; ordinal < bestMatchedTokenCount; ordinal++)
            {
                Advance();
            }

            return (matchedTypeName, bestMappedType);
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
