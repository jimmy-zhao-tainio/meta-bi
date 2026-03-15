CREATE TABLE [dbo].[S_CustomerProfile] (
    [CustomerId] int NOT NULL,
    [CustomerName] nvarchar(200) NULL,
    CONSTRAINT [PK_S_CustomerProfile] PRIMARY KEY ([CustomerId])
);
GO
CREATE INDEX [IX_S_CustomerProfile_CustomerName]
    ON [dbo].[S_CustomerProfile] ([CustomerName]);
