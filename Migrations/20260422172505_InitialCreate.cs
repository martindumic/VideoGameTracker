using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VideoGameTracker.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Developers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Founded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeveloperId = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_Developers_DeveloperId",
                        column: x => x.DeveloperId,
                        principalTable: "Developers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameGenres",
                columns: table => new
                {
                    GamesId = table.Column<int>(type: "int", nullable: false),
                    GenresId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGenres", x => new { x.GamesId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_GameGenres_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGenres_Genres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamePlatforms",
                columns: table => new
                {
                    GamesId = table.Column<int>(type: "int", nullable: false),
                    PlatformsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamePlatforms", x => new { x.GamesId, x.PlatformsId });
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamePlatforms_Platforms_PlatformsId",
                        column: x => x.PlatformsId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursPlayed = table.Column<int>(type: "int", nullable: false),
                    ReviewId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEntries_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GameEntries_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GameEntries_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Developers",
                columns: new[] { "Id", "Country", "Description", "Founded", "Name" },
                values: new object[,]
                {
                    { 1, "Poland", "Polish video game developer known for The Witcher series", new DateTime(2002, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "CD Projekt Red" },
                    { 2, "USA", "American video game developer and publisher known for GTA series", new DateTime(1998, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rockstar Games" },
                    { 3, "USA", "Developer of legendary fps games and Half-Life series", new DateTime(1996, 8, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Valve Corporation" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Games with character progression and storytelling", "Role-Playing Game" },
                    { 2, "Fast-paced action-oriented games", "Action" },
                    { 3, "Fast shooter games played from first-person perspective", "First-Person Shooter" },
                    { 4, "Adventure and exploration games", "Adventure" }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "PC", 0 },
                    { 2, "PlayStation 5", 1 },
                    { 3, "Xbox Series X", 2 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "RegisteredAt", "Username" },
                values: new object[,]
                {
                    { 1, "gamerpro@email.com", "hashed_password_1", new DateTime(2020, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "GamerPro" },
                    { 2, "rpgfan@email.com", "hashed_password_2", new DateTime(2021, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "RPGFanatic" },
                    { 3, "actionjunkie@email.com", "hashed_password_3", new DateTime(2019, 11, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "ActionJunkie" }
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "AverageRating", "Description", "DeveloperId", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, 95, "Open-world RPG with rich storytelling and complex characters", 1, 2015, "The Witcher 3: Wild Hunt" },
                    { 2, 77, "Futuristic action RPG set in Night City", 1, 2020, "Cyberpunk 2077" },
                    { 3, 88, "Predecessor to The Witcher 3", 1, 2011, "The Witcher 2: Assassins of Kings" },
                    { 4, 97, "Open-world action game set in Los Santos", 2, 2013, "Grand Theft Auto V" },
                    { 5, 97, "Western action-adventure game", 2, 2018, "Red Dead Redemption 2" },
                    { 6, 94, "Action game set in Liberty City", 2, 2008, "GTA IV" },
                    { 7, 96, "Revolutionary first-person shooter", 3, 2004, "Half-Life 2" },
                    { 8, 82, "Competitive team-based FPS", 3, 2023, "Counter-Strike 2" },
                    { 9, 93, "VR prequel to Half-Life 2", 3, 2020, "Half-Life: Alyx" }
                });

            migrationBuilder.InsertData(
                table: "GameEntries",
                columns: new[] { "Id", "DateAdded", "GameId", "HoursPlayed", "ReviewId", "Status", "UserId" },
                values: new object[,]
                {
                    { 7, new DateTime(2023, 11, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, 0, null, 0, 3 },
                    { 8, new DateTime(2020, 10, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 95, null, 2, 2 }
                });

            migrationBuilder.InsertData(
                table: "GameGenres",
                columns: new[] { "GamesId", "GenresId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 4 },
                    { 2, 1 },
                    { 2, 2 },
                    { 3, 1 },
                    { 3, 2 },
                    { 4, 2 },
                    { 4, 4 },
                    { 5, 2 },
                    { 5, 4 },
                    { 6, 2 },
                    { 7, 2 },
                    { 7, 3 },
                    { 8, 3 },
                    { 9, 2 },
                    { 9, 3 }
                });

            migrationBuilder.InsertData(
                table: "GamePlatforms",
                columns: new[] { "GamesId", "PlatformsId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 1 },
                    { 2, 2 },
                    { 2, 3 },
                    { 3, 1 },
                    { 4, 1 },
                    { 4, 2 },
                    { 4, 3 },
                    { 5, 1 },
                    { 5, 2 },
                    { 5, 3 },
                    { 6, 1 },
                    { 6, 3 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Comment", "CreatedAt", "GameId", "Score", "UserId" },
                values: new object[,]
                {
                    { 1, "Amazing game, incredible story and characters!", new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 95, 1 },
                    { 2, "Best RPG I've ever played!", new DateTime(2023, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 98, 2 },
                    { 3, "Great open-world gameplay", new DateTime(2023, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 90, 1 },
                    { 4, "Competitive and fun", new DateTime(2023, 8, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 85, 3 },
                    { 5, "Good game, but buggy at launch", new DateTime(2023, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 75, 2 },
                    { 6, "Outstanding story and immersion", new DateTime(2023, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 96, 3 }
                });

            migrationBuilder.InsertData(
                table: "GameEntries",
                columns: new[] { "Id", "DateAdded", "GameId", "HoursPlayed", "ReviewId", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 150, 1, 2, 1 },
                    { 2, new DateTime(2022, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 200, 2, 2, 2 },
                    { 3, new DateTime(2023, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, 80, 3, 1, 1 },
                    { 4, new DateTime(2023, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, 45, 4, 1, 3 },
                    { 5, new DateTime(2021, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 120, 5, 2, 2 },
                    { 6, new DateTime(2022, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, 180, 6, 2, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_GameId",
                table: "GameEntries",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_ReviewId",
                table: "GameEntries",
                column: "ReviewId",
                unique: true,
                filter: "[ReviewId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_UserId",
                table: "GameEntries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGenres_GenresId",
                table: "GameGenres",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_GamePlatforms_PlatformsId",
                table: "GamePlatforms",
                column: "PlatformsId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_DeveloperId",
                table: "Games",
                column: "DeveloperId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_GameId",
                table: "Reviews",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameEntries");

            migrationBuilder.DropTable(
                name: "GameGenres");

            migrationBuilder.DropTable(
                name: "GamePlatforms");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Developers");
        }
    }
}
