namespace ECommerce527.Repositories
{
    public interface IProductSubImageRepository : IRepository<ProductSubImage>
    {
        public void RemoveRange(IEnumerable<ProductSubImage> productSubImages); 
    }
}
