using MetaSql.Core;

public sealed class DesiredSqlLoaderTests
{
    [Fact]
    public void LoadsCreateTableAndAlterConstraintBatches()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, "H_Customer.sql"),
                """
                -- Deterministic schema script

                CREATE TABLE [dbo].[H_Customer] (
                    [HashKey] binary(16) NOT NULL,
                    [CustomerId] nvarchar(50) NOT NULL,
                    CONSTRAINT [PK_H_Customer] PRIMARY KEY CLUSTERED ([HashKey] ASC)
                );
                GO

                ALTER TABLE [dbo].[H_Customer] WITH CHECK ADD CONSTRAINT [UQ_H_Customer] UNIQUE ([CustomerId]);
                GO
                """);

            var model = new DesiredSqlLoader().LoadFromDirectory(root);

            var table = Assert.Single(model.Tables);
            Assert.Equal("dbo", table.SchemaName);
            Assert.Equal("H_Customer", table.TableName);
            Assert.Equal(2, table.Columns.Count);
            Assert.Single(table.InlineConstraints);
            Assert.Single(table.AlterConstraints);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void LoadsCreateIndexFromSeparateFile()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, "H_Customer.sql"), "CREATE TABLE [dbo].[H_Customer] ([HashKey] binary(16) NOT NULL);");
            File.WriteAllText(Path.Combine(root, "IX_H_Customer.sql"), "CREATE INDEX [IX_H_Customer_Test] ON [dbo].[H_Customer] ([HashKey]);");

            var model = new DesiredSqlLoader().LoadFromDirectory(root);

            Assert.Equal("IX_H_Customer_Test", Assert.Single(Assert.Single(model.Tables).Indexes).Name);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void LoadsMultipleValidFiles()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, "A.sql"), "CREATE TABLE [dbo].[A] ([Id] int NOT NULL);");
            File.WriteAllText(Path.Combine(root, "B.sql"), "CREATE TABLE [dbo].[B] ([Id] int NOT NULL);");

            var model = new DesiredSqlLoader().LoadFromDirectory(root);

            Assert.Equal(2, model.Tables.Count);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void DuplicateObjectDefinitions_LastOneWins()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, "A.sql"), "CREATE TABLE [dbo].[A] ([Id] int NOT NULL);");
            File.WriteAllText(Path.Combine(root, "B.sql"), "CREATE TABLE [dbo].[A] ([Id] bigint NOT NULL);");

            var model = new DesiredSqlLoader().LoadFromDirectory(root);

            Assert.Equal("bigint", Assert.Single(Assert.Single(model.Tables).Columns).TypeSql);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void UnsupportedStatementFormProducesNoParsedTables()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, "Drop.sql"), "DROP TABLE [dbo].[A];");

            var model = new DesiredSqlLoader().LoadFromDirectory(root);

            Assert.Empty(model.Tables);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void MissingDesiredSqlDirectoryFails()
    {
        var path = Path.Combine(TestSupport.CreateTempDirectory(), "missing");

        var exception = Assert.Throws<InvalidOperationException>(() => new DesiredSqlLoader().LoadFromDirectory(path));

        Assert.Contains("does not exist", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void EmptyDesiredSqlDirectoryFails()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            var exception = Assert.Throws<InvalidOperationException>(() => new DesiredSqlLoader().LoadFromDirectory(root));
            Assert.Contains("does not contain any .sql files", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void BadConstraintBeforeCreateTableFailsPlainly()
    {
        var root = TestSupport.CreateTempDirectory();
        try
        {
            File.WriteAllText(Path.Combine(root, "Bad.sql"), "ALTER TABLE [dbo].[H_Customer] ADD CONSTRAINT [UQ_H_Customer] UNIQUE ([CustomerId]);");

            var exception = Assert.Throws<InvalidOperationException>(() => new DesiredSqlLoader().LoadFromDirectory(root));

            Assert.Contains("before its CREATE TABLE was seen", exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
