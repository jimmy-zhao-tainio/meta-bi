using System.Globalization;
using MetaTransformScript;

namespace MetaTransformScript.Sql.Parsing;

internal sealed partial class MetaTransformScriptSqlModelBuilder
{
    public BuiltNode CreateNamedTableReference(BuiltNode schemaObjectName, BuiltNode? alias = null, BuiltNode? tableSampleClause = null)
    {
        var tableReference = new TableReference
        {
            Id = NextId(nameof(TableReference))
        };
        model.TableReferenceList.Add(tableReference);

        var aliasBase = new TableReferenceWithAlias
        {
            Id = NextId(nameof(TableReferenceWithAlias)),
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);

        var named = new NamedTableReference
        {
            Id = NextId(nameof(NamedTableReference)),
            BaseId = aliasBase.Id
        };
        model.NamedTableReferenceList.Add(named);
        model.NamedTableReferenceSchemaObjectLinkList.Add(new NamedTableReferenceSchemaObjectLink
        {
            Id = NextId(nameof(NamedTableReferenceSchemaObjectLink)),
            OwnerId = named.Id,
            ValueId = schemaObjectName.GetId(nameof(SchemaObjectName))
        });

        if (alias is not null)
        {
            model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
            {
                Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
                OwnerId = aliasBase.Id,
                ValueId = alias.GetId(nameof(Identifier))
            });
        }

        if (tableSampleClause is not null)
        {
            model.NamedTableReferenceTableSampleClauseLinkList.Add(new NamedTableReferenceTableSampleClauseLink
            {
                Id = NextId(nameof(NamedTableReferenceTableSampleClauseLink)),
                OwnerId = named.Id,
                ValueId = tableSampleClause.GetId(nameof(TableSampleClause))
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
            OwnerId = tableSampleClause.Id,
            ValueId = sampleNumber.GetId(nameof(ScalarExpression))
        });

        if (repeatSeed is not null)
        {
            model.TableSampleClauseRepeatSeedLinkList.Add(new TableSampleClauseRepeatSeedLink
            {
                Id = NextId(nameof(TableSampleClauseRepeatSeedLink)),
                OwnerId = tableSampleClause.Id,
                ValueId = repeatSeed.GetId(nameof(ScalarExpression))
            });
        }

        return BuiltNode.Create((nameof(TableSampleClause), tableSampleClause.Id));
    }

