#!/bin/bash
# Wait until SQL Server is ready
echo "Waiting for SQL Server to be available..."
sleep 15

# Run your init script using sqlcmd
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong!Pass123 -i /init/todo-db.sql
