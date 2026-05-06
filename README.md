# VideoGameTracker

ASP.NET Core MVC aplikacija za praćenje video igara, developera, žanrova, platformi, korisnika i recenzija.

## Pokretanje na drugom računalu

### Preduvjeti

- Instaliran .NET 10 SDK
- Instaliran i pokrenut Docker Desktop
- Preuzet ili kopiran cijeli projekt

### 1. Pokreni SQL Server container

U terminalu pokreni:

```bash
docker run -e ACCEPT_EULA=Y -e SA_PASSWORD=VideoGame123! -p 1433:1433 --name vgt-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Ako je container već jednom kreiran i samo ga želiš ponovno upaliti, koristi:

```bash
docker start vgt-sql
```

### 2. Provjeri connection string

U datoteci [appsettings.json](appsettings.json) connection string mora pokazivati na lokalni SQL Server container:

```json
"Server=localhost,1433;Database=VideoGameTrackerDb;User Id=sa;Password=VideoGame123!;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

### 3. Vrati NuGet pakete

Iz roota rješenja pokreni:

```bash
dotnet restore
```

### 4. Primijeni migracije

Prije prvog pokretanja baze primijeni EF migracije:

```bash
dotnet ef database update --project VideoGameTracker.csproj --startup-project VideoGameTracker.csproj
```

Ova naredba će kreirati bazu i tablice prema postojećim migracijama u folderu [Migrations](Migrations).

### 5. Pokreni aplikaciju

```bash
dotnet run --project VideoGameTracker.csproj
```

### 6. Otvori aplikaciju u pregledniku

Po pokretanju aplikacija će ispisati lokalnu adresu, najčešće nešto poput:

```bash
https://localhost:xxxx
```

Otvori tu adresu u browseru.

## Napomena

- `dotnet ef migrations add ...` se koristi samo kada mijenjaš modele i želiš generirati novu migraciju.
- `dotnet ef database update` samo primjenjuje postojeće migracije na bazu.
- Ako premještaš projekt na novo računalo, dovoljno je imati kod, Docker i .NET SDK; baza se može ponovno izgraditi kroz migracije.