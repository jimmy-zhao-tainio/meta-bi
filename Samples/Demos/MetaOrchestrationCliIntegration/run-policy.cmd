@call env.cmd

meta-pipeline --new-workspace PolicyPipelineWS
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace PolicyPipelineWS --name UpdateSharedLanding
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace PolicyPipelineWS --pipeline UpdateSharedLanding --step-name update-shared-landing --script update_shared_landing --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-pipeline add-pipeline --workspace PolicyPipelineWS --name MergeSharedLanding
@if errorlevel 1 exit /b %errorlevel%
meta-pipeline add-step --workspace PolicyPipelineWS --pipeline MergeSharedLanding --step-name merge-shared-landing --script merge_shared_landing --transform-workspace TransformWS --binding-workspace BindingWS --execution-connection-env META_ORCHESTRATION_DEMO_SQL
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration --pipeline-workspace PolicyPipelineWS --transform-workspace TransformWS --binding-workspace BindingWS --new-workspace PolicyOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration inspect --workspace PolicyOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration list-issues --workspace PolicyOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration add-order --workspace PolicyOrchestrationWS --from-task UpdateSharedLanding.update-shared-landing --to-task MergeSharedLanding.merge-shared-landing --object dbo.SharedLanding --reason "Update shared landing before merge."
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration set-lock-policy --workspace PolicyOrchestrationWS --object dbo.SharedLanding --left-effect Mutation --right-effect Mutation --behavior serialize --reason "Shared landing mutations should not overlap."
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration refresh-run-plan --workspace PolicyOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%

meta-orchestration inspect-run-plan --workspace PolicyOrchestrationWS
@if errorlevel 1 exit /b %errorlevel%
