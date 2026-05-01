using Microsoft.EntityFrameworkCore;

namespace ECommerce527.Repositories
{
    public interface IProductColorRepository :IRepository<ProductColor>
    {
        public void RemoveRange(IEnumerable<ProductColor> productColors);
    }
}
