using ShoppingCart.Service;
using ShoppingCart.Service.Helpers.Enums;
using ShoppingCart.Service.Models;

namespace ShoppingCart.Application
{
    class Program
    {
        static void Main(string[] args)
        {
            var smartPhone = new Category("Smart Phone");
            var samsung = new Category("Samsung",smartPhone);
            var iphone = new Category("Iphone",smartPhone);

            var laptop = new Category("Laptop");
            var apple = new Category("Apple",laptop);
            var msi = new Category("MSI",laptop);

            var j7pro = new Product("J7 Pro", 1500, samsung);
            var iphone7 = new Product("Iphone 7", 3000, iphone);
            var pe60 = new Product("PE60", 4500, msi);

            var deliveryCostCalculator = new DeliveryCostCalculator(1.5, 10);
            var shoppingCart = new Service.ShoppingCart(deliveryCostCalculator);
            shoppingCart.AddProduct(j7pro, 10);
            shoppingCart.AddProduct(iphone7, 5);
            shoppingCart.AddProduct(pe60, 3);

            var campaignForPhone = new Campaign(smartPhone, 5, 10, DiscountType.Rate);
            var campaignForLaptop = new Campaign(laptop, 150, 30, DiscountType.Rate);
            shoppingCart.ApplyCampaigns(campaignForPhone, campaignForLaptop);

            var coupon = new Coupon(1, 10, DiscountType.Amount);
            shoppingCart.ApplyCoupon(coupon);

            System.Console.WriteLine(shoppingCart.Print());

            System.Console.WriteLine($"{"*** After 3 'iPhone 7' were removed ***",60} \n\n");

            shoppingCart.RemoveProduct(iphone7, 3);
            System.Console.WriteLine(shoppingCart.Print());

            System.Console.ReadKey();
        }
    }
}
