-- Deterministic schema script

CREATE TABLE [dbo].[BusinessBridge] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [AnchorHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridge] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessBridgeHub] (
    [Id] NVARCHAR(128) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [RoleName] NVARCHAR(MAX) NULL,
    [BusinessBridgeId] NVARCHAR(128) NOT NULL,
    [BusinessHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridgeHub] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessBridgeHubKeyPartProjection] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessBridgeId] NVARCHAR(128) NOT NULL,
    [BusinessHubKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridgeHubKeyPartProjection] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessBridgeHubSatelliteAttributeProjection] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessBridgeId] NVARCHAR(128) NOT NULL,
    [BusinessHubSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridgeHubSatelliteAttributeProjection] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessBridgeLink] (
    [Id] NVARCHAR(128) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [RoleName] NVARCHAR(MAX) NULL,
    [BusinessBridgeId] NVARCHAR(128) NOT NULL,
    [BusinessLinkId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridgeLink] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessBridgeLinkSatelliteAttributeProjection] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessBridgeId] NVARCHAR(128) NOT NULL,
    [BusinessLinkSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessBridgeLinkSatelliteAttributeProjection] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLink] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [ChildHubId] NVARCHAR(128) NOT NULL,
    [ParentHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLink] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLinkSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [SatelliteKind] NVARCHAR(MAX) NOT NULL,
    [BusinessHierarchicalLinkId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLinkSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLinkSatelliteAttribute] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHierarchicalLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLinkSatelliteAttribute] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessHierarchicalLinkSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLinkSatelliteKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHierarchicalLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLinkSatelliteKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessHierarchicalLinkSatelliteKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHub] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_BusinessHub] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessHubKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [SatelliteKind] NVARCHAR(MAX) NOT NULL,
    [BusinessHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubSatelliteAttribute] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHubSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubSatelliteAttribute] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubSatelliteAttributeDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessHubSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubSatelliteAttributeDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubSatelliteKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHubSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubSatelliteKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessHubSatelliteKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessHubSatelliteKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessHubSatelliteKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLink] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_BusinessLink] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkHub] (
    [Id] NVARCHAR(128) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [RoleName] NVARCHAR(MAX) NULL,
    [BusinessHubId] NVARCHAR(128) NOT NULL,
    [BusinessLinkId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkHub] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [SatelliteKind] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkSatelliteAttribute] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkSatelliteAttribute] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkSatelliteAttributeDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkSatelliteAttributeDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkSatelliteKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkSatelliteKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessLinkSatelliteKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkSatelliteKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessLinkSatelliteKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessPointInTime] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [BusinessHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessPointInTime] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessPointInTimeHubSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessHubSatelliteId] NVARCHAR(128) NOT NULL,
    [BusinessPointInTimeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessPointInTimeHubSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessPointInTimeLinkSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessLinkSatelliteId] NVARCHAR(128) NOT NULL,
    [BusinessPointInTimeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessPointInTimeLinkSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessPointInTimeStamp] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessPointInTimeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessPointInTimeStamp] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessPointInTimeStampDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessPointInTimeStampId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessPointInTimeStampDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReference] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    CONSTRAINT [PK_BusinessReference] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [SatelliteKind] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceSatelliteAttribute] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceSatelliteAttribute] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceSatelliteAttributeDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceSatelliteAttributeDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceSatelliteKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceSatelliteKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessReferenceSatelliteKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessReferenceSatelliteKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessReferenceSatelliteKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLink] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [EquivalentHubId] NVARCHAR(128) NOT NULL,
    [PrimaryHubId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLink] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLinkSatellite] (
    [Id] NVARCHAR(128) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [SatelliteKind] NVARCHAR(MAX) NOT NULL,
    [BusinessSameAsLinkId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLinkSatellite] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLinkSatelliteAttribute] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessSameAsLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLinkSatelliteAttribute] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLinkSatelliteAttributeDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessSameAsLinkSatelliteAttributeId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLinkSatelliteAttributeDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLinkSatelliteKeyPart] (
    [Id] NVARCHAR(128) NOT NULL,
    [DataTypeId] NVARCHAR(MAX) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Ordinal] NVARCHAR(MAX) NOT NULL,
    [BusinessSameAsLinkSatelliteId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLinkSatelliteKeyPart] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

