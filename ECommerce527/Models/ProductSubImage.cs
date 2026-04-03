using Microsoft.EntityFrameworkCore;

namespace ECommerce527.Models
{
    [PrimaryKey(nameof(ProductId) , nameof(Img))]
    public class ProductSubImage
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string Img { get; set; }
    }
}
