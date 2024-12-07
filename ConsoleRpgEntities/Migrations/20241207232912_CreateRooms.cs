using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsoleRpgEntities.Migrations
{
    public partial class CreateRooms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Players",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Monsters",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Room",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NorthId = table.Column<int>(type: "int", nullable: true),
                    SouthId = table.Column<int>(type: "int", nullable: true),
                    EastId = table.Column<int>(type: "int", nullable: true),
                    WestId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Room", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Room_Room_EastId",
                        column: x => x.EastId,
                        principalTable: "Room",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Room_Room_NorthId",
                        column: x => x.NorthId,
                        principalTable: "Room",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Room_Room_SouthId",
                        column: x => x.SouthId,
                        principalTable: "Room",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Room_Room_WestId",
                        column: x => x.WestId,
                        principalTable: "Room",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_RoomId",
                table: "Players",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Monsters_RoomId",
                table: "Monsters",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_EastId",
                table: "Room",
                column: "EastId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_NorthId",
                table: "Room",
                column: "NorthId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_SouthId",
                table: "Room",
                column: "SouthId");

            migrationBuilder.CreateIndex(
                name: "IX_Room_WestId",
                table: "Room",
                column: "WestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Monsters_Room_RoomId",
                table: "Monsters",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Room_RoomId",
                table: "Players",
                column: "RoomId",
                principalTable: "Room",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Monsters_Room_RoomId",
                table: "Monsters");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Room_RoomId",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Room");

            migrationBuilder.DropIndex(
                name: "IX_Players_RoomId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Monsters_RoomId",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Monsters");
        }
    }
}