CREATE TABLE [dbo].[BusinessSameAsLinkSatelliteKeyPartDataTypeDetail] (
    [Id] NVARCHAR(128) NOT NULL,
    [Name] NVARCHAR(MAX) NOT NULL,
    [Value] NVARCHAR(MAX) NOT NULL,
    [BusinessSameAsLinkSatelliteKeyPartId] NVARCHAR(128) NOT NULL,
    CONSTRAINT [PK_BusinessSameAsLinkSatelliteKeyPartDataTypeDetail] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

ALTER TABLE [dbo].[BusinessBridge] WITH CHECK ADD CONSTRAINT [FK_BusinessBridge_BusinessHub_AnchorHubId] FOREIGN KEY([AnchorHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHub] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHub_BusinessBridge_BusinessBridgeId] FOREIGN KEY([BusinessBridgeId]) REFERENCES [dbo].[BusinessBridge]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHub] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHub_BusinessHub_BusinessHubId] FOREIGN KEY([BusinessHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHubKeyPartProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHubKeyPartProjection_BusinessBridge_BusinessBridgeId] FOREIGN KEY([BusinessBridgeId]) REFERENCES [dbo].[BusinessBridge]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHubKeyPartProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHubKeyPartProjection_BusinessHubKeyPart_BusinessHubKeyPartId] FOREIGN KEY([BusinessHubKeyPartId]) REFERENCES [dbo].[BusinessHubKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHubSatelliteAttributeProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHubSatelliteAttributeProjection_BusinessBridge_BusinessBridgeId] FOREIGN KEY([BusinessBridgeId]) REFERENCES [dbo].[BusinessBridge]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeHubSatelliteAttributeProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeHubSatelliteAttributeProjection_BusinessHubSatelliteAttribute_BusinessHubSatelliteAttributeId] FOREIGN KEY([BusinessHubSatelliteAttributeId]) REFERENCES [dbo].[BusinessHubSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeLink] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeLink_BusinessBridge_BusinessBridgeId] FOREIGN KEY([BusinessBridgeId]) REFERENCES [dbo].[BusinessBridge]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeLink] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeLink_BusinessLink_BusinessLinkId] FOREIGN KEY([BusinessLinkId]) REFERENCES [dbo].[BusinessLink]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeLinkSatelliteAttributeProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeLinkSatelliteAttributeProjection_BusinessBridge_BusinessBridgeId] FOREIGN KEY([BusinessBridgeId]) REFERENCES [dbo].[BusinessBridge]([Id]);
GO