    public BuiltNode CreateSchemaObjectFunctionTableReference(
        BuiltNode schemaObjectName,
        IReadOnlyList<BuiltNode> parameters,
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            BaseId = aliasBase.Id
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    OwnerId = aliasAndColumns.Id,
                    ValueId = columns[ordinal].GetId(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var functionReference = new SchemaObjectFunctionTableReference
        {
            Id = NextId(nameof(SchemaObjectFunctionTableReference)),
            BaseId = aliasAndColumns.Id
        };
        model.SchemaObjectFunctionTableReferenceList.Add(functionReference);
        model.SchemaObjectFunctionTableReferenceSchemaObjectLinkList.Add(new SchemaObjectFunctionTableReferenceSchemaObjectLink
        {
            Id = NextId(nameof(SchemaObjectFunctionTableReferenceSchemaObjectLink)),
            OwnerId = functionReference.Id,
            ValueId = schemaObjectName.GetId(nameof(SchemaObjectName))
        });

        for (var ordinal = 0; ordinal < parameters.Count; ordinal++)
        {
            model.SchemaObjectFunctionTableReferenceParametersItemList.Add(new SchemaObjectFunctionTableReferenceParametersItem
            {
                Id = NextId(nameof(SchemaObjectFunctionTableReferenceParametersItem)),
                OwnerId = functionReference.Id,
                ValueId = parameters[ordinal].GetId(nameof(ScalarExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create(
            (nameof(TableReference), tableReference.Id),
            (nameof(TableReferenceWithAlias), aliasBase.Id),
            (nameof(TableReferenceWithAliasAndColumns), aliasAndColumns.Id),
            (nameof(SchemaObjectFunctionTableReference), functionReference.Id));
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            BaseId = aliasBase.Id
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    OwnerId = aliasAndColumns.Id,
                    ValueId = columns[ordinal].GetId(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var queryDerivedTable = new QueryDerivedTable
        {
            Id = NextId(nameof(QueryDerivedTable)),
            BaseId = aliasAndColumns.Id
        };
        model.QueryDerivedTableList.Add(queryDerivedTable);
        model.QueryDerivedTableQueryExpressionLinkList.Add(new QueryDerivedTableQueryExpressionLink
        {
            Id = NextId(nameof(QueryDerivedTableQueryExpressionLink)),
            OwnerId = queryDerivedTable.Id,
            ValueId = queryExpression.GetId(nameof(QueryExpression))
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            BaseId = aliasBase.Id
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    OwnerId = aliasAndColumns.Id,
                    ValueId = columns[ordinal].GetId(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var inlineDerivedTable = new InlineDerivedTable
        {
            Id = NextId(nameof(InlineDerivedTable)),
            BaseId = aliasAndColumns.Id
        };
        model.InlineDerivedTableList.Add(inlineDerivedTable);

        for (var ordinal = 0; ordinal < rowValues.Count; ordinal++)
        {
            model.InlineDerivedTableRowValuesItemList.Add(new InlineDerivedTableRowValuesItem
            {
                Id = NextId(nameof(InlineDerivedTableRowValuesItem)),
                OwnerId = inlineDerivedTable.Id,
                ValueId = rowValues[ordinal].GetId(nameof(RowValue)),
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var aliasAndColumns = new TableReferenceWithAliasAndColumns
        {
            Id = NextId(nameof(TableReferenceWithAliasAndColumns)),
            BaseId = aliasBase.Id
        };
        model.TableReferenceWithAliasAndColumnsList.Add(aliasAndColumns);

        if (columns is not null)
        {
            for (var ordinal = 0; ordinal < columns.Count; ordinal++)
            {
                model.TableReferenceWithAliasAndColumnsColumnsItemList.Add(new TableReferenceWithAliasAndColumnsColumnsItem
                {
                    Id = NextId(nameof(TableReferenceWithAliasAndColumnsColumnsItem)),
                    OwnerId = aliasAndColumns.Id,
                    ValueId = columns[ordinal].GetId(nameof(Identifier)),
                    Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
                });
            }
        }

        var xmlNodesTableReference = new XmlNodesTableReference
        {
            Id = NextId(nameof(XmlNodesTableReference)),
            BaseId = aliasAndColumns.Id
        };
        model.XmlNodesTableReferenceList.Add(xmlNodesTableReference);
        model.XmlNodesTableReferenceTargetExpressionLinkList.Add(new XmlNodesTableReferenceTargetExpressionLink
        {
            Id = NextId(nameof(XmlNodesTableReferenceTargetExpressionLink)),
            OwnerId = xmlNodesTableReference.Id,
            ValueId = targetExpression.GetId(nameof(ScalarExpression))
        });
        model.XmlNodesTableReferenceXQueryStringLinkList.Add(new XmlNodesTableReferenceXQueryStringLink
        {
            Id = NextId(nameof(XmlNodesTableReferenceXQueryStringLink)),
            OwnerId = xmlNodesTableReference.Id,
            ValueId = xQueryString.GetId(nameof(StringLiteral))
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
            BaseId = tableReference.Id
        };
        model.JoinParenthesisTableReferenceList.Add(joinParenthesisTableReference);
        model.JoinParenthesisTableReferenceJoinLinkList.Add(new JoinParenthesisTableReferenceJoinLink
        {
            Id = NextId(nameof(JoinParenthesisTableReferenceJoinLink)),
            OwnerId = joinParenthesisTableReference.Id,
            ValueId = join.GetId(nameof(TableReference))
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
                OwnerId = rowValue.Id,
                ValueId = columnValues[ordinal].GetId(nameof(ScalarExpression)),
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var pivotedTableReference = new PivotedTableReference
        {
            Id = NextId(nameof(PivotedTableReference)),
            BaseId = aliasBase.Id
        };
        model.PivotedTableReferenceList.Add(pivotedTableReference);
        model.PivotedTableReferenceTableReferenceLinkList.Add(new PivotedTableReferenceTableReferenceLink
        {
            Id = NextId(nameof(PivotedTableReferenceTableReferenceLink)),
            OwnerId = pivotedTableReference.Id,
            ValueId = sourceTableReference.GetId(nameof(TableReference))
        });
        model.PivotedTableReferenceAggregateFunctionIdentifierLinkList.Add(new PivotedTableReferenceAggregateFunctionIdentifierLink
        {
            Id = NextId(nameof(PivotedTableReferenceAggregateFunctionIdentifierLink)),
            OwnerId = pivotedTableReference.Id,
            ValueId = aggregateFunctionIdentifier.GetId(nameof(MultiPartIdentifier))
        });
        model.PivotedTableReferencePivotColumnLinkList.Add(new PivotedTableReferencePivotColumnLink
        {
            Id = NextId(nameof(PivotedTableReferencePivotColumnLink)),
            OwnerId = pivotedTableReference.Id,
            ValueId = pivotColumn.GetId(nameof(ColumnReferenceExpression))
        });

        for (var ordinal = 0; ordinal < valueColumns.Count; ordinal++)
        {
            model.PivotedTableReferenceValueColumnsItemList.Add(new PivotedTableReferenceValueColumnsItem
            {
                Id = NextId(nameof(PivotedTableReferenceValueColumnsItem)),
                OwnerId = pivotedTableReference.Id,
                ValueId = valueColumns[ordinal].GetId(nameof(ColumnReferenceExpression)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        for (var ordinal = 0; ordinal < inColumns.Count; ordinal++)
        {
            model.PivotedTableReferenceInColumnsItemList.Add(new PivotedTableReferenceInColumnsItem
            {
                Id = NextId(nameof(PivotedTableReferenceInColumnsItem)),
                OwnerId = pivotedTableReference.Id,
                ValueId = inColumns[ordinal].GetId(nameof(Identifier)),
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
            BaseId = tableReference.Id
        };
        model.TableReferenceWithAliasList.Add(aliasBase);
        model.TableReferenceWithAliasAliasLinkList.Add(new TableReferenceWithAliasAliasLink
        {
            Id = NextId(nameof(TableReferenceWithAliasAliasLink)),
            OwnerId = aliasBase.Id,
            ValueId = alias.GetId(nameof(Identifier))
        });

        var unpivotedTableReference = new UnpivotedTableReference
        {
            Id = NextId(nameof(UnpivotedTableReference)),
            BaseId = aliasBase.Id
        };
        model.UnpivotedTableReferenceList.Add(unpivotedTableReference);
        model.UnpivotedTableReferenceTableReferenceLinkList.Add(new UnpivotedTableReferenceTableReferenceLink
        {
            Id = NextId(nameof(UnpivotedTableReferenceTableReferenceLink)),
            OwnerId = unpivotedTableReference.Id,
            ValueId = sourceTableReference.GetId(nameof(TableReference))
        });
        model.UnpivotedTableReferenceValueColumnLinkList.Add(new UnpivotedTableReferenceValueColumnLink
        {
            Id = NextId(nameof(UnpivotedTableReferenceValueColumnLink)),
            OwnerId = unpivotedTableReference.Id,
            ValueId = valueColumn.GetId(nameof(Identifier))
        });
        model.UnpivotedTableReferencePivotColumnLinkList.Add(new UnpivotedTableReferencePivotColumnLink
        {
            Id = NextId(nameof(UnpivotedTableReferencePivotColumnLink)),
            OwnerId = unpivotedTableReference.Id,
            ValueId = pivotColumn.GetId(nameof(Identifier))
        });

        for (var ordinal = 0; ordinal < inColumns.Count; ordinal++)
        {
            model.UnpivotedTableReferenceInColumnsItemList.Add(new UnpivotedTableReferenceInColumnsItem
            {
                Id = NextId(nameof(UnpivotedTableReferenceInColumnsItem)),
                OwnerId = unpivotedTableReference.Id,
                ValueId = inColumns[ordinal].GetId(nameof(ColumnReferenceExpression)),
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
            BaseId = tableReference.Id
        };
        model.JoinTableReferenceList.Add(joinBase);

        var qualified = new QualifiedJoin
        {
            Id = NextId(nameof(QualifiedJoin)),
            BaseId = joinBase.Id,
            QualifiedJoinType = joinType
        };
        model.QualifiedJoinList.Add(qualified);
        model.JoinTableReferenceFirstTableReferenceLinkList.Add(new JoinTableReferenceFirstTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceFirstTableReferenceLink)),
            OwnerId = joinBase.Id,
            ValueId = firstTableReference.GetId(nameof(TableReference))
        });
        model.JoinTableReferenceSecondTableReferenceLinkList.Add(new JoinTableReferenceSecondTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceSecondTableReferenceLink)),
            OwnerId = joinBase.Id,
            ValueId = secondTableReference.GetId(nameof(TableReference))
        });
        model.QualifiedJoinSearchConditionLinkList.Add(new QualifiedJoinSearchConditionLink
        {
            Id = NextId(nameof(QualifiedJoinSearchConditionLink)),
            OwnerId = qualified.Id,
            ValueId = searchCondition.GetId(nameof(BooleanExpression))
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
            BaseId = tableReference.Id
        };
        model.JoinTableReferenceList.Add(joinBase);

        var unqualified = new UnqualifiedJoin
        {
            Id = NextId(nameof(UnqualifiedJoin)),
            BaseId = joinBase.Id,
            UnqualifiedJoinType = joinType
        };
        model.UnqualifiedJoinList.Add(unqualified);
        model.JoinTableReferenceFirstTableReferenceLinkList.Add(new JoinTableReferenceFirstTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceFirstTableReferenceLink)),
            OwnerId = joinBase.Id,
            ValueId = firstTableReference.GetId(nameof(TableReference))
        });
        model.JoinTableReferenceSecondTableReferenceLinkList.Add(new JoinTableReferenceSecondTableReferenceLink
        {
            Id = NextId(nameof(JoinTableReferenceSecondTableReferenceLink)),
            OwnerId = joinBase.Id,
            ValueId = secondTableReference.GetId(nameof(TableReference))
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
                OwnerId = row.Id,
                ValueId = tableReferences[ordinal].GetId(nameof(TableReference)),
                Ordinal = ordinal.ToString(CultureInfo.InvariantCulture)
            });
        }

        return BuiltNode.Create((nameof(FromClause), row.Id));
    }
}
