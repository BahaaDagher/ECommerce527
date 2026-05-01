using ECommerce527.Data;

namespace ECommerce527.Repositories
{
    public class ProductColorRepository : Repository<ProductColor> , IProductColorRepository
    {
        public ProductColorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void RemoveRange(IEnumerable<ProductColor> productColors)
        {
            _context.ProductColors.RemoveRange(productColors);
        }
    }
}
