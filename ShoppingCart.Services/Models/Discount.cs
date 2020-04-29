using System;

using ShoppingCart.Service.Helpers.Enums;

namespace ShoppingCart.Service.Models
{
    public abstract class Discount
    {
        public int MinimumAmount { get; }
        public double DiscountAmount { get; }
        public DiscountType DiscountType { get; }

        protected Discount(int minimumAmount, double discountAmount, DiscountType discountType)
        {
            if (minimumAmount < 0)
            {
                throw new ArgumentException("Minimum amount must be positive or zero");
            }

            if (discountAmount < 0)
            {
                throw new ArgumentException("Discount amount must be positive or zero");
            }

            MinimumAmount = minimumAmount;
            DiscountAmount = discountAmount;
            DiscountType = discountType;
        }
    }
}