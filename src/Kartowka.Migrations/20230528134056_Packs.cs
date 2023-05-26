#nullable disable

using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Kartowka.Migrations
{
    /// <inheritdoc />
    public partial class Packs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    username = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email_address = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_online_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    password_hash = table.Column<byte[]>(type: "bytea", nullable: false),
                    password_salt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "packs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    author_id = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_packs", x => x.id);
                    table.ForeignKey(
                        name: "fk_packs_users_user_id",
                        column: x => x.author_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rounds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    pack_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rounds", x => x.id);
                    table.ForeignKey(
                        name: "fk_rounds_packs_pack_id",
                        column: x => x.pack_id,
                        principalTable: "packs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions_categories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    order = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    round_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_categories_rounds_round_id",
                        column: x => x.round_id,
                        principalTable: "rounds",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    content = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    score = table.Column<int>(type: "integer", nullable: false),
                    content_type = table.Column<int>(type: "integer", nullable: false),
                    question_type = table.Column<int>(type: "integer", nullable: false),
                    question_category_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_questions_categories_questions_category_id",
                        column: x => x.question_category_id,
                        principalTable: "questions_categories",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_packs_author_id",
                table: "packs",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "ix_packs_name",
                table: "packs",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_questions_question_category_id",
                table: "questions",
                column: "question_category_id");

            migrationBuilder.CreateIndex(
                name: "ix_questions_categories_round_id",
                table: "questions_categories",
                column: "round_id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_id",
                table: "rounds",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_rounds_pack_id_order",
                table: "rounds",
                columns: new[] { "pack_id", "order" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "questions_categories");

            migrationBuilder.DropTable(
                name: "rounds");

            migrationBuilder.DropTable(
                name: "packs");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
