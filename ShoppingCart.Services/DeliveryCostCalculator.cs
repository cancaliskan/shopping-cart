using System;

using ShoppingCart.Service.Contracts;

namespace ShoppingCart.Service
{
    public class DeliveryCostCalculator : IDeliveryCostCalculator
    {
        public double CostPerDelivery { get; }
        public double CostPerProduct { get; }
        public double FixedCost { get; }

        public DeliveryCostCalculator(double costPerDelivery, double costPerProduct, double fixedCost = 2.99)
        {
            CostPerDelivery = costPerDelivery;
            CostPerProduct = costPerProduct;
            FixedCost = fixedCost;
        }

        public double CalculateFor(IShoppingCart cart)
        {
            if (cart == null)
            {
                throw new ArgumentNullException($"{nameof(cart)} is null");
            }

            var numberOfDeliveries = cart.GetNumberOfDeliveries();
            var numberOfProducts = cart.GetNumberOfProducts();
            var result = (CostPerDelivery * numberOfDeliveries) + (CostPerProduct * numberOfProducts) + FixedCost;

            return result;
        }
    }
}