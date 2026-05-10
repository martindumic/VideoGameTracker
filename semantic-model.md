# Semantic DB Model

## Developer
Kratki opis: Studio ili tvrtka koja razvija igre.

Glavna svojstva:
- Id
- Name
- Country
- Founded
- Description
- Games

## Game
Kratki opis: Videoigra u katalogu aplikacije.

Glavna svojstva:
- Id
- Title
- ReleaseYear
- Description
- DeveloperId
- Developer
- AverageRating
- Genres
- Platforms
- GameEntries

## GameEntry
Kratki opis: Korisnicki zapis o igranju odredene igre (status, sati, opcionalna recenzija).

Glavna svojstva:
- Id
- GameId
- Game
- UserId
- User
- Status
- CreatedAt
- HoursPlayed
- ReviewScore
- ReviewComment

## Genre
Kratki opis: Zanr igre (npr. RPG, Action).

Glavna svojstva:
- Id
- Name
- Description
- Games

## Platform
Kratki opis: Platforma na kojoj je igra dostupna (PC, PlayStation, Xbox).

Glavna svojstva:
- Id
- Name
- Type
- Games

## User
Kratki opis: Korisnik aplikacije.

Glavna svojstva:
- Id
- Username
- Email
- Password
- RegisteredAt
- GameEntries

## Veze medu entitetima

1-N veze:
- Developer -> Game (1:N), FK: Game.DeveloperId
- Game -> GameEntry (1:N), FK: GameEntry.GameId
- User -> GameEntry (1:N), FK: GameEntry.UserId

N-N veze:
- Game <-> Genre (N:N), join tablica: GameGenres
- Game <-> Platform (N:N), join tablica: GamePlatforms

Opcionalne veze:
- GameEntry ima opcionalne recenzijske podatke (ReviewScore, ReviewComment).
