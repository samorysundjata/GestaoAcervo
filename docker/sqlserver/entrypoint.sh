#!/bin/bash
# =============================================================================
# entrypoint.sh — Inicializa o SQL Server e executa o script de criação do banco
# =============================================================================

echo ">>> Iniciando SQL Server..."
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo ">>> Aguardando SQL Server ficar pronto..."
for i in {1..30}; do
    /opt/mssql-tools18/bin/sqlcmd \
        -S localhost \
        -U sa \
        -P "$SA_PASSWORD" \
        -Q "SELECT 1" \
        -No \
        > /dev/null 2>&1

    if [ $? -eq 0 ]; then
        echo ">>> SQL Server pronto! Executando script de inicialização..."
        /opt/mssql-tools18/bin/sqlcmd \
            -S localhost \
            -U sa \
            -P "$SA_PASSWORD" \
            -i /docker-entrypoint-initdb.d/init-db.sql \
            -No
        echo ">>> Script executado com sucesso."
        break
    fi

    echo "    Tentativa $i/30 — aguardando 2s..."
    sleep 2
done

# Mantém o processo do SQL Server em foreground
wait $SQL_PID
