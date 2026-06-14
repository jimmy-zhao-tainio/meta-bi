@echo off
if not defined AW_SQL_SERVER set "AW_SQL_SERVER=localhost"
if not defined AW_SOURCE_DATABASE set "AW_SOURCE_DATABASE=AdventureWorks2022"
if not defined AW_TARGET_DATABASE set "AW_TARGET_DATABASE=AdventureWorksMetaDemo"
if not defined AW_TABULAR_SERVER set "AW_TABULAR_SERVER=.\TABULAR"
if not defined AW_TABULAR_DATABASE set "AW_TABULAR_DATABASE=AdventureWorksMetaDemoTabular"
if not defined AW_RUN_ROOT set "AW_RUN_ROOT=Runs"
if not defined AW_SOURCE_SQL set "AW_SOURCE_SQL=Server=%AW_SQL_SERVER%;Database=%AW_SOURCE_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"
if not defined AW_TARGET_SQL set "AW_TARGET_SQL=Server=%AW_SQL_SERVER%;Database=%AW_TARGET_DATABASE%;Trusted_Connection=True;TrustServerCertificate=True;"

echo AW_SQL_SERVER=%AW_SQL_SERVER%
echo AW_SOURCE_DATABASE=%AW_SOURCE_DATABASE%
echo AW_TARGET_DATABASE=%AW_TARGET_DATABASE%
echo AW_TABULAR_SERVER=%AW_TABULAR_SERVER%
echo AW_TABULAR_DATABASE=%AW_TABULAR_DATABASE%
echo AW_RUN_ROOT=%AW_RUN_ROOT%
