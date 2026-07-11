# AdventureWorks video snag log

| Id | Phase | Symptom | Owner | Status | Resolution |
| --- | --- | --- | --- | --- | --- |
| AWV-0001 | Video scaffold | Need a recording harness that separates source setup, business requirements, agent tasking, generated run output, and product snags. | Demo harness | Fixed | Added this scaffold under `Demos/AdventureWorksBiStackVideo`. |
| AWV-0002 | Source setup | AdventureWorks backup download/restore is machine-specific. | Demo harness | Fixed | Added `prepare-adventureworks-db.ps1` to download the official `AdventureWorks2022.bak`, restore the local SQL database, and verify source data through the mesh before recording. |
