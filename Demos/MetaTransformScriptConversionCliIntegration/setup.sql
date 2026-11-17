IF DB_ID(N'MetaTransformScriptConversionDemo') IS NULL
    CREATE DATABASE MetaTransformScriptConversionDemo;
GO
USE MetaTransformScriptConversionDemo;
GO
CREATE TABLE dbo.SourceRows
(
    SequenceNo int NOT NULL PRIMARY KEY,
    NumericValue int NULL,
    TextValue nvarchar(50) NULL
);
CREATE TABLE dbo.TargetRows
(
    SequenceNo bigint NOT NULL PRIMARY KEY,
    NumericValue bigint NULL,
    TextValue nvarchar(50) NULL
);
INSERT dbo.SourceRows (SequenceNo, NumericValue, TextValue)
VALUES (1,17,N'ordinary text'), (2,-42,N'Åsa äter räksmörgås.'), (3,NULL,NULL), (4,0,N'');
