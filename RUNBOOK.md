# RUNBOOK

## Проверка состояния
docker compose ps

## Логи приложения
docker compose logs --tail 50 app

## Логи базы данных
docker compose logs --tail 30 mssql

## Проверка эндпоинтов
curl -k https://192.168.0.106/health
curl -k https://192.168.0.106/version
curl -k https://192.168.0.106/db/ping

## Обновление приложения
cd ~/deploy/is-stack
docker compose pull app
docker compose up -d app

## Резервное копирование базы данных
docker run -it --rm --network is-stack_is-net mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P 'z[jx.ctrcf1' -Q "BACKUP DATABASE IsLabDb TO DISK = N'/var/opt/mssql/backup/IsLabDb_full.bak' WITH INIT, COMPRESSION;"

## Восстановление базы данных
docker run -it --rm --network is-stack_is-net mcr.microsoft.com/mssql-tools /opt/mssql-tools/bin/sqlcmd -S mssql -U sa -P 'z[jx.ctrcf1' -Q "RESTORE DATABASE IsLabDb_RestoreTest FROM DISK = N'/var/opt/mssql/backup/IsLabDb_full.bak' WITH MOVE 'IsLabDb' TO '/var/opt/mssql/data/IsLabDb_RestoreTest.mdf', MOVE 'IsLabDb_log' TO '/var/opt/mssql/data/IsLabDb_RestoreTest_Log.ldf', REPLACE;"
