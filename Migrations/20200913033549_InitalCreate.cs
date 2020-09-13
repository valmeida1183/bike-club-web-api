using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace bike_club_api.Migrations
{
    public partial class InitalCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(maxLength: 50, nullable: false),
                    Complement = table.Column<string>(maxLength: 50, nullable: false),
                    State = table.Column<string>(maxLength: 2, nullable: false),
                    City = table.Column<string>(maxLength: 30, nullable: false),
                    ZipCode = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Difficulties",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Difficulties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    Code = table.Column<string>(maxLength: 1, nullable: false),
                    Description = table.Column<string>(maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.Code);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Bikes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gears = table.Column<int>(maxLength: 36, nullable: false, defaultValue: 0),
                    FrameSize = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    RimSize = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    Model = table.Column<string>(maxLength: 20, nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: false),
                    Price = table.Column<decimal>(type: "money", nullable: false),
                    Image = table.Column<string>(maxLength: 50, nullable: false),
                    GenderCode = table.Column<string>(nullable: false),
                    CategoryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bikes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bikes_Genders_GenderCode",
                        column: x => x.GenderCode,
                        principalTable: "Genders",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Password = table.Column<string>(maxLength: 8000, nullable: false),
                    Phone = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    LastName = table.Column<string>(maxLength: 50, nullable: false),
                    GenderCode = table.Column<string>(nullable: false),
                    RoleName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Genders_GenderCode",
                        column: x => x.GenderCode,
                        principalTable: "Genders",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleName",
                        column: x => x.RoleName,
                        principalTable: "Roles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShopCarts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PurchaseDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "money", nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopCarts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopCarts_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShopCarts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Description = table.Column<string>(maxLength: 300, nullable: false),
                    MonitorId = table.Column<int>(nullable: false),
                    DifficultyId = table.Column<int>(nullable: false),
                    AddressId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tours_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tours_Difficulties_DifficultyId",
                        column: x => x.DifficultyId,
                        principalTable: "Difficulties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tours_Users_MonitorId",
                        column: x => x.MonitorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Purchases",
                columns: table => new
                {
                    ShopCartId = table.Column<int>(nullable: false),
                    BikeId = table.Column<int>(nullable: false),
                    Quantity = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Purchases", x => new { x.BikeId, x.ShopCartId });
                    table.ForeignKey(
                        name: "FK_Purchases_Bikes_BikeId",
                        column: x => x.BikeId,
                        principalTable: "Bikes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Purchases_ShopCarts_ShopCartId",
                        column: x => x.ShopCartId,
                        principalTable: "ShopCarts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TourParticipants",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    TourId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourParticipants", x => new { x.UserId, x.TourId });
                    table.ForeignKey(
                        name: "FK_TourParticipants_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TourParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Road" },
                    { 2, "Mountain" },
                    { 3, "Hybrid" },
                    { 4, "Touring" },
                    { 5, "Folding" },
                    { 6, "Women's" },
                    { 7, "Eletric" }
                });

            migrationBuilder.InsertData(
                table: "Difficulties",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Very Easy" },
                    { 2, "Easy" },
                    { 3, "Medium" },
                    { 4, "Hard" },
                    { 5, "Very Hard" }
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "Code", "Description" },
                values: new object[,]
                {
                    { "M", "Male" },
                    { "F", "Female" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Name", "Description" },
                values: new object[,]
                {
                    { "Monitor", "Monitor, responsible for managing the tours" },
                    { "Cyclist", "Cyclist is the common user" }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 1, 2, "The classic mountain bike. Equipped with front suspension, V-Brake and aluminum frame.", 17.5m, 18, "M", "mountain1.png", "Silver Mountain", 600.90m, 27.5m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[,]
                {
                    { 18, 7, "Eletric tour bike. Equipped with V-Brake brakes and carbon fiber frame, powerfull front light, rear and front load support", 17.5m, "F", "tour6.png", "Orange Eletric", 3200.0m, 27.5m },
                    { 19, 4, "Root tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", 17.5m, "M", "tour7.png", "Blue Tour", 600.0m, 29.0m }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[,]
                {
                    { 17, 7, "Eletric tour bike. Equipped with V-Brake brakes and carbon fiber frame, powerfull front light and rear load support", 17.5m, 21, "M", "tour5.png", "Green Eletric", 3000.0m, 27.5m },
                    { 16, 4, "Advanced tour bike. Equipped with V-Brake brakes and carbon fiber frame", 17.5m, 18, "M", "tour4.png", "Green Tour II", 950.0m, 29.9m }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 15, 4, "Root tour bike. Equipped with V-Brake brakes, aluminum frame and rear load support", 17.5m, "M", "tour3.png", "Green Tour", 550.0m, 27.5m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[,]
                {
                    { 14, 3, "More versatile hybrid bike. Equipped with V-Brake brakes and aluminum frame", 17.5m, 18, "M", "tour2.png", "Gray Tour", 450.0m, 27.5m },
                    { 13, 1, "Special and exclusive model of Road bike. Equipped with disc brakes, carbon fiber frame and thin tires.", 17.5m, 36, "M", "road6.png", "Black Widow", 9000.0m, 29.0m }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 12, 1, "Root road model. Equipped with V-Brake and aluminum frame.", 17.5m, "M", "road5.png", "Blue Speed", 500.0m, 29.0m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 20, 4, "Special tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", 17.5m, 6, "F", "tour8.png", "Pink Tour", 400.0m, 27.5m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 11, 1, "Root road model. Equipped with V-Brake and aluminum frame.", 17.5m, "M", "road4.png", "Green Speed", 500.0m, 27.5m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[,]
                {
                    { 9, 1, "The top of road bikes. Equipped with disc brakes, carbon fiber frame and thin tires.", 17.5m, 21, "M", "road2.png", "Scarlet Speed", 6100.90m, 29.0m },
                    { 8, 1, "Classic road model. Equipped with V-brake and aluminum frame.", 17.5m, 10, "M", "road1.png", "Orange Speed", 350.00m, 27.5m },
                    { 7, 2, "The top of mountain bikes. Equipped with RockShox front suspension, disc brakes, carbon fiber frame and Maxxis Minion tires.", 17.5m, 36, "M", "mountain6.png", "Scarlet Mountain", 5790.90m, 29.0m },
                    { 6, 2, "Silver mountain's advanced model. Equipped with RockShox front suspension, disc brakes, carbon fiber frame.", 17.5m, 21, "M", "mountain5.png", "Silver Mountain II", 2390.90m, 29.0m },
                    { 5, 4, "The classic bicycle for tours. Equipped with V-Brake brakes, aluminum frame, and rear and front load support.", 17.5m, 18, "M", "tour1.png", "Orange Tour", 380.90m, 27.5m },
                    { 4, 2, "The winter bike. Equipped with larger, non-slip special tires, disc brakes, aluminum frame.", 17.5m, 21, "M", "mountain4.png", "Snow Mountain", 1400.90m, 29.0m },
                    { 3, 3, "Advanced hybrid model. Equipped with front suspension, disc brakes, aluminum frame.", 17.5m, 21, "M", "mountain3.png", "Blue Sky", 1200.90m, 27.5m },
                    { 2, 3, "Versatile model, classified as hybrid. Equipped with front suspension, V-Brake, aluminum frame.", 17.5m, 10, "M", "mountain2.png", "Mechanical Orange", 450.90m, 27.5m },
                    { 10, 1, "Advanced road bike model. Equipped with disc brakes and aluminum frame.", 17.5m, 18, "M", "road3.png", "Yellow Speed", 2200.90m, 29.0m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "GenderCode", "LastName", "Name", "Password", "Phone", "RoleName" },
                values: new object[] { 1, "administrator@administrator.com", "M", "Master", "Admin", "1lFqy5Swsz77Zh/Us7s2uMNMW+Fwhjl8PyhcDR2cpoU=", "(99)99999-9999", "Monitor" });

            migrationBuilder.CreateIndex(
                name: "IX_Bikes_CategoryId",
                table: "Bikes",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Bikes_GenderCode",
                table: "Bikes",
                column: "GenderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_ShopCartId",
                table: "Purchases",
                column: "ShopCartId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCarts_AddressId",
                table: "ShopCarts",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopCarts_UserId",
                table: "ShopCarts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TourParticipants_TourId",
                table: "TourParticipants",
                column: "TourId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_AddressId",
                table: "Tours",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_DifficultyId",
                table: "Tours",
                column: "DifficultyId");

            migrationBuilder.CreateIndex(
                name: "IX_Tours_MonitorId",
                table: "Tours",
                column: "MonitorId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderCode",
                table: "Users",
                column: "GenderCode");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleName",
                table: "Users",
                column: "RoleName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Purchases");

            migrationBuilder.DropTable(
                name: "TourParticipants");

            migrationBuilder.DropTable(
                name: "Bikes");

            migrationBuilder.DropTable(
                name: "ShopCarts");

            migrationBuilder.DropTable(
                name: "Tours");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Difficulties");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Genders");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
