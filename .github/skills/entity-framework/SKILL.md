---
name: entity-framework
description: "Use when: adding or modifying EF entities, relationships, DbSet properties, OnModelCreating configuration, or generating/applying migrations in VideoGameTracker."
---

# Entity Framework Skill (VideoGameTracker)

## Namjena
Ovaj skill standardizira izmjene EF modela u projektu VideoGameTracker tako da relacije, migracije, seed podaci i repozitoriji ostanu konzistentni.

## Kada koristiti ovaj skill
Koristi ovaj skill kada treba:
- dodati ili izmijeniti EF entitet
- dodati ili izmijeniti `[Key]`, `[ForeignKey]`, `virtual` navigacijska svojstva i `ICollection<T>`
- dodati novi `DbSet<T>` u `VideoGameTrackerDbContext`
- prilagoditi relacije u `OnModelCreating`
- generirati ili primijeniti EF migraciju
- uskladiti promjene s repozitorijima i postojećim seed podacima

## Pravila za izmjene EF modela

1. Entiteti:
- svaki entitet mora imati primarni kljuc (`Id`) i `[Key]`
- FK svojstva oznaci s `[ForeignKey(nameof(NavigationProperty))]` kada je primjenjivo
- navigacijska svojstva ostavi `virtual`
- kolekcijske navigacije definiraj kao `virtual ICollection<T>` i inicijaliziraj ih konstruktorom

2. Relacije:
- 1-N: osiguraj FK na N strani i odgovarajucu kolekciju na 1 strani
- N-N: definiraj kolekcije na obje strane i mapiranje preko `UsingEntity` u `OnModelCreating`
- opcionalne veze: FK treba biti nullable (`int?`) i konfiguriran odgovarajuci `OnDelete`

3. DbContext:
- dodaj `DbSet<T>` za svaki novi entitet
- sve netrivijalne relacije eksplicitno konfiguriraj u `OnModelCreating`
- ne mijenjaj `OnDelete` ponasanje bez provjere utjecaja na postojeci seed i podatke

4. Repozitoriji:
- nakon promjene modela provjeri da repository upiti (`GetAll`, `GetById`, `Include`) i dalje rade
- ako je relacija promijenjena, prilagodi eager loading gdje je potreban

## Pravila za migracije

1. Prije migracije:
- build mora prolaziti bez gresaka
- model i `DbContext` moraju biti uskladeni
- provjeri da promjena nije vec obuhvacena nekom postojecnom migracijom

2. Generiranje migracije (primjer):
- `dotnet ef migrations add <DescriptiveName> --project VideoGameTracker/VideoGameTracker.csproj --startup-project VideoGameTracker/VideoGameTracker.csproj --context VideoGameTrackerDbContext`

3. Primjena migracije:
- `dotnet ef database update --project VideoGameTracker/VideoGameTracker.csproj --startup-project VideoGameTracker/VideoGameTracker.csproj --context VideoGameTrackerDbContext`

4. Validacija nakon migracije:
- provjeri da su migracijske datoteke smislenog diff-a
- provjeri da seed radi i da nema FK konflikata
- pokreni aplikaciju i provjeri glavne list/details stranice

## Checklist prije izmjene
- [ ] Razumijem postojecu relaciju i cardinality
- [ ] Identificirao sam sve zahvacene modele i repozitorije
- [ ] Planirano je kako ce promjena utjecati na seed podatke
- [ ] Postoji jasan naziv migracije

## Checklist poslije izmjene
- [ ] Build prolazi
- [ ] Migracija se uspjesno generira
- [ ] `database update` prolazi bez greske
- [ ] Repozitoriji rade bez runtime gresaka
- [ ] List/Details stranice za zahvacene entitete rade ispravno

## Primjer koristenja

Prompt primjer:
"Primijeni entity-framework skill: dodaj optional relaciju iz `GameEntry` prema novom audit entitetu, dodaj `DbSet`, konfiguriraj `OnModelCreating`, napravi migraciju i provjeri da postojeci seed i repozitoriji nisu razbijeni."
