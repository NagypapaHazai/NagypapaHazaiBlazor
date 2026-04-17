/* =========================================================================
   NagypapaHázai - Adatbazis Jogosultsag Konfigurator
   =========================================================================
   Parancssoros inditas (ha kezzel futtatod):
   sqlcmd -S .\SQLEXPRESS -E -i cucusql.sql
========================================================================= */

SET NOCOUNT ON;

PRINT '---[ 0. Cel adatbazis ellenorzese ]---';

IF DB_ID(N'NagypapaHazai') IS NULL
BEGIN
    PRINT '!!! FIGYELEM: A NagypapaHazai adatbazis meg nem letezik a szerveren.';
    PRINT '!!! Importalja eloszor a bacpac fajlt (bacpac\NagypapaHazai.bacpac),';
    PRINT '!!! majd futtassa ujra ezt a scriptet.';
    RETURN;
END
GO

PRINT '---[ 1. Rendszerszintu bejelentkezesek (Login) ellenorzese ]---';

-- Kliens (Web) alkalmazaskeszlet beallitasa
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\NagyPapa_Kliens_AppPool')
BEGIN
    CREATE LOGIN [IIS APPPOOL\NagyPapa_Kliens_AppPool] FROM WINDOWS;
    PRINT '>>> Uj login bejegyezve: Kliens (NagyPapa_Kliens_AppPool)';
END
ELSE
BEGIN
    PRINT '... A Kliens app pool login mar rendelkezesre all.';
END
GO

-- Szolgaltatas (API) alkalmazaskeszlet beallitasa
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool')
BEGIN
    CREATE LOGIN [IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool] FROM WINDOWS;
    PRINT '>>> Uj login bejegyezve: Szolgaltatas (NagyPapa_Szolgaltatas_AppPool)';
END
ELSE
BEGIN
    PRINT '... A Szolgaltatas app pool login mar rendelkezesre all.';
END
GO

PRINT '';
PRINT '---[ 2. Adatbazis felhasznalok es szerepkorok kiosztasa ]---';

-- Ugras a cel adatbazisra (a GitHub repo appsettings.json alapjan: NagypapaHazai)
USE [NagypapaHazai];
GO

-- Kliens fiok lekepezese az adatbazisra
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\NagyPapa_Kliens_AppPool')
BEGIN
    CREATE USER [IIS APPPOOL\NagyPapa_Kliens_AppPool] FOR LOGIN [IIS APPPOOL\NagyPapa_Kliens_AppPool];
    PRINT '>>> Felhasznalo mappelese megtortent a Kliens fiokhoz.';
END
GO

-- Szerver fiok lekepezese az adatbazisra
IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool')
BEGIN
    CREATE USER [IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool] FOR LOGIN [IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool];
    PRINT '>>> Felhasznalo mappelese megtortent az API fiokhoz.';
END
GO

-- Maximalis adminisztratori (db_owner) jogok aktivalasa mindket pool szamara
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\NagyPapa_Kliens_AppPool];
ALTER ROLE db_owner ADD MEMBER [IIS APPPOOL\NagyPapa_Szolgaltatas_AppPool];
PRINT '>>> DB_OWNER (Tulajdonos) jogosultsagok hozzarendelve mindket modulhoz.';
GO

PRINT '';
PRINT '*************************************************************';
PRINT '* SQL SZERVER HOZZAFERESEK SIKERESEN BEALLITVA!             *';
PRINT '*************************************************************';
GO