ALTER TABLE [dbo].[BusinessBridgeLinkSatelliteAttributeProjection] WITH CHECK ADD CONSTRAINT [FK_BusinessBridgeLinkSatelliteAttributeProjection_BusinessLinkSatelliteAttribute_BusinessLinkSatelliteAttributeId] FOREIGN KEY([BusinessLinkSatelliteAttributeId]) REFERENCES [dbo].[BusinessLinkSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLink] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLink_BusinessHub_ChildHubId] FOREIGN KEY([ChildHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLink] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLink_BusinessHub_ParentHubId] FOREIGN KEY([ParentHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLinkSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLinkSatellite_BusinessHierarchicalLink_BusinessHierarchicalLinkId] FOREIGN KEY([BusinessHierarchicalLinkId]) REFERENCES [dbo].[BusinessHierarchicalLink]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLinkSatelliteAttribute] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLinkSatelliteAttribute_BusinessHierarchicalLinkSatellite_BusinessHierarchicalLinkSatelliteId] FOREIGN KEY([BusinessHierarchicalLinkSatelliteId]) REFERENCES [dbo].[BusinessHierarchicalLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLinkSatelliteAttributeDataTypeDetail_BusinessHierarchicalLinkSatelliteAttribute_Business_002E2FFC9721A02D] FOREIGN KEY([BusinessHierarchicalLinkSatelliteAttributeId]) REFERENCES [dbo].[BusinessHierarchicalLinkSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLinkSatelliteKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLinkSatelliteKeyPart_BusinessHierarchicalLinkSatellite_BusinessHierarchicalLinkSatelliteId] FOREIGN KEY([BusinessHierarchicalLinkSatelliteId]) REFERENCES [dbo].[BusinessHierarchicalLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessHierarchicalLinkSatelliteKeyPartDataTypeDetail_BusinessHierarchicalLinkSatelliteKeyPart_BusinessHier_87D2853EAAF55BAC] FOREIGN KEY([BusinessHierarchicalLinkSatelliteKeyPartId]) REFERENCES [dbo].[BusinessHierarchicalLinkSatelliteKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessHubKeyPart_BusinessHub_BusinessHubId] FOREIGN KEY([BusinessHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessHubKeyPartDataTypeDetail_BusinessHubKeyPart_BusinessHubKeyPartId] FOREIGN KEY([BusinessHubKeyPartId]) REFERENCES [dbo].[BusinessHubKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessHubSatellite_BusinessHub_BusinessHubId] FOREIGN KEY([BusinessHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubSatelliteAttribute] WITH CHECK ADD CONSTRAINT [FK_BusinessHubSatelliteAttribute_BusinessHubSatellite_BusinessHubSatelliteId] FOREIGN KEY([BusinessHubSatelliteId]) REFERENCES [dbo].[BusinessHubSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubSatelliteAttributeDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessHubSatelliteAttributeDataTypeDetail_BusinessHubSatelliteAttribute_BusinessHubSatelliteAttributeId] FOREIGN KEY([BusinessHubSatelliteAttributeId]) REFERENCES [dbo].[BusinessHubSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubSatelliteKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessHubSatelliteKeyPart_BusinessHubSatellite_BusinessHubSatelliteId] FOREIGN KEY([BusinessHubSatelliteId]) REFERENCES [dbo].[BusinessHubSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessHubSatelliteKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessHubSatelliteKeyPartDataTypeDetail_BusinessHubSatelliteKeyPart_BusinessHubSatelliteKeyPartId] FOREIGN KEY([BusinessHubSatelliteKeyPartId]) REFERENCES [dbo].[BusinessHubSatelliteKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkHub] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkHub_BusinessHub_BusinessHubId] FOREIGN KEY([BusinessHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkHub] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkHub_BusinessLink_BusinessLinkId] FOREIGN KEY([BusinessLinkId]) REFERENCES [dbo].[BusinessLink]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkSatellite_BusinessLink_BusinessLinkId] FOREIGN KEY([BusinessLinkId]) REFERENCES [dbo].[BusinessLink]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkSatelliteAttribute] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkSatelliteAttribute_BusinessLinkSatellite_BusinessLinkSatelliteId] FOREIGN KEY([BusinessLinkSatelliteId]) REFERENCES [dbo].[BusinessLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkSatelliteAttributeDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkSatelliteAttributeDataTypeDetail_BusinessLinkSatelliteAttribute_BusinessLinkSatelliteAttributeId] FOREIGN KEY([BusinessLinkSatelliteAttributeId]) REFERENCES [dbo].[BusinessLinkSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkSatelliteKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkSatelliteKeyPart_BusinessLinkSatellite_BusinessLinkSatelliteId] FOREIGN KEY([BusinessLinkSatelliteId]) REFERENCES [dbo].[BusinessLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessLinkSatelliteKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessLinkSatelliteKeyPartDataTypeDetail_BusinessLinkSatelliteKeyPart_BusinessLinkSatelliteKeyPartId] FOREIGN KEY([BusinessLinkSatelliteKeyPartId]) REFERENCES [dbo].[BusinessLinkSatelliteKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTime] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTime_BusinessHub_BusinessHubId] FOREIGN KEY([BusinessHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeHubSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeHubSatellite_BusinessHubSatellite_BusinessHubSatelliteId] FOREIGN KEY([BusinessHubSatelliteId]) REFERENCES [dbo].[BusinessHubSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeHubSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeHubSatellite_BusinessPointInTime_BusinessPointInTimeId] FOREIGN KEY([BusinessPointInTimeId]) REFERENCES [dbo].[BusinessPointInTime]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeLinkSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeLinkSatellite_BusinessLinkSatellite_BusinessLinkSatelliteId] FOREIGN KEY([BusinessLinkSatelliteId]) REFERENCES [dbo].[BusinessLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeLinkSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeLinkSatellite_BusinessPointInTime_BusinessPointInTimeId] FOREIGN KEY([BusinessPointInTimeId]) REFERENCES [dbo].[BusinessPointInTime]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeStamp] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeStamp_BusinessPointInTime_BusinessPointInTimeId] FOREIGN KEY([BusinessPointInTimeId]) REFERENCES [dbo].[BusinessPointInTime]([Id]);
GO

