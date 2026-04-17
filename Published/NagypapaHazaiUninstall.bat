@echo off
setlocal EnableDelayedExpansion
title Projekt NagypapaHazai - Rendszer Tisztito / Eltavolito

:: ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
::   NagypapaHazai Kornyezet Felszamolo Modul
:: ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

echo.
echo ************************************************************
echo * N A G Y P A P A H A Z A I - U N I N S T A L L            *
echo ************************************************************
echo.

:: --- [1] Biztonsagi ellenorzes: Jogosultsagok ---
fsutil dirty query %systemdrive% >nul 2>&1
if !errorlevel! neq 0 (
    echo [ ! ] Kritikus hiba: Ezt az eszkozt Rendszergazdakent kell futtatni!
    echo Zarja be az ablakot, es nyissa meg ujra Administrator modban.
    echo.
    pause
    exit /b
)

:: --- [2] Valtozok inicializalasa (az elozo scriptek alapjan) ---
set "IIS_VEZERLO=%WinDir%\System32\inetsrv\appcmd.exe"
set "WEB_NEV=NagyPapa_Kliens_Site"
set "API_NEV=NagyPapa_Szolgaltatas_Site"
set "WEB_POOL=NagyPapa_Kliens_AppPool"
set "API_POOL=NagyPapa_Szolgaltatas_AppPool"
:: A GitHub repo appsettings.json szerinti adatbazis nev
set "ADATBAZIS_NEV=NagypapaHazai"

set "BASED=%~dp0"
set "BASED=%BASED:~0,-1%"
set "CEL_MAPPA=%BASED%\Forditott"

:: --- [3] Felhasznaloi megerositest ---
echo Figyelem! Ez a muvelet veglegesen leallitja es torli a rendszerbol:
echo  - A "%WEB_NEV%" es "%API_NEV%" webszerver beallitasokat.
echo  - Az ezekhez tartozo IIS memoria medenceket.
echo  - Az SQL szerverbol a kiosztott hozzaferesi jogokat.
echo  - A letrehozott biztonsagi tuzfal szabalyokat.
echo.
choice /C IN /N /M "Biztosan folytatni kivanja a torlest? [I=Igen / N=Nem]: "
if !errorlevel! equ 2 (
    echo A felhasznalo megszakitotta a folyamatot. Kilepes semmilyen valtoztatas nelkul...
    pause
    exit /b
)

echo.
:: --- [4] Weblapok megszuntetese ---
echo ---^> Fazis 1/4: Webes vegpontok leallitasa es kitorlese...
if exist "%IIS_VEZERLO%" (
    "%IIS_VEZERLO%" stop site "%API_NEV%" >nul 2>&1
    "%IIS_VEZERLO%" stop site "%WEB_NEV%" >nul 2>&1
    "%IIS_VEZERLO%" delete site "%API_NEV%" >nul 2>&1
    "%IIS_VEZERLO%" delete site "%WEB_NEV%" >nul 2>&1
) else (
    echo [ ! ] appcmd.exe nem talalhato, IIS torles atugorva.
)

:: --- [5] Application Poolok torlese ---
echo ---^> Fazis 2/4: Memoria medencek (App Pools) megszuntetese...
if exist "%IIS_VEZERLO%" (
    "%IIS_VEZERLO%" stop apppool "%API_POOL%" >nul 2>&1
    "%IIS_VEZERLO%" stop apppool "%WEB_POOL%" >nul 2>&1
    "%IIS_VEZERLO%" delete apppool "%API_POOL%" >nul 2>&1
    "%IIS_VEZERLO%" delete apppool "%WEB_POOL%" >nul 2>&1
)

:: --- [6] SQL hozzaferesek visszavonasa ---
echo ---^> Fazis 3/4: Adatbazis jogosultsagok visszavonasa...
where sqlcmd >nul 2>&1
if !errorlevel! equ 0 (
    :: Felhasznalok torlese az adatbazisbol (csak ha letezik az adatbazis)
    sqlcmd -S .\SQLEXPRESS -E -Q "IF DB_ID('%ADATBAZIS_NEV%') IS NOT NULL BEGIN USE [%ADATBAZIS_NEV%]; IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name='IIS APPPOOL\%WEB_POOL%') DROP USER [IIS APPPOOL\%WEB_POOL%]; IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name='IIS APPPOOL\%API_POOL%') DROP USER [IIS APPPOOL\%API_POOL%]; END" >nul 2>&1
    :: Loginok torlese a szerverrol
    sqlcmd -S .\SQLEXPRESS -E -Q "IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name='IIS APPPOOL\%WEB_POOL%') DROP LOGIN [IIS APPPOOL\%WEB_POOL%]; IF EXISTS (SELECT 1 FROM sys.server_principals WHERE name='IIS APPPOOL\%API_POOL%') DROP LOGIN [IIS APPPOOL\%API_POOL%];" >nul 2>&1
    echo SQL jogosultsagok torolve.
) else (
    echo [ ! ] SQLCMD nem talalhato, az adatbazis felhasznalok torleset manualisan kell elvegezni.
)

:: --- [7] Tuzfal takaritas ---
echo ---^> Fazis 4/4: Biztonsagi tuzfal szabalyok radirozasa...
netsh advfirewall firewall delete rule name="Nagypapa_Kliens_Szabaly" >nul 2>&1
netsh advfirewall firewall delete rule name="Nagypapa_Szerver_Szabaly" >nul 2>&1

:: --- [8] Zaro uzenet ---
echo.
echo ************************************************************
echo * T I S Z T I T A S   B E F E J E Z O D O T T              *
echo ************************************************************
echo.
echo Megjegyzes: A leforditott fizikai fajlok a "%CEL_MAPPA%" mappaban
echo megmaradtak a merevlemezen. Azok torleset Onnek kell elvegeznie,
echo amennyiben mar nincs rajuk szuksege!
echo.
pause
endlocal
