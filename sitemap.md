# Sitemap (Semantic Routing Model)

## Autentikacija i opce stranice

- GET / -> LoginController.Index -> Views/Login/Index.cshtml
- GET /login -> LoginController.Index -> Views/Login/Index.cshtml
- POST /login -> LoginController.Index -> Views/Login/Index.cshtml (u slucaju greske) / redirect
- GET /login/logout -> LoginController.Logout -> redirect na /login

- GET /home -> HomeController.Index -> Views/Home/Index.cshtml
- POST /home/create-game-entry -> HomeController.CreateGameEntry -> redirect na /home
- GET /home/privacy -> HomeController.Privacy -> Views/Home/Privacy.cshtml
- GET /home/error -> HomeController.Error -> Views/Shared/Error.cshtml

## Games

- GET /games -> GamesController.Index -> Views/Games/Index.cshtml
- GET /games/{id} -> GamesController.Details -> Views/Games/Details.cshtml

## Developers

- GET /developers -> DevelopersController.Index -> Views/Developers/Index.cshtml
- GET /developers/{id} -> DevelopersController.Details -> Views/Developers/Details.cshtml

## Users

- GET /users -> UsersController.Index -> Views/Users/Index.cshtml
- GET /users/{id} -> UsersController.Details -> Views/Users/Details.cshtml

## Game Entries

- GET /game-entries -> GameEntriesController.Index -> Views/GameEntries/Index.cshtml
- GET /game-entries/{id} -> GameEntriesController.Details -> Views/GameEntries/Details.cshtml

## Genres

- GET /genres -> GenresController.Index -> Views/Genres/Index.cshtml
- GET /genres/{id} -> GenresController.Details -> Views/Genres/Details.cshtml

## Platforms

- GET /platforms -> PlatformsController.Index -> Views/Platforms/Index.cshtml
- GET /platforms/{id} -> PlatformsController.Details -> Views/Platforms/Details.cshtml

## Napomena o kompatibilnosti

Aplikacija i dalje ima konfiguriran konvencionalni fallback route ({controller}/{action}/{id?}) radi kompatibilnosti sa starijim URL-ovima i linkovima, ali gore navedene rute su primarne i eksplicitno definirane attribute routingom.