ALTER TABLE [dbo].[BusinessPointInTimeStampDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessPointInTimeStampDataTypeDetail_BusinessPointInTimeStamp_BusinessPointInTimeStampId] FOREIGN KEY([BusinessPointInTimeStampId]) REFERENCES [dbo].[BusinessPointInTimeStamp]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceKeyPart_BusinessReference_BusinessReferenceId] FOREIGN KEY([BusinessReferenceId]) REFERENCES [dbo].[BusinessReference]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceKeyPartDataTypeDetail_BusinessReferenceKeyPart_BusinessReferenceKeyPartId] FOREIGN KEY([BusinessReferenceKeyPartId]) REFERENCES [dbo].[BusinessReferenceKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceSatellite_BusinessReference_BusinessReferenceId] FOREIGN KEY([BusinessReferenceId]) REFERENCES [dbo].[BusinessReference]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceSatelliteAttribute] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceSatelliteAttribute_BusinessReferenceSatellite_BusinessReferenceSatelliteId] FOREIGN KEY([BusinessReferenceSatelliteId]) REFERENCES [dbo].[BusinessReferenceSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceSatelliteAttributeDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceSatelliteAttributeDataTypeDetail_BusinessReferenceSatelliteAttribute_BusinessReferenceSatelliteAttributeId] FOREIGN KEY([BusinessReferenceSatelliteAttributeId]) REFERENCES [dbo].[BusinessReferenceSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceSatelliteKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceSatelliteKeyPart_BusinessReferenceSatellite_BusinessReferenceSatelliteId] FOREIGN KEY([BusinessReferenceSatelliteId]) REFERENCES [dbo].[BusinessReferenceSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessReferenceSatelliteKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessReferenceSatelliteKeyPartDataTypeDetail_BusinessReferenceSatelliteKeyPart_BusinessReferenceSatelliteKeyPartId] FOREIGN KEY([BusinessReferenceSatelliteKeyPartId]) REFERENCES [dbo].[BusinessReferenceSatelliteKeyPart]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLink] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLink_BusinessHub_EquivalentHubId] FOREIGN KEY([EquivalentHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLink] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLink_BusinessHub_PrimaryHubId] FOREIGN KEY([PrimaryHubId]) REFERENCES [dbo].[BusinessHub]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLinkSatellite] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLinkSatellite_BusinessSameAsLink_BusinessSameAsLinkId] FOREIGN KEY([BusinessSameAsLinkId]) REFERENCES [dbo].[BusinessSameAsLink]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLinkSatelliteAttribute] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLinkSatelliteAttribute_BusinessSameAsLinkSatellite_BusinessSameAsLinkSatelliteId] FOREIGN KEY([BusinessSameAsLinkSatelliteId]) REFERENCES [dbo].[BusinessSameAsLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLinkSatelliteAttributeDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLinkSatelliteAttributeDataTypeDetail_BusinessSameAsLinkSatelliteAttribute_BusinessSameAsLinkSa_8C8BA056D253570A] FOREIGN KEY([BusinessSameAsLinkSatelliteAttributeId]) REFERENCES [dbo].[BusinessSameAsLinkSatelliteAttribute]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLinkSatelliteKeyPart] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLinkSatelliteKeyPart_BusinessSameAsLinkSatellite_BusinessSameAsLinkSatelliteId] FOREIGN KEY([BusinessSameAsLinkSatelliteId]) REFERENCES [dbo].[BusinessSameAsLinkSatellite]([Id]);
GO

ALTER TABLE [dbo].[BusinessSameAsLinkSatelliteKeyPartDataTypeDetail] WITH CHECK ADD CONSTRAINT [FK_BusinessSameAsLinkSatelliteKeyPartDataTypeDetail_BusinessSameAsLinkSatelliteKeyPart_BusinessSameAsLinkSatelliteKeyPartId] FOREIGN KEY([BusinessSameAsLinkSatelliteKeyPartId]) REFERENCES [dbo].[BusinessSameAsLinkSatelliteKeyPart]([Id]);
GO

