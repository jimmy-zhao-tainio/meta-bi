from __future__ import annotations

import math
import os
import xml.etree.ElementTree as ET
from collections import defaultdict
from xml.sax.saxutils import escape


MODEL_PATH = os.path.join("MetaTransformScript", "Workspaces", "MetaTransformScript", "model.xml")
OUTPUT_PATH = os.path.join("docs", "images", "meta-transform-script-entity-graph.svg")

BACKGROUND = "#ffffff"
TEXT = "#1f2937"
MUTED = "#6b7280"
PANEL = "#f8fafc"
PANEL_STROKE = "#cbd5e1"
COLORS = {
    "query": ("#dbeafe", "#93c5fd", "#1d4ed8"),
    "source": ("#dcfce7", "#86efac", "#166534"),
    "expression": ("#fee2e2", "#fca5a5", "#b91c1c"),
    "support": ("#fef3c7", "#fcd34d", "#92400e"),
    "link": ("#ede9fe", "#c4b5fd", "#6d28d9"),
    "item": ("#cffafe", "#67e8f9", "#0f766e"),
}

NODE_WIDTH = 270
NODE_HEIGHT = 24
NODE_RX = 6
COLUMN_GAP = 26
ROW_GAP = 8
PANEL_PADDING = 20
HEADER_HEIGHT = 60
LEGEND_HEIGHT = 78
LEFT_MARGIN = 36
TOP_MARGIN = 36
FONT_FAMILY = "Segoe UI, Arial, sans-serif"


def load_model(path: str) -> list[str]:
    root = ET.parse(path).getroot()
    entities = []
    for entity in root.findall("./EntityList/Entity"):
        entities.append(entity.attrib["name"])

    return entities


def classify_entity(name: str) -> str:
    if name.endswith("Link"):
        return "link"
    if name.endswith("Item"):
        return "item"

    query_keywords = (
        "TransformScript",
        "TSqlStatement",
        "StatementWith",
        "SelectStatement",
        "SelectElement",
        "QueryExpression",
        "QuerySpecification",
        "CommonTableExpression",
        "FromClause",
        "WhereClause",
        "HavingClause",
        "GroupByClause",
        "GroupingSpecification",
        "GroupingSets",
        "Rollup",
        "Cube",
        "CompositeGroupingSpecification",
        "GrandTotalGroupingSpecification",
        "OrderByClause",
        "OffsetClause",
        "TopRowFilter",
        "WindowClause",
        "WindowDefinition",
        "WindowFrameClause",
        "WindowDelimiter",
        "ExpressionWithSortOrder",
        "XmlNamespaces",
    )
    if any(keyword in name for keyword in query_keywords):
        return "query"

    source_keywords = (
        "TableReference",
        "Join",
        "FromClause",
        "NamedTableReference",
        "SchemaObjectFunctionTableReference",
        "GlobalFunctionTableReference",
        "FullTextTableReference",
        "QueryDerivedTable",
        "InlineDerivedTable",
        "PivotedTableReference",
        "UnpivotedTableReference",
        "TableSampleClause",
        "SchemaObjectName",
        "CallTarget",
    )
    if any(keyword in name for keyword in source_keywords):
        return "source"

    support_keywords = (
        "Identifier",
        "Literal",
        "DataTypeReference",
        "MultiPartIdentifier",
        "IdentifierOrValueExpression",
    )
    if any(keyword in name for keyword in support_keywords):
        return "support"

    return "expression"


def split_columns(items: list[str], column_count: int) -> list[list[str]]:
    if not items:
        return [[] for _ in range(column_count)]

    per_col = math.ceil(len(items) / column_count)
    columns: list[list[str]] = []
    for index in range(column_count):
        start = index * per_col
        end = start + per_col
        columns.append(items[start:end])
    return columns


def build_columns(entities: list[str]) -> list[tuple[str, str, list[str]]]:
    by_category: dict[str, list[str]] = defaultdict(list)
    for entity in entities:
        by_category[classify_entity(entity)].append(entity)

    for values in by_category.values():
        values.sort(key=lambda item: (family_sort_key(item), item.lower()))

    columns: list[tuple[str, str, list[str]]] = [
        ("Query And Script", "query", by_category["query"]),
        ("Sources And Joins", "source", by_category["source"]),
        ("Expressions And Predicates", "expression", by_category["expression"]),
        ("Names, Literals, Types", "support", by_category["support"]),
    ]

    for index, group in enumerate(split_columns(by_category["link"], 3), start=1):
        columns.append((f"Link Entities {index}", "link", group))

    item_columns = split_columns(by_category["item"], 2)
    for index, group in enumerate(item_columns, start=1):
        columns.append((f"Item Entities {index}", "item", group))

    return columns


def family_sort_key(name: str) -> tuple[int, str]:
    prefixes = [
        "TransformScript",
        "StatementWith",
        "SelectStatement",
        "QueryExpression",
        "QuerySpecification",
        "Select",
        "FromClause",
        "TableReference",
        "Join",
        "WhereClause",
        "Boolean",
        "Scalar",
        "ValueExpression",
        "FunctionCall",
        "CaseExpression",
        "WhenClause",
        "Window",
        "OrderByClause",
        "OffsetClause",
        "TopRowFilter",
        "Identifier",
        "Literal",
        "DataTypeReference",
    ]
    for index, prefix in enumerate(prefixes):
        if name.startswith(prefix):
            return index, name

    return len(prefixes), name


