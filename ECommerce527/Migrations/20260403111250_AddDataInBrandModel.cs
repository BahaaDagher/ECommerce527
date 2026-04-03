using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce527.Migrations
{
    /// <inheritdoc />
    public partial class AddDataInBrandModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into Brands (name, description, status) values ('Apple', 'Nulla neque libero, convallis eget, eleifend luctus, ultricies eu, nibh.', 1);insert into Brands (name, description, status) values ('Dell', 'Fusce posuere felis sed lacus.', 0);insert into Brands (name, description, status) values ('Oppo', 'Cras pellentesque volutpat dui.', 1);insert into Brands (name, description, status) values ('Samsung', 'Proin at turpis a pede posuere nonummy.', 0);insert into Brands (name, description, status) values ('hp', 'Vestibulum rutrum rutrum neque.', 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from Brands");
        }
    }
}
