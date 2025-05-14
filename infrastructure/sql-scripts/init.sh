#!/bin/bash

echo "Waiting for SQL Server to be available..."
for i in {1..30}; do
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Pass123 -Q "SELECT 1" > /dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "SQL Server is ready!"
        break
    fi
    sleep 2
done

echo "Running init script..."
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Pass123 -i /init/todo-db.sql
