@echo Preparing shared MetaOrchestration demo workspace...
@call prepare.cmd
@if errorlevel 1 exit /b %errorlevel%

@echo Running complete DAG execution scenario...
@call run-complete.cmd
@if errorlevel 1 exit /b %errorlevel%

@echo Running explicit policy run-plan scenario...
@call run-policy.cmd
@if errorlevel 1 exit /b %errorlevel%

@echo Running invalid DAG evidence scenario...
@call run-invalid.cmd
@if errorlevel 1 exit /b %errorlevel%

@echo Running failure dependency execution scenario...
@call run-failure.cmd
@if errorlevel 1 exit /b %errorlevel%
