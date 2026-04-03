using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce527.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInCAtegoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Categories (name, description, status) values ('Mobiles', 'Vestibulum ante ipsum primis in faucibus orci luctus et ultrices.', 1);insert into Categories (name, description, status) values ('Labtops', 'Vestibulum ante ipsum primis in faucibus orci luctus et ', 0);insert into Categories (name, description, status) values ('Tablets', 'Proin interdum mauris non ligula pellentesque ultrices.', 1);insert into Categories (name, description, status) values ('Accessorise', 'In sagittis dui vel nisl.', 0);insert into Categories (name, description, status) values ('Consoles', 'Vivamus vel nulla eget eros elementum pellentesque.', 1);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from Categories");
        }
    }
}
