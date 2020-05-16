# OpenttdDiscord Bot Database

All MySQL scripts required in process of creating database are placed inside SQL folder.
Please execute them in ascending order.

Do not execute scripts from `TestSQL` folder inside `SQL` folder. This folder is used for integration tests.

If you wish to run integration tests running on dockerized database you need to build docker image first by running either `build.sh` or `build.ps1` from `SQL` folder.