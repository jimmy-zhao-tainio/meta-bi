@echo Preparing shared MetaOrchestration demo workspace...
@call prepare.cmd
@if errorlevel 1 exit /b %errorlevel%

@echo Running complete DAG execution scenario...
@call run-complete.cmd
@if errorlevel 1 exit /b %errorlevel%
