@echo off
setlocal EnableDelayedExpansion
title Projekt NagypapaHazai - Szerver Konfigurator

REM ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
REM   NagypapaHazai Automata Rendszerepito Modul
REM ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

echo.
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo +   N A G Y P A P A H A Z A I   S T A R T E R                +
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo.

REM --- [A] Privilegiumok vizsgalata ---
fsutil dirty query %systemdrive% >nul 2>&1
if %errorlevel% neq 0 (
    echo FIGYELMEZTETES: Hianyozo privilegiumok!
    echo Kerem, inditsa el ujra a parancssort emelt szintu ^(Administrator^) modban.
    echo.
    pause
    exit /b
)
echo Jogosultsagok ellenorzese: Rendszergazda mod aktiv.
echo.

REM --- [B] Mappaszerkezet es hivatkozasok beallitasa ---
set "BASED=%~dp0"
set "BASED=%BASED:~0,-1%"

REM A solution mappa megkeresese (a GitHub repo NagypapaHazaiSolutions neven tarolja)
if exist "%BASED%\NagypapaHazaiSolutions" (
    set "KOD_MAPPA=%BASED%\NagypapaHazaiSolutions"
) else if exist "%BASED%\..\NagypapaHazaiSolutions" (
    set "KOD_MAPPA=%BASED%\..\NagypapaHazaiSolutions"
) else if exist "%BASED%\Solutions" (
    set "KOD_MAPPA=%BASED%\Solutions"
) else (
    echo KRITIKUS: Nem talalhato a NagypapaHazaiSolutions mappa!
    echo Helyezze a .bat fajlt a repo gyokerebe vagy a mappa melle.
    pause
    exit /b
)
echo Solution mappa: !KOD_MAPPA!

set "CEL_MAPPA=%BASED%\Forditott"
set "WEB_KONYVTAR=%CEL_MAPPA%\Kliens"
set "SZERVER_KONYVTAR=%CEL_MAPPA%\Szolgaltatas"

REM A GitHub repo tenyleges projekt- es csproj nevei
set "UI_PROJEKT=!KOD_MAPPA!\NagypapahazaiBlazor\NagypapaHazaiBlazor.csproj"
set "API_PROJEKT=!KOD_MAPPA!\NagypapaHazai.API\NagypapaHazai.API.csproj"

if not exist "!UI_PROJEKT!" (
    echo KRITIKUS: Nem talalhato a web projekt: !UI_PROJEKT!
    pause
    exit /b
)
if not exist "!API_PROJEKT!" (
    echo KRITIKUS: Nem talalhato az API projekt: !API_PROJEKT!
    pause
    exit /b
)

set "NH_WEB_NEV=NagyPapa_Kliens_Site"
set "NH_API_NEV=NagyPapa_Szolgaltatas_Site"
set "NH_WEB_MEDENCE=NagyPapa_Kliens_AppPool"
set "NH_API_MEDENCE=NagyPapa_Szolgaltatas_AppPool"

REM Portok a launchSettings.json ertekei alapjan
set "UI_KAPU=5011"
set "API_KAPU=5002"

REM --- [C] Windows Webszerver funkciok aktivalasa ---
echo === 1. Windows Webszerver (IIS) komponensek telepitese ===
echo Kis turelmet, a hianyzo funkciok bekapcsolasa folyamatban van...
echo.

set "SZUKSEGES_ELEMEK=IIS-WebServerRole IIS-WebServer IIS-CommonHttpFeatures IIS-StaticContent IIS-DefaultDocument IIS-DirectoryBrowsing IIS-HttpErrors IIS-HttpRedirect IIS-ApplicationDevelopment IIS-NetFxExtensibility45 IIS-ISAPIExtensions IIS-ISAPIFilter IIS-HealthAndDiagnostics IIS-HttpLogging IIS-LoggingLibraries IIS-RequestMonitor IIS-Security IIS-RequestFiltering IIS-Performance IIS-HttpCompressionStatic IIS-WebServerManagementTools IIS-ManagementConsole IIS-ManagementScriptingTools IIS-WebSockets"

for %%X in (%SZUKSEGES_ELEMEK%) do (
    dism /online /enable-feature /featurename:%%X /all /norestart /quiet >nul 2>&1
)

