using System;

using Moq;
using NUnit.Framework;

using ShoppingCart.Service.Contracts;

namespace ShoppingCart.Service.Test
{
    [TestFixture]
    public class DeliveryCostCalculatorTests
    {
        private DeliveryCostCalculator _deliveryCostCalculator;
        private Mock<IShoppingCart> _cart;

        [SetUp]
        public void Setup()
        {
            _cart = new Mock<IShoppingCart>();
        }

        [TestCase(3,7)]
        [TestCase(1.5, 10)]
        [TestCase(13.7, 7.9)]
        public void CalculateFor_NullCart_ReturnsException(double cosPerDelivery, double costPerProduct)
        {
            // arrange
            _deliveryCostCalculator = new DeliveryCostCalculator(cosPerDelivery, costPerProduct);

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => _deliveryCostCalculator.CalculateFor(null));
        }

        [TestCase(2,10)]
        [TestCase(2,10, 5)]
        [TestCase(7,4, 1.6)]
        public void CalculateFor_CartWithNoProduct_ReturnsFixedCost(double cosPerDelivery, double costPerProduct, double fixedCost = 2.99)
        {
            // arrange
            _deliveryCostCalculator = new DeliveryCostCalculator(cosPerDelivery, costPerProduct, fixedCost);
            _cart.Setup(m => m.GetNumberOfDeliveries()).Returns(0);
            _cart.Setup(m => m.GetNumberOfProducts()).Returns(0);

            // act
            var result = _deliveryCostCalculator.CalculateFor(_cart.Object);

            // assert
            Assert.That(result == fixedCost);
        }

        [TestCase(2, 10)]
        [TestCase(2, 10, 5)]
        [TestCase(7, 4, 1.6)]
        public void CalculateFor_CartWithOneDeliveryOneProduct_ReturnsValidCalculation(double cosPerDelivery, double costPerProduct, double fixedCost = 2.99)
        {
            // arrange
            _deliveryCostCalculator = new DeliveryCostCalculator(cosPerDelivery, costPerProduct, fixedCost);
            _cart.Setup(m => m.GetNumberOfDeliveries()).Returns(1);
            _cart.Setup(m => m.GetNumberOfProducts()).Returns(1);
            var expected = (cosPerDelivery * 1) + (costPerProduct * 1) + fixedCost;

            // act
            var result = _deliveryCostCalculator.CalculateFor(_cart.Object);

            // assert
            Assert.That(result == expected);
        }
    }
}