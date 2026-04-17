# NagypapaHazai – Telepítési útmutató

Ez a dokumentum lépésről lépésre végigvezet a **NagypapaHazai** rendszer helyi (Windows + IIS + SQL Server) telepítésén a mellékelt `.bat` és `.sql` fájlok segítségével.

Forrás repó: [https://github.com/NagypapaHazai/NagypapaHazaiBlazor](https://github.com/NagypapaHazai/NagypapaHazaiBlazor)

---

## 1. Előfeltételek

A telepítő gépen a következőkre van szükség:

| Komponens | Minimum verzió | Letöltés |
|-----------|---------------|----------|
| Windows 10 / 11 vagy Windows Server | — | — |
| .NET 8 SDK | 8.0.x | <https://dotnet.microsoft.com/download/dotnet/8.0> |
| SQL Server Express | 2019+ | <https://www.microsoft.com/sql-server/sql-server-downloads> |
| SQL Server Management Studio (SSMS) | legújabb | <https://learn.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms> |
| `sqlcmd` segédprogram | része az SQL Server / SSMS telepítésnek | — |

> **Fontos:** A `.bat` fájlokat **rendszergazdai (Administrator) parancssorból** kell futtatni. Jobb klikk → „Futtatás rendszergazdaként".

Az IIS-t **nem kell** előre telepíteni – a `NagypapaHazai.bat` bekapcsolja a szükséges Windows-funkciókat.

A .NET 8 Hosting Bundle-t sem kell külön telepíteni – a `NagypapaHazai.bat` ezt is letölti és feltelepíti, ha hiányzik.

---

## 2. Mappaszerkezet előkészítése

1. Klónozd vagy töltsd le ZIP-ben a [NagypapaHazaiBlazor repót](https://github.com/NagypapaHazai/NagypapaHazaiBlazor).
2. Csomagold ki pl. ide: `C:\Users\User\Desktop\NagypapaHazai\`.
3. A letöltött tartalom így nézzen ki:

```
NagypapaHazai\
├── NagypapaHazaiSolutions\
│   ├── NagypapaHazai.API\
│   ├── NagypapaHazai.Shared\
│   ├── NagypapahazaiBlazor\
│   └── NagypapaHazaiBlazor.sln
├── bacpac\
│   └── NagypapaHazai.bacpac
├── Documents\
└── README.md
```

4. Másold a **javított fájlokat** ebbe a gyökérmappába (vagy egy almappába, pl. `Published\`, a `.bat` automatikusan megtalálja a `NagypapaHazaiSolutions`-t):

```
NagypapaHazai\
├── NagypapaHazai.bat
├── NagypapaHazaiStart.bat
├── NagypapaHazaiUninstall.bat
└── cucusql.sql
```

A `cucusql.sql`-nek **ugyanabban a mappában** kell lennie, ahol a `NagypapaHazai.bat` van.

---

## 3. Adatbázis importálása (egyszeri lépés)

A `bacpac\NagypapaHazai.bacpac` fájlból kell létrehozni a `NagypapaHazai` nevű adatbázist.

### SSMS-ben:

1. Nyisd meg az **SQL Server Management Studio**-t.
2. Csatlakozz a `.\SQLEXPRESS` példányhoz Windows hitelesítéssel.
3. Object Explorer → **Databases** mappán jobb klikk → **Import Data-tier Application…**
4. Next → Browse → válaszd ki a `bacpac\NagypapaHazai.bacpac` fájlt.
5. Database name: **`NagypapaHazai`** (pontosan így, ékezet nélkül).
6. Next → Finish. Várd meg, amíg befejeződik.

Ellenőrzés: a Databases alatt jelenjen meg a `NagypapaHazai` adatbázis.

> **Miért ez a név?** Az `appsettings.json` a repóban ezt a nevet használja a connection stringben. Ha mást adsz meg, a `.bat` és a `cucusql.sql` sem fogja megtalálni.

---

## 4. Telepítés – `NagypapaHazai.bat`

Ez a fő telepítő script. Mit csinál:

1. Ellenőrzi a rendszergazdai jogokat.
2. Bekapcsolja a szükséges IIS Windows-funkciókat (WebServer, ManagementConsole, stb.).
3. Ellenőrzi a .NET 8 ASP.NET Core Hosting Bundle meglétét, ha hiányzik, letölti és telepíti.
4. Lefuttatja a `dotnet publish`-t a Blazor UI-ra és az API-ra → `Forditott\Kliens` és `Forditott\Szolgaltatas` mappákba.
5. Létrehozza az IIS Application Pool-okat (`NagyPapa_Kliens_AppPool`, `NagyPapa_Szolgaltatas_AppPool`) és a Site-okat (`NagyPapa_Kliens_Site` a **5011**-es porton, `NagyPapa_Szolgaltatas_Site` a **5002**-es porton).
6. Beállítja az `IIS_IUSRS` / `IUSR` olvasási jogokat a kimeneti mappákra.
7. Futtatja a `cucusql.sql`-t → létrehozza az IIS AppPool login-okat az SQL Serverben és `db_owner` jogot ad nekik a `NagypapaHazai` adatbázison.
8. Tűzfalszabályokat hoz létre a két portra.
9. Elindítja az AppPool-okat és Site-okat.

### Futtatás

1. Keresd meg a Start menüben a **Command Prompt**-ot → jobb klikk → **Futtatás rendszergazdaként**.
2. Lépj be a mappába:
   ```
   cd /d C:\Users\User\Desktop\NagypapaHazai
   ```
3. Indítsd el:
   ```
   NagypapaHazai.bat
   ```
4. Várd meg, amíg végigmegy az összes 1–6. fázison (build + IIS + SQL + tűzfal). Első futtatáskor a .NET Hosting Bundle letöltése **több percig** is eltarthat.

### Sikeres befejezés után

A képernyőn a következőket látod:

```
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
+   MINDEN FOLYAMAT SIKERESEN VEGREHAJTVA!                   +
++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
Felhasznaloi felulet elerhetosege: http://localhost:5011
Hatarfelulet (API) elerhetosege:   http://localhost:5002
```

Nyisd meg a böngészőt: <http://localhost:5011>

---

## 5. A program indítása / újraindítása – `NagypapaHazaiStart.bat`

Ha a gépet újraindítottad, vagy az IIS leállt, ezzel gyorsan vissza tudod kapcsolni az alkalmazást – **nem kell újra telepíteni**.

Mit csinál:

- Ellenőrzi a rendszergazdai jogokat.
- Elindítja a `W3SVC` (IIS) és `WAS` szolgáltatásokat, ha nem futnak.
- Ellenőrzi, hogy a két Site (`NagyPapa_Kliens_Site`, `NagyPapa_Szolgaltatas_Site`) létezik-e. Ha nem, szól, hogy előbb a `NagypapaHazai.bat`-ot kell futtatni.
- Elindítja az AppPool-okat és a Site-okat.
- Felajánlja a böngésző megnyitását.

### Futtatás

1. Jobb klikk a `NagypapaHazaiStart.bat`-on → **Futtatás rendszergazdaként**.
2. A végén `I` (Igen) billentyűvel a böngésző automatikusan megnyitja a <http://localhost:5011> címet.

---

## 6. Eltávolítás – `NagypapaHazaiUninstall.bat`

Ha szeretnéd teljesen törölni az IIS-be telepített komponenseket (pl. új telepítés előtt), futtasd ezt.

Mit csinál:

1. Ellenőrzi a rendszergazdai jogokat és megerősítést kér.
2. Leállítja és törli a két IIS Site-ot.
3. Leállítja és törli a két IIS Application Pool-t.
4. Visszavonja az IIS AppPool-ok hozzáférését az SQL Serverből (USER + LOGIN drop).
5. Törli a tűzfalszabályokat.

### Futtatás

1. Jobb klikk a `NagypapaHazaiUninstall.bat`-on → **Futtatás rendszergazdaként**.
2. A megerősítésnél nyomj `I`-t a folytatáshoz vagy `N`-t a megszakításhoz.

> **Figyelem:** A `Forditott` mappa (a lefordított binárisok) **nem kerülnek törlésre** – ezt kézzel kell kitörölni, ha már nem kell. A `NagypapaHazai` adatbázis **sem törlődik**, csak a hozzáférési jogok. Ha az adatbázist is törölni akarod, SSMS-ben jobb klikk a `NagypapaHazai` adatbázison → Delete.

---

## 7. Hibaelhárítás

### „Hianyozo privilegiumok!"
Nem rendszergazda módban futtattad. Zárd be, jobb klikk → Futtatás rendszergazdaként.

### „KRITIKUS: Nem talalhato a NagypapaHazaiSolutions mappa!"
A `.bat` fájl nem a repó gyökerében van. Helyezd a `NagypapaHazai.bat`-ot közvetlenül a `NagypapaHazai\` mappába (ahol a `NagypapaHazaiSolutions\` is van), vagy egy almappába, pl. `NagypapaHazai\Published\`.

### „KRITIKUS: A dotnet SDK nincs telepitve"
Telepítsd a .NET 8 SDK-t: <https://dotnet.microsoft.com/download/dotnet/8.0>. Indíts új parancssort a telepítés után.

### `NETSDK1152: Found multiple publish output files with the same relative path`
A javított `NagypapaHazai.bat` már tartalmazza a `/p:ErrorOnDuplicatePublishOutputFiles=false` kapcsolót, ami ezt kezeli. Ha mégis előjön, győződj meg róla, hogy a legfrissebb javított verziót használod.

### „FIGYELEM: A NagypapaHazai adatbazis meg nem letezik a szerveren."
Nem importáltad a bacpac-et, vagy más néven importáltad. Ismételd meg a **3. fejezet** lépéseit.

### „SQLCMD hianyzik, az automatikus adatbazis konfiguracio atugorva."
Telepítsd az SSMS-t vagy az SQL Server Command Line Utilities csomagot. Alternatíva: nyisd meg a `cucusql.sql`-t SSMS-ben és futtasd kézzel (F5).

### A böngésző „HTTP 500.19" vagy „HTTP 500.30" hibát dob
A .NET Hosting Bundle telepítése után az IIS-t újra kell indítani:
```
iisreset
```
Ha ez sem segít, indítsd újra a gépet, majd futtasd a `NagypapaHazaiStart.bat`-ot.

### A 5011 vagy 5002 port már foglalt
Keresd meg, mi foglalja: `netstat -ano | findstr :5011`. Szabadítsd fel a portot, vagy módosítsd a `.bat` fájlban a `UI_KAPU` / `API_KAPU` változókat, valamint az IIS Managerben a site binding-ot.

---

## 8. Gyors összefoglaló parancsok

Rendszergazdai parancssorból, a `NagypapaHazai\` mappából:

```bat
REM Első telepítés
NagypapaHazai.bat

REM Indítás újraindítás után
NagypapaHazaiStart.bat

REM Eltávolítás
NagypapaHazaiUninstall.bat
```

Elérhetőségek telepítés után:

- Felhasználói felület: <http://localhost:5011>
- API: <http://localhost:5002>
- API Swagger (csak Development környezetben): <http://localhost:5002/swagger>

---

**Sok sikert a telepítéshez!**