set "IIS_VEZERLO=%WinDir%\System32\inetsrv\appcmd.exe"
if not exist "%IIS_VEZERLO%" (
    echo KRITIKUS: Az IIS vezerloalkalmazas nem elerheto.
    pause
    exit /b
)

REM --- [D] Futtatokornyezet felkutatasa ---
echo === 2. Microsoft .NET 8 futtatokornyezet detektalasa ===
set "KORNYEZET_MEGVAN=NEM"
if exist "%WinDir%\System32\inetsrv\aspnetcorev2.dll" set "KORNYEZET_MEGVAN=IGEN"

if "!KORNYEZET_MEGVAN!"=="IGEN" (
    echo A szukseges keretrendszer mar telepitve van a gepen.
    goto KORNYEZET_KESZ
)

echo Letoltes megkezdese a hianyzo .NET Hosting csomaghoz...
set "CSOMAG_VERZIO=8.0.11"
set "LETOLTESI_CIM=https://builds.dotnet.microsoft.com/dotnet/aspnetcore/Runtime/!CSOMAG_VERZIO!/dotnet-hosting-!CSOMAG_VERZIO!-win.exe"
set "IDEIGLENES_FAJL=%TEMP%\net8-install.exe"

powershell -Command "Invoke-WebRequest -Uri '!LETOLTESI_CIM!' -OutFile '!IDEIGLENES_FAJL!' -UseBasicParsing" >nul 2>&1

echo Installalas elinditva a hatterben...
start /WAIT "" "!IDEIGLENES_FAJL!" /install /quiet /norestart OPT_NO_RUNTIME=0 OPT_NO_SHAREDFX=0 OPT_NO_X86=0 OPT_NO_SHARED_CONFIG_CHECK=1

del /Q "!IDEIGLENES_FAJL!" >nul 2>&1
iisreset /restart >nul 2>&1

:KORNYEZET_KESZ
echo.

REM --- [E] Kod forditasa ---
echo === 3. Forraskod forditasa es elokeszitese ===
if exist "%IIS_VEZERLO%" (
    "%IIS_VEZERLO%" stop apppool "%NH_WEB_MEDENCE%" >nul 2>&1
    "%IIS_VEZERLO%" stop apppool "%NH_API_MEDENCE%" >nul 2>&1
)

if exist "%CEL_MAPPA%" rmdir /S /Q "%CEL_MAPPA%" >nul 2>&1
mkdir "%WEB_KONYVTAR%" >nul 2>&1
mkdir "%SZERVER_KONYVTAR%" >nul 2>&1

where dotnet >nul 2>&1
if %errorlevel% neq 0 (
    echo KRITIKUS: A dotnet SDK nincs telepitve vagy nincs a PATH-on.
    echo Telepitse a .NET 8 SDK-t: https://dotnet.microsoft.com/download
    pause
    exit /b
)

echo A Kliens (Blazor) alkalmazas generalasa fut...
dotnet publish "!UI_PROJEKT!" --configuration Release --output "%WEB_KONYVTAR%" --nologo /p:ErrorOnDuplicatePublishOutputFiles=false
if %errorlevel% neq 0 (
    echo FORDITASI HIBA: A webes feluletet nem sikerult feldolgozni.
    pause
    exit /b
)

echo A Szolgaltatas (API) generalasa fut...
REM Az API projekt hivatkozik a Blazor projektre (ProjectReference), ezert mindket projekt appsettings.json-ja atmasolodik.
REM Az ErrorOnDuplicatePublishOutputFiles=false feloldja a NETSDK1152 utkozest.
dotnet publish "!API_PROJEKT!" --configuration Release --output "%SZERVER_KONYVTAR%" --nologo /p:ErrorOnDuplicatePublishOutputFiles=false
if %errorlevel% neq 0 (
    echo FORDITASI HIBA: Az API epitese megszakadt.
    pause
    exit /b
)
echo.

REM --- [F] Webes kiszolgalo parameterezese ---
echo === 4. Webszerver beallitasok letrehozasa ===
"%IIS_VEZERLO%" delete apppool "%NH_WEB_MEDENCE%" >nul 2>&1
"%IIS_VEZERLO%" add apppool /name:"%NH_WEB_MEDENCE%" /managedRuntimeVersion:"" /managedPipelineMode:Integrated >nul 2>&1
"%IIS_VEZERLO%" set apppool "%NH_WEB_MEDENCE%" /processModel.identityType:ApplicationPoolIdentity >nul 2>&1

