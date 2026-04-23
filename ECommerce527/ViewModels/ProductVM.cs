namespace ECommerce527.ViewModels
{
    public class ProductVM
    {
       public List<Category> Categories { get; set; }
       public List<Brand> Brands { get; set; }
       public List<ProductSubImage>? ProductSubImages { get; set; }
       public List<ProductColor>? ProductColors { get; set; }
       public Product? Product  { get; set; }
        
    }
}
