@echo off
setlocal EnableDelayedExpansion
title Projekt NagypapaHazai - Szolgaltatas Indito

REM ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
REM   NagypapaHazai WebKiszolgalo Inditomotor
REM ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

echo.
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo +   N A G Y P A P A H A Z A I - G Y O R S I N D I T O        +
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo.

REM --- [A] Emelt szintu jogok vizsgalata ---
fsutil dirty query %systemdrive% >nul 2>&1
if !errorlevel! neq 0 (
    echo ^>^>^> JOGOSULTSAGI PROBLEMA: A programot Rendszergazdakent kell futtatni!
    echo Kerem, zarja be, majd jobb egergombbal valassza a megfelelo inditasi funkciot.
    echo.
    pause
    exit /b 1
)

set "IIS_CMD_TOOL=%WinDir%\System32\inetsrv\appcmd.exe"
set "KLIENS_OLDAL=NagyPapa_Kliens_Site"
set "SZERVER_OLDAL=NagyPapa_Szolgaltatas_Site"
set "KLIENS_MEDENCE=NagyPapa_Kliens_AppPool"
set "SZERVER_MEDENCE=NagyPapa_Szolgaltatas_AppPool"
REM Portok a launchSettings.json alapjan
set "KLIENS_KAPU=5011"
set "SZERVER_KAPU=5002"

REM --- [B] Alapveto Windows Web Szolgaltatasok vizsgalata ---
echo ---^> Lepes 1: Hangeromu es hatterfolyamatok tesztelese...
sc query W3SVC | find /I "RUNNING" >nul
if !errorlevel! neq 0 (
    echo A W3SVC (IIS) jelenleg all. Bekapcsolas folyamatban...
    net start W3SVC >nul 2>&1
    if !errorlevel! neq 0 (
        echo ^>^>^> VEGZETES HIBA: Az IIS motor inditasa elbukott. Telepitve lett a rendszer korabban?
        pause
        exit /b 1
    )
    echo Az IIS kiszolgalo felebresztve.
) else (
    echo Az IIS kiszolgalo mar aktiv allapotban van.
)

sc query WAS | find /I "RUNNING" >nul
if !errorlevel! neq 0 (
    net start WAS >nul 2>&1
)
echo.

if not exist "%IIS_CMD_TOOL%" (
    echo ^>^>^> VEGZETES HIBA: Hianyzik az appcmd.exe. A konfigurator script meg nem futott le!
    pause
    exit /b 1
)

REM --- [C] Meglevo konfiguraciok hitelesitese ---
"%IIS_CMD_TOOL%" list site "%KLIENS_OLDAL%" | find /I "%KLIENS_OLDAL%" >nul
if !errorlevel! neq 0 (
    echo ^>^>^> VEGZETES HIBA: Nincs telepitve a webes felulet ^(%KLIENS_OLDAL%^). Eloszor futtassa a NagypapaHazai.bat telepitot!
    pause
    exit /b 1
)

"%IIS_CMD_TOOL%" list site "%SZERVER_OLDAL%" | find /I "%SZERVER_OLDAL%" >nul
if !errorlevel! neq 0 (
    echo ^>^>^> VEGZETES HIBA: Nincs telepitve az API ^(%SZERVER_OLDAL%^). Eloszor futtassa a NagypapaHazai.bat telepitot!
    pause
    exit /b 1
)

REM --- [D] Memoria medencek es Weboldalak aktivalasa ---
echo ---^> Lepes 2: Memoria medencek es vegpontok betoltese...
"%IIS_CMD_TOOL%" start apppool "%SZERVER_MEDENCE%" >nul 2>&1
"%IIS_CMD_TOOL%" start apppool "%KLIENS_MEDENCE%" >nul 2>&1
"%IIS_CMD_TOOL%" start site "%SZERVER_OLDAL%" >nul 2>&1
"%IIS_CMD_TOOL%" start site "%KLIENS_OLDAL%" >nul 2>&1
echo Minden komponens sikeresen elinditva.
echo.

echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo +   A RENDSZER ONLINE ES HASZNALATRA KESZ!                   +
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo.
echo   Felhasznaloi felulet:     http://localhost:%KLIENS_KAPU%
echo   Adatkapcsolati vegpont:   http://localhost:%SZERVER_KAPU%
echo.

REM --- [E] Automatikus bongeszo betoltes ---
choice /C IN /N /M "Kivanja azonnal megnyitni a weblapot a default bongeszoben? [I=Igen, N=Nem]: "
if !errorlevel! equ 1 start "" "http://localhost:%KLIENS_KAPU%"

echo.
pause
endlocal