"%IIS_VEZERLO%" delete apppool "%NH_API_MEDENCE%" >nul 2>&1
"%IIS_VEZERLO%" add apppool /name:"%NH_API_MEDENCE%" /managedRuntimeVersion:"" /managedPipelineMode:Integrated >nul 2>&1
"%IIS_VEZERLO%" set apppool "%NH_API_MEDENCE%" /processModel.identityType:ApplicationPoolIdentity >nul 2>&1

"%IIS_VEZERLO%" delete site "%NH_WEB_NEV%" >nul 2>&1
"%IIS_VEZERLO%" add site /name:"%NH_WEB_NEV%" /physicalPath:"%WEB_KONYVTAR%" /bindings:http/*:%UI_KAPU%: >nul 2>&1
"%IIS_VEZERLO%" set site "%NH_WEB_NEV%" /[path='/'].applicationPool:"%NH_WEB_MEDENCE%" >nul 2>&1

"%IIS_VEZERLO%" delete site "%NH_API_NEV%" >nul 2>&1
"%IIS_VEZERLO%" add site /name:"%NH_API_NEV%" /physicalPath:"%SZERVER_KONYVTAR%" /bindings:http/*:%API_KAPU%: >nul 2>&1
"%IIS_VEZERLO%" set site "%NH_API_NEV%" /[path='/'].applicationPool:"%NH_API_MEDENCE%" >nul 2>&1
echo.

REM --- [G] Hozzaferesi jogositvanyok ---
echo === 5. Mappak hozzaferesi jogainak biztositasa ===
icacls "%CEL_MAPPA%" /grant "IIS_IUSRS:(OI)(CI)RX" /T /C /Q >nul 2>&1
icacls "%CEL_MAPPA%" /grant "IUSR:(OI)(CI)RX" /T /C /Q >nul 2>&1
echo.

REM --- [H] SQL kapcsolat felepitese ---
echo === 6. Adatbazis peldany es tuzfal konfiguracio ===
set "SQL_SZERVER=.\SQLEXPRESS"
set "SQL_SCRIPT=%BASED%\cucusql.sql"

if not exist "!SQL_SCRIPT!" (
    echo [ ! ] Nem talalhato a cucusql.sql a .bat melletti mappaban: !SQL_SCRIPT!
    echo SQL konfiguracio atugorva.
    goto SQL_KESZ
)

where sqlcmd >nul 2>&1
if %errorlevel% equ 0 (
    sqlcmd -S %SQL_SZERVER% -E -i "!SQL_SCRIPT!" -b
    if !errorlevel! neq 0 (
        echo [ ! ] Az SQL script futtatasa hibaval tert vissza. Ellenorizze, hogy az adatbazis letezik.
    ) else (
        echo SQL parancsfajl feldolgozasa sikeresen lezajlott.
    )
) else (
    echo SQLCMD hianyzik, az automatikus adatbazis konfiguracio atugorva.
)
:SQL_KESZ
echo.

REM --- [I] Halozati szabalyok es veglegesites ---
netsh advfirewall firewall delete rule name="Nagypapa_Kliens_Szabaly" >nul 2>&1
netsh advfirewall firewall delete rule name="Nagypapa_Szerver_Szabaly" >nul 2>&1
netsh advfirewall firewall add rule name="Nagypapa_Kliens_Szabaly" dir=in action=allow protocol=TCP localport=%UI_KAPU% >nul 2>&1
netsh advfirewall firewall add rule name="Nagypapa_Szerver_Szabaly" dir=in action=allow protocol=TCP localport=%API_KAPU% >nul 2>&1

"%IIS_VEZERLO%" start apppool "%NH_WEB_MEDENCE%" >nul 2>&1
"%IIS_VEZERLO%" start apppool "%NH_API_MEDENCE%" >nul 2>&1
"%IIS_VEZERLO%" start site "%NH_WEB_NEV%" >nul 2>&1
"%IIS_VEZERLO%" start site "%NH_API_NEV%" >nul 2>&1

echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo +   MINDEN FOLYAMAT SIKERESEN VEGREHAJTVA!                   +
echo ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
echo Felhasznaloi felulet elerhetosege: http://localhost:%UI_KAPU%
echo Hatarfelulet (API) elerhetosege:   http://localhost:%API_KAPU%
echo.
pause
endlocal
