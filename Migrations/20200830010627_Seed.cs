using Microsoft.EntityFrameworkCore.Migrations;

namespace bike_club_api.Migrations
{
    public partial class Seed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Bikes",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

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
                values: new object[] { 19, 4, "Root tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", 17.5m, "M", "tour7.png", "Blue Tour", 600.0m, 29.0m });

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
                values: new object[,]
                {
                    { 12, 1, "Root road model. Equipped with V-Brake and aluminum frame.", 17.5m, "M", "road5.png", "Blue Speed", 500.0m, 29.0m },
                    { 11, 1, "Root road model. Equipped with V-Brake and aluminum frame.", 17.5m, "M", "road4.png", "Green Speed", 500.0m, 27.5m }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[,]
                {
                    { 10, 1, "Advanced road bike model. Equipped with disc brakes and aluminum frame.", 17.5m, 18, "M", "road3.png", "Yellow Speed", 2200.90m, 29.0m },
                    { 9, 1, "The top of road bikes. Equipped with disc brakes, carbon fiber frame and thin tires.", 17.5m, 21, "M", "road2.png", "Scarlet Speed", 6100.90m, 29.0m },
                    { 8, 1, "Classic road model. Equipped with V-brake and aluminum frame.", 17.5m, 10, "M", "road1.png", "Orange Speed", 350.00m, 27.5m },
                    { 7, 2, "The top of mountain bikes. Equipped with RockShox front suspension, disc brakes, carbon fiber frame and Maxxis Minion tires.", 17.5m, 36, "M", "mountain6.png", "Scarlet Mountain", 5790.90m, 29.0m },
                    { 6, 2, "Silver mountain's advanced model. Equipped with RockShox front suspension, disc brakes, carbon fiber frame.", 17.5m, 21, "M", "mountain6.png", "Silver Mountain II", 2390.90m, 29.0m },
                    { 5, 4, "The classic bicycle for tours. Equipped with V-Brake brakes, aluminum frame, and rear and front load support.", 17.5m, 18, "M", "tour1.png", "Orange Tour", 380.90m, 27.5m },
                    { 4, 2, "The winter bike. Equipped with larger, non-slip special tires, disc brakes, aluminum frame.", 17.5m, 21, "M", "mountain4.png", "Snow Mountain", 1400.90m, 29.0m },
                    { 3, 3, "Advanced hybrid model. Equipped with front suspension, disc brakes, aluminum frame.", 17.5m, 21, "M", "mountain3.png", "Blue Sky", 1200.90m, 27.5m },
                    { 2, 3, "Versatile model, classified as hybrid. Equipped with front suspension, V-Brake, aluminum frame.", 17.5m, 10, "M", "mountain2.png", "Mechanical Orange", 450.90m, 27.5m }
                });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 18, 7, "Eletric tour bike. Equipped with V-Brake brakes and carbon fiber frame, powerfull front light, rear and front load support", 17.5m, "F", "tour6.png", "Orange Eletric", 3200.0m, 27.5m });

            migrationBuilder.InsertData(
                table: "Bikes",
                columns: new[] { "Id", "CategoryId", "Description", "FrameSize", "Gears", "GenderCode", "Image", "Model", "Price", "RimSize" },
                values: new object[] { 20, 4, "Special tour bike. Equipped with V-Brake brakes and aluminum frame, rear and front load support", 17.5m, 6, "F", "tour8.png", "Pink Tour", 400.0m, 27.5m });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Bikes",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Difficulties",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Cyclist");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Name",
                keyValue: "Monitor");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Code",
                keyValue: "F");

            migrationBuilder.DeleteData(
                table: "Genders",
                keyColumn: "Code",
                keyValue: "M");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Bikes");
        }
    }
}
