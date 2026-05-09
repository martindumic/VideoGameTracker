# VideoGameTracker

ASP.NET Core MVC aplikacija za praćenje video igara, developera, žanrova, platformi, korisnika i recenzija.

## Pokretanje na drugom računalu

### Preduvjeti

- Instaliran .NET 10 SDK
- Preuzet ili kopiran cijeli projekt

### 1. Provjeri connection string

U datoteci [appsettings.json](appsettings.json) connection string mora pokazivati na lokalnu SQLite datoteku:

```json
"Data Source=VideoGameTracker.db"
```

### 2. Vrati NuGet pakete

Iz roota rješenja pokreni:

```bash
dotnet restore
```

### 3. Primijeni migracije

Prije prvog pokretanja baze primijeni EF migracije:

```bash
dotnet ef database update --project VideoGameTracker.csproj --startup-project VideoGameTracker.csproj
```

Ova naredba će kreirati bazu i tablice prema postojećim migracijama u folderu [Migrations](Migrations).

### 4. Pokreni aplikaciju

```bash
dotnet run --project VideoGameTracker.csproj
```

### 5. Otvori aplikaciju u pregledniku

Po pokretanju aplikacija će ispisati lokalnu adresu, najčešće nešto poput:

```bash
https://localhost:xxxx
```

Otvori tu adresu u browseru.

## Napomena

- `dotnet ef migrations add ...` se koristi samo kada mijenjaš modele i želiš generirati novu migraciju.
- `dotnet ef database update` samo primjenjuje postojeće migracije na bazu.
- Ako premještaš projekt na novo računalo, dovoljno je imati kod i .NET SDK; baza se može ponovno izgraditi kroz migracije.