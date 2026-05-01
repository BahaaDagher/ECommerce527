using ECommerce527.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce527.Repositories
{
    public class ProductSubImageRepository : Repository<ProductSubImage> , IProductSubImageRepository
    {
        public ProductSubImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductSubImage> productSubImages)
        {
            _context.ProductSubImages.RemoveRange(productSubImages); 
        }
    }
}
