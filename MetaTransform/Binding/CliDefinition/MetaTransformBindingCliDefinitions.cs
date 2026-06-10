using Meta.Core.Presentation.Cli;

namespace MetaTransform.Binding.CliDefinition;

public static class MetaTransformBindingCliDefinitions
{
    public const string AppName = "meta-transform-binding";

    public static CliAppDefinition CreateAppDefinition() =>
        new(
            AppName,
            new[]
            {
                "meta-transform-binding <command> [options]"
            },
            CreateCommandDefinitions(),
            Next: "meta-transform-binding bind --help");

    public static IReadOnlyList<CliCommandDefinition> CreateCommandDefinitions() =>
        new[]
        {
            CreateHelpCommandDefinition(),
            CreateBindCommandDefinition(),
        };

    public static CliCommandDefinition CreateHelpCommandDefinition() =>
        new(
            "help",
            "Show this help.",
            new[] { "meta-transform-binding help" });

    public static CliCommandDefinition CreateBindCommandDefinition() =>
        new(
            "bind",
            "Bind all transform scripts and validate against source/target schema contracts into a new workspace.",
            new[]
            {
                "meta-transform-binding bind --transform-workspace <path> --source-schema <path> [--source-schema <path> ...] --target-schema <path> --execute-system <name> --new-workspace <path> [--execute-system-default-schema-name <schema>] [--ignore-target-columns <col[,col...]>] [--ignore-target-columns-if-present <col[,col...]>] [--data-type-conversion-workspace <path>] [--allow-partial] [--partial-report <path>]"
            },
            new[]
            {
                new CliOptionDefinition("--transform-workspace <path>", "Required. MetaTransformScript workspace to bind."),
                new CliOptionDefinition("--source-schema <path>", "Required. Repeatable source MetaSchema workspace."),
                new CliOptionDefinition("--target-schema <path>", "Required. Target MetaSchema workspace."),
                new CliOptionDefinition("--execute-system <name>", "Required. Execution context for one/two-part source identifiers."),
                new CliOptionDefinition("--new-workspace <path>", "Required. Directory where the binding workspace will be created."),
                new CliOptionDefinition("--execute-system-default-schema-name <schema>", "Required when any one-part source identifier exists."),
                new CliOptionDefinition("--ignore-target-columns <col[,col...]>", "Optional comma-separated target columns to exclude from target conformance checks."),
                new CliOptionDefinition("--ignore-target-columns-if-present <col[,col...]>", "Optional comma-separated target columns to exclude only on target tables where they exist."),
                new CliOptionDefinition("--data-type-conversion-workspace <path>", "Optional sanctioned conversion policy workspace. Omitted uses built-in defaults."),
                new CliOptionDefinition("--allow-partial", "Optional. Save only objects that bind and validate successfully; skipped objects are failures."),
                new CliOptionDefinition("--partial-report <path>", "Optional TSV report for objects skipped due to binding or validation failure. Requires --allow-partial.")
            },
            new[]
            {
                "bind is atomic: it binds and validates in one run.",
                "If binding or validation fails, no binding workspace is created.",
                "--allow-partial is an explicit corpus/discovery mode: objects with binding or validation failures are skipped and successful bindings are saved.",
                "bind processes all transform scripts in the transform workspace.",
                "Target SQL identifier is read from ScriptObjectView.TargetSqlIdentifier.",
                "Source schema workspaces are repeatable; target schema workspace is single.",
                "Every schema workspace must contain exactly one system.",
                "--execute-system-default-schema-name is required when any one-part source identifier exists.",
                "--ignore-target-columns excludes named non-identity target columns from target conformance checks.",
                "Ignored names must exist on each target table or bind fails explicitly.",
                "--ignore-target-columns-if-present excludes named non-identity target columns only on target tables where they exist."
            },
            new[]
            {
                "meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SourceSchemaWS --target-schema .\\TargetSchemaWS --execute-system SalesDb --new-workspace .\\BindingWS",
                "meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SalesSchemaWS --source-schema .\\ReferenceSchemaWS --target-schema .\\WarehouseSchemaWS --execute-system WarehouseDb --execute-system-default-schema-name dbo --new-workspace .\\BindingWS --ignore-target-columns LoadUtc,RunId --ignore-target-columns-if-present UpdateAudit_ID",
                "meta-transform-binding bind --transform-workspace .\\TransformWS --source-schema .\\SourceSchemaWS --target-schema .\\TargetSchemaWS --execute-system SalesDb --execute-system-default-schema-name dbo --new-workspace .\\BindingWS --allow-partial --partial-report .\\binding-partial.tsv"
            });
}