def layout(columns: list[tuple[str, str, list[str]]]) -> tuple[dict[str, tuple[float, float]], dict[int, tuple[float, float, float, float]], float, float]:
    positions: dict[str, tuple[float, float]] = {}
    panel_boxes: dict[int, tuple[float, float, float, float]] = {}
    x = LEFT_MARGIN
    max_height = 0.0

    for panel_index, (_, _, nodes) in enumerate(columns):
        col_height = HEADER_HEIGHT + PANEL_PADDING * 2
        if nodes:
            col_height += len(nodes) * NODE_HEIGHT + max(0, len(nodes) - 1) * ROW_GAP

        panel_height = col_height
        max_height = max(max_height, panel_height)
        panel_boxes[panel_index] = (x, TOP_MARGIN, NODE_WIDTH + PANEL_PADDING * 2, panel_height)

        node_x = x + PANEL_PADDING
        node_y = TOP_MARGIN + HEADER_HEIGHT
        for node in nodes:
            positions[node] = (node_x, node_y)
            node_y += NODE_HEIGHT + ROW_GAP

        x += NODE_WIDTH + PANEL_PADDING * 2 + COLUMN_GAP

    width = x - COLUMN_GAP + LEFT_MARGIN
    height = TOP_MARGIN + max_height + LEGEND_HEIGHT + 28
    return positions, panel_boxes, width, height


def render_svg(entities: list[str]) -> str:
    columns = build_columns(entities)
    positions, panel_boxes, width, height = layout(columns)

    parts = [
        f'<svg xmlns="http://www.w3.org/2000/svg" width="{width:.0f}" height="{height:.0f}" viewBox="0 0 {width:.0f} {height:.0f}">',
        f'<rect width="100%" height="100%" fill="{BACKGROUND}" />',
        f'<text x="{LEFT_MARGIN}" y="30" font-family="{FONT_FAMILY}" font-size="22" font-weight="700" fill="{TEXT}">MetaTransformScript entity overview</text>',
        f'<text x="{LEFT_MARGIN}" y="50" font-family="{FONT_FAMILY}" font-size="12" fill="{MUTED}">Generated from MetaTransformScript/Workspaces/MetaTransformScript/model.xml - {len(entities)} entities grouped by role</text>',
    ]

    for panel_index, (title, category, nodes) in enumerate(columns):
        x, y, w, h = panel_boxes[panel_index]
        fill, stroke, text_color = COLORS[category]
        parts.append(
            f'<rect x="{x}" y="{y}" width="{w}" height="{h}" rx="10" ry="10" fill="{PANEL}" stroke="{PANEL_STROKE}" />'
        )
        parts.append(
            f'<text x="{x + PANEL_PADDING}" y="{y + 26}" font-family="{FONT_FAMILY}" font-size="14" font-weight="700" fill="{text_color}">{escape(title)}</text>'
        )
        parts.append(
            f'<text x="{x + PANEL_PADDING}" y="{y + 44}" font-family="{FONT_FAMILY}" font-size="11" fill="{MUTED}">{len(nodes)} entities</text>'
        )

        for node in nodes:
            nx, ny = positions[node]
            parts.append(
                f'<rect x="{nx}" y="{ny}" width="{NODE_WIDTH}" height="{NODE_HEIGHT}" rx="{NODE_RX}" ry="{NODE_RX}" fill="{fill}" stroke="{stroke}" />'
            )
            parts.append(
                f'<text x="{nx + 8}" y="{ny + 16}" font-family="{FONT_FAMILY}" font-size="11" fill="{TEXT}">{escape(node)}</text>'
            )

    legend_y = height - LEGEND_HEIGHT + 16
    parts.append(f'<text x="{LEFT_MARGIN}" y="{legend_y}" font-family="{FONT_FAMILY}" font-size="14" font-weight="700" fill="{TEXT}">Legend</text>')
    legend_items = [
        ("query", "Query/script entities"),
        ("source", "Table/source entities"),
        ("expression", "Expression/predicate entities"),
        ("support", "Names, literals, and type entities"),
        ("link", "Optional/structured relationship entities"),
        ("item", "Ordered collection entities"),
    ]
    lx = LEFT_MARGIN
    ly = legend_y + 16
    for index, (category, label) in enumerate(legend_items):
        fill, stroke, _ = COLORS[category]
        item_x = lx + (index % 3) * 360
        item_y = ly + (index // 3) * 24
        parts.append(
            f'<rect x="{item_x}" y="{item_y - 12}" width="18" height="12" rx="3" ry="3" fill="{fill}" stroke="{stroke}" />'
        )
        parts.append(
            f'<text x="{item_x + 26}" y="{item_y - 2}" font-family="{FONT_FAMILY}" font-size="11" fill="{TEXT}">{escape(label)}</text>'
        )

    parts.append("</svg>")
    return "\n".join(parts)


def main() -> None:
    entities = load_model(MODEL_PATH)
    os.makedirs(os.path.dirname(OUTPUT_PATH), exist_ok=True)
    with open(OUTPUT_PATH, "w", encoding="utf-8", newline="\n") as handle:
        handle.write(render_svg(entities))
    print(f"Wrote {OUTPUT_PATH}")


if __name__ == "__main__":
    main()
