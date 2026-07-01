@echo off
if not defined AW_SQL_SERVER set "AW_SQL_SERVER=localhost"
if not defined AW_SOURCE_DATABASE set "AW_SOURCE_DATABASE=AdventureWorks2022"
if not defined AW_RDV_DATABASE set "AW_RDV_DATABASE=AdventureWorksRawVault"
if not defined AW_BDV_DATABASE set "AW_BDV_DATABASE=AdventureWorksBusinessVault"
if not defined AW_DW_DATABASE set "AW_DW_DATABASE=AdventureWorksMetaDemo"
if not defined AW_TARGET_DATABASE set "AW_TARGET_DATABASE=%AW_DW_DATABASE%"
if not defined AW_TABULAR_SERVER set "AW_TABULAR_SERVER=.\TABULAR"
if not defined AW_TABULAR_DATABASE set "AW_TABULAR_DATABASE=AdventureWorksMetaDemoTabular"
if not defined AW_RUN_ROOT set "AW_RUN_ROOT=Runs"
if not defined AW_SOURCE_SQL set "AW_SOURCE_SQL=Server=%AW_SQL_SERVER%;Database=%AW_SOURCE_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined AW_RDV_SQL set "AW_RDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_RDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined AW_BDV_SQL set "AW_BDV_SQL=Server=%AW_SQL_SERVER%;Database=%AW_BDV_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined AW_DW_SQL set "AW_DW_SQL=Server=%AW_SQL_SERVER%;Database=%AW_DW_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined AW_TARGET_SQL set "AW_TARGET_SQL=%AW_DW_SQL%"

echo AW_SQL_SERVER=%AW_SQL_SERVER%
echo AW_SOURCE_DATABASE=%AW_SOURCE_DATABASE%
echo AW_RDV_DATABASE=%AW_RDV_DATABASE%
echo AW_BDV_DATABASE=%AW_BDV_DATABASE%
echo AW_DW_DATABASE=%AW_DW_DATABASE%
echo AW_TARGET_DATABASE=%AW_TARGET_DATABASE%
echo AW_TABULAR_SERVER=%AW_TABULAR_SERVER%
echo AW_TABULAR_DATABASE=%AW_TABULAR_DATABASE%
echo AW_RUN_ROOT=%AW_RUN_ROOT%
