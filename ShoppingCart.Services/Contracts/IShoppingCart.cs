using ShoppingCart.Service.Models;

namespace ShoppingCart.Service.Contracts
{
    public interface IShoppingCart
    {
        double GetTotalPrice();
        double GetTotalPriceAfterDiscounts();
        double GetCouponDiscount();
        double GetCampaignDiscount();
        double GetDeliveryCost();

        void AddProduct(Product product, int amount);
        void RemoveProduct(Product product, int amount);

        void ApplyCampaigns(params Campaign[] campaigns);
        void ApplyCoupon(Coupon coupon);

        int GetNumberOfDeliveries();
        int GetNumberOfProducts();

        string Print();
    }
}