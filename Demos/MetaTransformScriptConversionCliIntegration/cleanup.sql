USE master;
IF DB_ID(N'MetaTransformScriptConversionDemo') IS NOT NULL
BEGIN
    ALTER DATABASE MetaTransformScriptConversionDemo SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MetaTransformScriptConversionDemo;
END;
IF DB_ID(N'MetaTransformScriptConversionPipeline') IS NOT NULL
BEGIN
    ALTER DATABASE MetaTransformScriptConversionPipeline SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE MetaTransformScriptConversionPipeline;
END;
