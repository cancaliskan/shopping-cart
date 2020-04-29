using ShoppingCart.Service.Helpers.Enums;

namespace ShoppingCart.Service.Models
{
    public class Coupon : Discount
    {
        public Coupon(int minimumAmount, double discountAmount, DiscountType discountType)
                      : base(minimumAmount, discountAmount, discountType)
        {
        }
    }
}