using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

using ShoppingCart.Service.Contracts;
using ShoppingCart.Service.Helpers.Enums;
using ShoppingCart.Service.Models;

namespace ShoppingCart.Service
{
    public class ShoppingCart : IShoppingCart
    {
        public Dictionary<Product, int> Products { get; set; }
        public Coupon Coupon { get; set; }
        public List<Campaign> Campaigns { get; set; }
        private IDeliveryCostCalculator DeliveryCostCalculator { get; }

        public ShoppingCart(IDeliveryCostCalculator deliveryCostCalculator)
        {
            Products = new Dictionary<Product, int>();
            Campaigns = new List<Campaign>();
            DeliveryCostCalculator = deliveryCostCalculator;
        }

        public double GetTotalPrice()
        {
            return Products.Sum(e => e.Key.Price * e.Value);
        }

        public double GetTotalPriceAfterDiscounts()
        {
            var amount = GetTotalPrice();
            amount = amount - ApplyCampaign(amount);
            amount = amount - ApplyCoupon(amount);
            return amount;
        }

        public double GetCouponDiscount()
        {
            var totalAmount = GetTotalPrice();
            return ApplyCoupon(totalAmount);
        }

        public double GetCampaignDiscount()
        {
            var totalAmount = GetTotalPrice();
            return ApplyCampaign(totalAmount);
        }

        public double GetDeliveryCost()
        {
            return DeliveryCostCalculator.CalculateFor(this);
        }

        public void AddProduct(Product product, int amount)
        {
            if (product == null || amount <= 0)
            {
                return;
            }

            // If product is exist, increase amount of product
            if (Products.TryGetValue(product, out var productAmount))
            {
                Products[product] = productAmount + amount;
                return;
            }

            Products.Add(product, amount);
        }

        public void RemoveProduct(Product product, int amount)
        {
            var productForRemove = Products.Keys.FirstOrDefault(p => p.Title == product.Title);
            if (productForRemove == null)
            {
                throw new Exception($"There is no {product.Title} in products!");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Amount must be greater than zero!");
            }

            var numberOfProduct = Products[productForRemove];
            // Remove all products
            if (amount >= numberOfProduct)
            {
                Products.Remove(productForRemove);
                return;
            }

            // Decrease number of product
            Products[productForRemove] = numberOfProduct - amount;
        }

        public void ApplyCampaigns(params Campaign[] campaigns)
        {
            Campaigns.AddRange(campaigns);
        }

        public void ApplyCoupon(Coupon coupon)
        {
            Coupon = coupon;
        }

        public int GetNumberOfDeliveries()
        {
            var numberOfProductTypes = Products.GroupBy(e => e.Key.Category.Title).Count();

            return numberOfProductTypes;
        }

        public int GetNumberOfProducts()
        {
            var numberOfProduct = Products.Count;

            return numberOfProduct;
        }

        public string Print()
        {
            var builder = new StringBuilder();
            var productsGroupByCategory = Products.GroupBy(p => p.Key.Category).ToDictionary(g => g.Key, g => g.ToList());

            builder.AppendLine($"{"Category Name",15} {"Product Name",15} {"Quantity",15} {"Unit Price",18} {"Total Price",18}");
            foreach (var (category, products) in productsGroupByCategory)
            {
                var categoryTitle = GetRootCategory(category);
                foreach (var (product, quantity) in products)
                {
                    builder.AppendLine($"{categoryTitle,15} {product.Title,15} {quantity,15} {product.Price,15} TL {GetProductPrice(product),15} TL\t");
                }
            }

            builder.AppendLine($"\nTotal Price: {GetTotalPrice()} TL" +
                               $"\nTotal Price After Discounts: {GetTotalPriceAfterDiscounts()} TL" +
                               $"\nTotal Discount: {GetTotalDiscount()} TL" +
                               $"\nDelivery Cost: {GetDeliveryCost()} TL");

            return builder.ToString();
        }

        private double GetTotalDiscount()
        {
            return GetTotalPrice() - GetTotalPriceAfterDiscounts();
        }

        private Dictionary<Product, int> GetProductsByCategory(Category category)
        {
            return Products.Where(e => e.Key.Category == category || HasSubCategory(category, e.Key.Category)).ToDictionary(e => e.Key, e => e.Value);
        }

        private static bool HasSubCategory(Category parent, Category sub)
        {
            var temp = sub.ParentCategory;
            while (temp != null)
            {
                if (temp == parent)
                {
                    return true;
                }
                temp = temp.ParentCategory;
            }

            return false;
        }

        private static string GetRootCategory(Category category)
        {
            var temp = category.ParentCategory;
            while (temp != null)
            {
                category = temp;
                temp = temp.ParentCategory;
            }

            return category.Title;
        }

        private double ApplyCoupon(double totalAmount)
        {
            double discountAmount = 0;

            if (Coupon == null || !(totalAmount >= Coupon.MinimumAmount))
            {
                return discountAmount;
            }

            switch (Coupon.DiscountType)
            {
                case DiscountType.Rate:
                    discountAmount = totalAmount * (Coupon.DiscountAmount / 100);
                    break;
                case DiscountType.Amount:
                    discountAmount = Coupon.DiscountAmount;
                    break;
            }

            return discountAmount;
        }

        private double ApplyCampaign(double totalAmount)
        {
            double discountAmount = 0;

            foreach (var campaign in Campaigns)
            {
                var productsByCategory = GetProductsByCategory(campaign.Category);
                if (productsByCategory.Values.Sum() >= campaign.MinimumAmount)
                {
                    switch (campaign.DiscountType)
                    {
                        case DiscountType.Rate:
                            {
                                if (discountAmount != 0)
                                {
                                    totalAmount = totalAmount - discountAmount;
                                }

                                discountAmount = discountAmount + (totalAmount * (campaign.DiscountAmount / 100));
                                break;
                            }
                        case DiscountType.Amount:
                            {
                                discountAmount = campaign.DiscountAmount;
                                break;
                            }
                    }
                }
            }

            return discountAmount;
        }

        private double GetProductPrice(Product product)
        {
            if (Products.TryGetValue(product, out var quantity))
            {
                return product.Price * quantity;
            }

            throw new KeyNotFoundException(nameof(product));
        }
    }
}