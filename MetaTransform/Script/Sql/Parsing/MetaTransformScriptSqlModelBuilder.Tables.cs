using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateNamedTableReference(
        BuiltNode schemaObjectName,
        BuiltNode? alias = null,
        BuiltNode? tableSampleClause = null,
        IReadOnlyList<BuiltNode>? tableHints = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);

        var named = new NamedTableReference
        {
            Id = NextId(nameof(NamedTableReference)),
            TableReferenceWithAlias = aliasBase
        };
        model.NamedTableReferenceList.Add(named);
        model.NamedTableReferenceSchemaObjectLinkList.Add(new NamedTableReferenceSchemaObjectLink
        {
            Id = NextId(nameof(NamedTableReferenceSchemaObjectLink)),
            NamedTableReference = named,
            SchemaObjectName = schemaObjectName.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        if (alias is not null)
        {
            model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
            {
                Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
                TableReferenceWithAlias = aliasBase,
                Identifier = alias.GetRef<Identifier>(nameof(Identifier))
            });
        }

        if (tableHints is not null)
        {
            for (var ordinal = 0; ordinal < tableHints.Count; ordinal++)
            {
                model.TableReferenceWithAliasTableHintsItemList.Add(new TableReferenceWithAliasTableHintsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasTableHintsItem)),
                    TableReferenceWithAlias = aliasBase,
                    SqlHint = tableHints[ordinal].GetRef<SqlHint>(nameof(SqlHint)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        if (tableSampleClause is not null)
        {
            model.NamedTableReferenceTableSampleClauseLinkList.Add(new NamedTableReferenceTableSampleClauseLink
            {
                Id = NextId(nameof(NamedTableReferenceTableSampleClauseLink)),
                NamedTableReference = named,
                TableSampleClause = tableSampleClause.GetRef<TableSampleClause>(nameof(TableSampleClause))
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(NamedTableReference), named.Id));
    }

    public BuiltNode CreateTableSampleClause(BuiltNode sampleNumber, string option, BuiltNode? repeatSeed = null, bool system = false)
    {
        var tableSampleClause = new TableSampleClause
        {
            Id = NextId(nameof(TableSampleClause)),
            System = system ? "true" : string.Empty,
            TableSampleClauseOption = option
        };
        model.TableSampleClauseList.Add(tableSampleClause);

        model.TableSampleClauseSampleNumberLinkList.Add(new TableSampleClauseSampleNumberLink
        {
            Id = NextId(nameof(TableSampleClauseSampleNumberLink)),
            TableSampleClause = tableSampleClause,
            ScalarExpression = sampleNumber.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });

        if (repeatSeed is not null)
        {
            model.TableSampleClauseRepeatSeedLinkList.Add(new TableSampleClauseRepeatSeedLink
            {
                Id = NextId(nameof(TableSampleClauseRepeatSeedLink)),
                TableSampleClause = tableSampleClause,
                ScalarExpression = repeatSeed.GetRef<ScalarExpression>(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create((nameof(TableSampleClause), tableSampleClause.Id));
    }

    public BuiltNode CreateGlobalFunctionTableReference(
        BuiltNode functionName,
        IReadOnlyList<BuiltNode> parameters,
        BuiltNode? alias = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        if (alias is not null)
        {
            model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
            {
                Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
                TableReferenceWithAlias = aliasBase,
                Identifier = alias.GetRef<Identifier>(nameof(Identifier))
            });
        }

        var functionReference = new GlobalFunctionTableReference
        {
            Id = NextId(nameof(GlobalFunctionTableReference)),
            TableReferenceWithAlias = aliasBase
        };
        model.GlobalFunctionTableReferenceList.Add(functionReference);
        model.GlobalFunctionTableReferenceNameLinkList.Add(new GlobalFunctionTableReferenceNameLink
        {
            Id = NextId(nameof(GlobalFunctionTableReferenceNameLink)),
            GlobalFunctionTableReference = functionReference,
            Identifier = functionName.GetRef<Identifier>(nameof(Identifier))
        });

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.GlobalFunctionTableReferenceParametersItemList.Add(new GlobalFunctionTableReferenceParametersItem
            {
                Id = NextId(nameof(GlobalFunctionTableReferenceParametersItem)),
                GlobalFunctionTableReference = functionReference,
                ScalarExpression = parameters[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(GlobalFunctionTableReference), functionReference.Id));
    }

    public BuiltNode CreateSchemaObjectFunctionTableReference(
        BuiltNode schemaObjectName,
        IReadOnlyList<BuiltNode> parameters,
        BuiltNode? alias = null,
        IReadOnlyList<BuiltNode>? columns = null)
    {
        if (alias is null && columns is { Count: > 0 })
        {
            throw new InvalidOperationException("Table-valued function column aliases require a table alias.");
        }

        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        if (alias is not null)
        {
            model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
            {
                Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
                TableReferenceWithAlias = aliasBase,
                Identifier = alias.GetRef<Identifier>(nameof(Identifier))
            });
        }

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            TableReferenceWithAlias = aliasBase
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    TableReferenceWithAliasAndColumns = aliasAndColumns,
                    Identifier = columns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var functionReference = new SchemaObjectFunctionTableReference
        {
            Id = NextId(nameof(SchemaObjectFunctionTableReference)),
            TableReferenceWithAliasAndColumns = aliasAndColumns
        };
        model.SchemaObjectFunctionTableReferenceList.Add(functionReference);
        model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList.Add(new SchemaObjectFunctionTableReferenceSchemaObjectLink
        {
            Id = NextId(nameof(SchemaObjectFunctionTableReferenceSchemaObjectLink)),
            SchemaObjectFunctionTableReference = functionReference,
            SchemaObjectName = schemaObjectName.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.SchemaObjectFunctionTableReferenceParametersItemList.Add(new SchemaObjectFunctionTableReferenceParametersItem
            {
                Id = NextId(nameof(SchemaObjectFunctionTableReferenceParametersItem)),
                SchemaObjectFunctionTableReference = functionReference,
                ScalarExpression = parameters[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(TableReferenceWithAliasAndColumns), aliasAndColumns.Id),
            (nameof(SchemaObjectFunctionTableReference), functionReference.Id));
    }

    public BuiltNode CreateFullTextTableReference(
        string fullTextFunctionType,
        BuiltNode tableName,
        IReadOnlyList<BuiltNode> columns,
        BuiltNode searchCondition,
        BuiltNode alias)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var fullTextTableReference = new FullTextTableReference
        {
            Id = NextId(nameof(FullTextTableReference)),
            TableReferenceWithAlias = aliasBase,
            FullTextFunctionType = fullTextFunctionType
        };
        model.FullTextTableReferenceList.Add(fullTextTableReference);
        model.FullTextTableReferenceTableNameLinkList.Add(new FullTextTableReferenceTableNameLink
        {
            Id = NextId(nameof(FullTextTableReferenceTableNameLink)),
            FullTextTableReference = fullTextTableReference,
            SchemaObjectName = tableName.GetRef<SchemaObjectName>(nameof(SchemaObjectName))
        });

        for (var ordinal = 0; ordinal < columns.Count; ordinal++)
        {
            model.FullTextTableReferenceColumnsItemList.Add(new FullTextTableReferenceColumnsItem
            {
                Id = NextId(nameof(FullTextTableReferenceColumnsItem)),
                FullTextTableReference = fullTextTableReference,
                ColumnReferenceExpression = columns[ordinal].GetRef<ColumnReferenceExpression>(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        model.FullTextTableReferenceSearchConditionLinkList.Add(new FullTextTableReferenceSearchConditionLink
        {
            Id = NextId(nameof(FullTextTableReferenceSearchConditionLink)),
            FullTextTableReference = fullTextTableReference,
            ValueExpression = searchCondition.GetRef<ValueExpression>(nameof(ValueExpression))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(FullTextTableReference), fullTextTableReference.Id));
    }

    public BuiltNode CreateQueryDerivedTable(
        BuiltNode queryExpression,
        BuiltNode alias,
        IReadOnlyList<BuiltNode>? columns = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            TableReferenceWithAlias = aliasBase
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    TableReferenceWithAliasAndColumns = aliasAndColumns,
                    Identifier = columns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var queryDerivedTable = new QueryDerivedTable
        {
            Id = NextId(nameof(QueryDerivedTable)),
            TableReferenceWithAliasAndColumns = aliasAndColumns
        };
        model.QueryDerivedTableList.Add(queryDerivedTable);
        model.QueryDerivedTableQueryExpressionLinkList.Add(new QueryDerivedTableQueryExpressionLink
        {
            Id = NextId(nameof(QueryDerivedTableQueryExpressionLink)),
            QueryDerivedTable = queryDerivedTable,
            QueryExpression = queryExpression.GetRef<QueryExpression>(nameof(QueryExpression))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(TableReferenceWithAliasAndColumns), aliasAndColumns.Id),
            (nameof(QueryDerivedTable), queryDerivedTable.Id));
    }

    public BuiltNode CreateInlineDerivedTable(
        IReadOnlyList<BuiltNode> rowValues,
        BuiltNode alias,
        IReadOnlyList<BuiltNode>? columns = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            TableReferenceWithAlias = aliasBase
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    TableReferenceWithAliasAndColumns = aliasAndColumns,
                    Identifier = columns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var inlineDerivedTable = new InlineDerivedTable
        {
            Id = NextId(nameof(InlineDerivedTable)),
            TableReferenceWithAliasAndColumns = aliasAndColumns
        };
        model.InlineDerivedTableList.Add(inlineDerivedTable);

        for (var ordinal = 0; ordinal < rowValues.Count; ordinal++)
        {
            model.InlineDerivedTableRowValuesItemList.Add(new InlineDerivedTableRowValuesItem
            {
                Id = NextId(nameof(InlineDerivedTableRowValuesItem)),
                InlineDerivedTable = inlineDerivedTable,
                RowValue = rowValues[ordinal].GetRef<RowValue>(nameof(RowValue)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(TableReferenceWithAliasAndColumns), aliasAndColumns.Id),
            (nameof(InlineDerivedTable), inlineDerivedTable.Id));
    }

    public BuiltNode CreateXmlNodesTableReference(
        BuiltNode targetExpression,
        BuiltNode xQueryString,
        BuiltNode alias,
        IReadOnlyList<BuiltNode>? columns = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            TableReferenceWithAlias = aliasBase
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    TableReferenceWithAliasAndColumns = aliasAndColumns,
                    Identifier = columns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var xmlNodesTableReference = new XmlNodesTableReference
        {
            Id = NextId(nameof(XmlNodesTableReference)),
            TableReferenceWithAliasAndColumns = aliasAndColumns
        };
        model.XmlNodesTableReferenceList.Add(xmlNodesTableReference);
        model.XmlNodesTableReferenceTargetExpressionLinkList.Add(new XmlNodesTableReferenceTargetExpressionLink
        {
            Id = NextId(nameof(XmlNodesTableReferenceTargetExpressionLink)),
            XmlNodesTableReference = xmlNodesTableReference,
            ScalarExpression = targetExpression.GetRef<ScalarExpression>(nameof(ScalarExpression))
        });
        model.XmlNodesTableReferenceXQueryStringLinkList.Add(new XmlNodesTableReferenceXQueryStringLink
        {
            Id = NextId(nameof(XmlNodesTableReferenceXQueryStringLink)),
            XmlNodesTableReference = xmlNodesTableReference,
            StringLiteral = xQueryString.GetRef<StringLiteral>(nameof(StringLiteral))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(TableReferenceWithAliasAndColumns), aliasAndColumns.Id),
            (nameof(XmlNodesTableReference), xmlNodesTableReference.Id));
    }

    public BuiltNode CreateJoinParenthesisTableReference(BuiltNode join)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var joinParenthesisTableReference = new JoinParenthesisTableReference
        {
            Id = NextId(nameof(JoinParenthesisTableReference)),
            TableReference = tableReference
        };
        model.JoinParenthesisTableReferenceList.Add(joinParenthesisTableReference);
        model.JoinParenthesisTableReferenceJoinLinkList.Add(new JoinParenthesisTableReferenceJoinLink
        {
            Id = NextId(nameof(JoinParenthesisTableReferenceJoinLink)),
            JoinParenthesisTableReference = joinParenthesisTableReference,
            TableReference = join.GetRef<TableReference>(nameof(TableReference))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(JoinParenthesisTableReference), joinParenthesisTableReference.Id));
    }

    public BuiltNode CreateRowValue(IReadOnlyList<BuiltNode> columnValues)
    {
        var rowValue = new RowValue
        {
            Id = NextId(nameof(RowValue))
        };
        model.RowValueList.Add(rowValue);

        for (var ordinal = 0; ordinal < columnValues.Count; ordinal++)
        {
            model.RowValueColumnValuesItemList.Add(new RowValueColumnValuesItem
            {
                Id = NextId(nameof(RowValueColumnValuesItem)),
                RowValue = rowValue,
                ScalarExpression = columnValues[ordinal].GetRef<ScalarExpression>(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(RowValue), rowValue.Id));
    }

    public BuiltNode CreatePivotedTableReference(
        BuiltNode sourceTableReference,
        BuiltNode aggregateFunctionIdentifier,
        IReadOnlyList<BuiltNode> valueColumns,
        BuiltNode pivotColumn,
        IReadOnlyList<BuiltNode> inColumns,
        BuiltNode alias)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var pivotedTableReference = new PivotedTableReference
        {
            Id = NextId(nameof(PivotedTableReference)),
            TableReferenceWithAlias = aliasBase
        };
        model.PivotedTableReferenceList.Add(pivotedTableReference);
        model.PivotedTableReferenceTableReferenceLinkList.Add(new PivotedTableReferenceTableReferenceLink
        {
            Id = NextId(nameof(PivotedTableReferenceTableReferenceLink)),
            PivotedTableReference = pivotedTableReference,
            TableReference = sourceTableReference.GetRef<TableReference>(nameof(TableReference))
        });
        model.PivotedTableReferenceAggregateFunctionIdentifierLinkList.Add(new PivotedTableReferenceAggregateFunctionIdentifierLink
        {
            Id = NextId(nameof(PivotedTableReferenceAggregateFunctionIdentifierLink)),
            PivotedTableReference = pivotedTableReference,
            MultiPartIdentifier = aggregateFunctionIdentifier.GetRef<MultiPartIdentifier>(nameof(MultiPartIdentifier))
        });
        model.PivotedTableReferencePivotColumnLinkList.Add(new PivotedTableReferencePivotColumnLink
        {
            Id = NextId(nameof(PivotedTableReferencePivotColumnLink)),
            PivotedTableReference = pivotedTableReference,
            ColumnReferenceExpression = pivotColumn.GetRef<ColumnReferenceExpression>(nameof(ColumnReferenceExpression))
        });

        for (var ordinal = 0; ordinal < valueColumns.Count; ordinal++)
        {
            model.PivotedTableReferenceValueColumnsItemList.Add(new PivotedTableReferenceValueColumnsItem
            {
                Id = NextId(nameof(PivotedTableReferenceValueColumnsItem)),
                PivotedTableReference = pivotedTableReference,
                ColumnReferenceExpression = valueColumns[ordinal].GetRef<ColumnReferenceExpression>(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        for (var ordinal = 0; ordinal < inColumns.Count; ordinal++)
        {
            model.PivotedTableReferenceInColumnsItemList.Add(new PivotedTableReferenceInColumnsItem
            {
                Id = NextId(nameof(PivotedTableReferenceInColumnsItem)),
                PivotedTableReference = pivotedTableReference,
                Identifier = inColumns[ordinal].GetRef<Identifier>(nameof(Identifier)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(PivotedTableReference), pivotedTableReference.Id));
    }

    public BuiltNode CreateUnpivotedTableReference(
        BuiltNode sourceTableReference,
        BuiltNode valueColumn,
        BuiltNode pivotColumn,
        IReadOnlyList<BuiltNode> inColumns,
        BuiltNode alias)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            TableReference = tableReference
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            TableReferenceWithAlias = aliasBase,
            Identifier = alias.GetRef<Identifier>(nameof(Identifier))
        });

        var unpivotedTableReference = new UnpivotedTableReference
        {
            Id = NextId(nameof(UnpivotedTableReference)),
            TableReferenceWithAlias = aliasBase
        };
        model.UnpivotedTableReferenceList.Add(unpivotedTableReference);
        model.UnpivotedTableReferenceTableReferenceLinkList.Add(new UnpivotedTableReferenceTableReferenceLink
        {
            Id = NextId(nameof(UnpivotedTableReferenceTableReferenceLink)),
            UnpivotedTableReference = unpivotedTableReference,
            TableReference = sourceTableReference.GetRef<TableReference>(nameof(TableReference))
        });
        model.UnpivotedTableReferenceValueColumnLinkList.Add(new UnpivotedTableReferenceValueColumnLink
        {
            Id = NextId(nameof(UnpivotedTableReferenceValueColumnLink)),
            UnpivotedTableReference = unpivotedTableReference,
            Identifier = valueColumn.GetRef<Identifier>(nameof(Identifier))
        });
        model.UnpivotedTableReferencePivotColumnLinkList.Add(new UnpivotedTableReferencePivotColumnLink
        {
            Id = NextId(nameof(UnpivotedTableReferencePivotColumnLink)),
            UnpivotedTableReference = unpivotedTableReference,
            Identifier = pivotColumn.GetRef<Identifier>(nameof(Identifier))
        });

        for (var ordinal = 0; ordinal < inColumns.Count; ordinal++)
        {
            model.UnpivotedTableReferenceInColumnsItemList.Add(new UnpivotedTableReferenceInColumnsItem
            {
                Id = NextId(nameof(UnpivotedTableReferenceInColumnsItem)),
                UnpivotedTableReference = unpivotedTableReference,
                ColumnReferenceExpression = inColumns[ordinal].GetRef<ColumnReferenceExpression>(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(UnpivotedTableReference), unpivotedTableReference.Id));
    }

    public BuiltNode CreateQualifiedJoin(BuiltNode firstTableReference, BuiltNode secondTableReference, string joinType, BuiltNode searchCondition)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var joinBase = new JoinTableReference
        {
            Id = NextId(nameof(JoinTableReference)),
            TableReference = tableReference
        };
        model.JoinTableReferenceList.Add(joinBase);

        var qualified = new QualifiedJoin
        {
            Id = NextId(nameof(QualifiedJoin)),
            JoinTableReference = joinBase,
            QualifiedJoinType = joinType
        };
        model.QualifiedJoinList.Add(qualified);
        model.JoinTableReferenceFirstTableReferenceLinkList.Add(new JoinTableReferenceFirstTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceFirstTableReferenceLink)),
            JoinTableReference = joinBase,
            TableReference = firstTableReference.GetRef<TableReference>(nameof(TableReference))
        });
        model.JoinTableReferenceSecondTableReferenceLinkList.Add(new JoinTableReferenceSecondTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceSecondTableReferenceLink)),
            JoinTableReference = joinBase,
            TableReference = secondTableReference.GetRef<TableReference>(nameof(TableReference))
        });
        model.QualifiedJoinSearchConditionLinkList.Add(new QualifiedJoinSearchConditionLink
        {
            Id = NextId(nameof(QualifiedJoinSearchConditionLink)),
            QualifiedJoin = qualified,
            BooleanExpression = searchCondition.GetRef<BooleanExpression>(nameof(BooleanExpression))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(JoinTableReference), joinBase.Id),
            (nameof(QualifiedJoin), qualified.Id));
    }

    public BuiltNode CreateUnqualifiedJoin(BuiltNode firstTableReference, BuiltNode secondTableReference, string joinType)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var joinBase = new JoinTableReference
        {
            Id = NextId(nameof(JoinTableReference)),
            TableReference = tableReference
        };
        model.JoinTableReferenceList.Add(joinBase);

        var unqualified = new UnqualifiedJoin
        {
            Id = NextId(nameof(UnqualifiedJoin)),
            JoinTableReference = joinBase,
            UnqualifiedJoinType = joinType
        };
        model.UnqualifiedJoinList.Add(unqualified);
        model.JoinTableReferenceFirstTableReferenceLinkList.Add(new JoinTableReferenceFirstTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceFirstTableReferenceLink)),
            JoinTableReference = joinBase,
            TableReference = firstTableReference.GetRef<TableReference>(nameof(TableReference))
        });
        model.JoinTableReferenceSecondTableReferenceLinkList.Add(new JoinTableReferenceSecondTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceSecondTableReferenceLink)),
            JoinTableReference = joinBase,
            TableReference = secondTableReference.GetRef<TableReference>(nameof(TableReference))
        });

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(JoinTableReference), joinBase.Id),
            (nameof(UnqualifiedJoin), unqualified.Id));
    }

    public BuiltNode CreateFromClause(IReadOnlyList<BuiltNode> tableReferences)
    {
        var row = new FromClause
        {
            Id = NextId(nameof(FromClause))
        };
        model.FromClauseList.Add(row);

        for (var ordinal = 0; ordinal < tableReferences.Count; ordinal++)
        {
            model.FromClauseTableReferencesItemList.Add(new FromClauseTableReferencesItem
            {
                Id = NextId(nameof(FromClauseTableReferencesItem)),
                FromClause = row,
                TableReference = tableReferences[ordinal].GetRef<TableReference>(nameof(TableReference)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(FromClause), row.Id));
    }
}
