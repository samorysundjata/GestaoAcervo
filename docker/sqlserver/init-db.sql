-- =============================================================================
-- Script de inicialização do banco de dados AcervoDB
-- Executado automaticamente pelo entrypoint do container SQL Server
-- =============================================================================

-- Aguarda o SQL Server inicializar completamente antes de criar o banco
-- (o wait é gerenciado pelo entrypoint.sh)

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AcervoDB')
BEGIN
    CREATE DATABASE AcervoDB;
    PRINT 'Banco de dados AcervoDB criado com sucesso.';
END
ELSE
BEGIN
    PRINT 'Banco de dados AcervoDB já existe.';
END
GO

-- As tabelas e schema serão criados pelas EF Core Migrations
-- ao iniciar a Acervo.API (configuração: migrate on startup)
