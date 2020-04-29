using ShoppingCart.Service.Helpers.Enums;

namespace ShoppingCart.Service.Models
{
    public class Campaign : Discount
    {
        public Category Category { get; set; }

        public Campaign(Category category, int minimumAmount, double discountAmount, DiscountType discountType)
                        : base(minimumAmount, discountAmount, discountType)
        {
            Category = category;
        }
    }
}