using System;

using Moq;
using NUnit.Framework;

using ShoppingCart.Service.Contracts;
using ShoppingCart.Service.Helpers.Enums;
using ShoppingCart.Service.Models;

namespace ShoppingCart.Service.Test
{
    [TestFixture]
    public class ShoppingCartTests
    {
        private Mock<IDeliveryCostCalculator> _calculator;
        private ShoppingCart _shoppingCart;

        [SetUp]
        public void Setup()
        {
            _calculator = new Mock<IDeliveryCostCalculator>();
            _shoppingCart = new ShoppingCart(_calculator.Object);
        }

        #region AddProduct

        [Test]
        public void AddProduct_ValidProductValidQuantity_ProductShouldAdd()
        {
            // arrange
            var product = new Product("Apple", 100.0, new Category("Fruit"));

            // act
            _shoppingCart.AddProduct(product, 5);

            // assert
            Assert.AreEqual(1, _shoppingCart.Products.Count);
            Assert.IsTrue(_shoppingCart.Products.ContainsKey(product));
            Assert.That(_shoppingCart.Products[product] == 5);
        }

        [Test]
        public void AddProduct_AddSameProductInstanceWithDifferentQuantity_ProductShouldAdd()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);

            // act
            _shoppingCart.AddProduct(product, 3);
            _shoppingCart.AddProduct(product, 2);

            // assert
            Assert.AreEqual(1, _shoppingCart.Products.Count);
            Assert.IsTrue(_shoppingCart.Products.ContainsKey(product));
            Assert.That(_shoppingCart.Products[product] == 5);
        }

        [Test]
        public void AddProduct_AddSameProductDifferentInstanceWithDifferentQuantity_ProductShouldAdd()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Pro", 1599.99, category);

            // act
            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            // assert
            Assert.AreEqual(2, _shoppingCart.Products.Count);
            Assert.IsTrue(_shoppingCart.Products.ContainsKey(product1));
            Assert.IsTrue(_shoppingCart.Products.ContainsKey(product2));
            Assert.That(_shoppingCart.Products[product1] == 3);
            Assert.That(_shoppingCart.Products[product2] == 2);
        }

        [Test]
        public void AddProduct_ProductIsNull_ProductShouldNotAdd()
        {
            // arrange
            Product product = null;

            // act
            _shoppingCart.AddProduct(product, 5);

            // assert
            Assert.AreEqual(0, _shoppingCart.Products.Count);
        }

        [Test]
        public void AddProduct_ProductQuantityIsZero_ProductShouldNotAdd()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);

            // act
            _shoppingCart.AddProduct(product, 0);

            // assert
            Assert.AreEqual(0, _shoppingCart.Products.Count);
            Assert.IsFalse(_shoppingCart.Products.ContainsKey(product));
        }

        [Test]
        public void AddProduct_ProductQuantityIsNegative_ProductShouldNotAdd()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);

            // act
            _shoppingCart.AddProduct(product, -1);

            // assert
            Assert.AreEqual(0, _shoppingCart.Products.Count);
            Assert.IsFalse(_shoppingCart.Products.ContainsKey(product));
        }

        #endregion

        #region GetNumberOfProducts

        [Test]
        public void GetNumberOfProduct_EmptyList_ReturnsZero()
        {
            // arrange

            // act
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();

            // assert
            Assert.AreEqual(0, numberOfProducts);
        }

        [Test]
        public void GetNumberOfProduct_AddValidProduct_ReturnsOne()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product, 5);

            // act
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();

            // assert
            Assert.AreEqual(1, numberOfProducts);
        }

        [Test]
        public void GetNumberOfProduct_AddValidTwoProduct_ReturnsTwo()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);
            _shoppingCart.AddProduct(product1, 5);
            _shoppingCart.AddProduct(product2, 5);

            // act
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();

            // assert
            Assert.AreEqual(2, numberOfProducts);
        }

        #endregion

        #region GetDeliveryCost

        [Test]
        public void GetDeliveryCost_TestWithMock_ReturnsMockValue()
        {
            // arrange
            _calculator.Setup(m => m.CalculateFor(_shoppingCart)).Returns(5);

            // act
            var deliveryCost = _shoppingCart.GetDeliveryCost();

            // assert
            Assert.That(deliveryCost == 5);
        }

        #endregion

        #region GetNumberOfDeliveries

        [Test]
        public void GetNumberOfDeliveries_EmptyCart_ReturnsZero()
        {
            // arrange

            // act
            var numberOfDeliveries = _shoppingCart.GetNumberOfDeliveries();

            // assert
            Assert.That(numberOfDeliveries == 0);
        }

        [Test]
        public void GetNumberOfDeliveries_OneProduct_ReturnsOne()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product, 5);

            // act
            var numberOfDeliveries = _shoppingCart.GetNumberOfDeliveries();

            // assert
            Assert.That(numberOfDeliveries == 1);
        }

        [Test]
        public void GetNumberOfDeliveries_TwoDifferentCategoryProduct_ReturnsTwo()
        {
            // arrange
            var smartPhoneCategory = new Category("Smart Phone");
            var laptopCategory = new Category("Laptop");

            var product1 = new Product("J7 Pro", 1599.99, smartPhoneCategory);
            var product2 = new Product("PE60", 5399.99, laptopCategory);

            _shoppingCart.AddProduct(product1, 5);
            _shoppingCart.AddProduct(product2, 3);

            // act
            var numberOfDeliveries = _shoppingCart.GetNumberOfDeliveries();

            // assert
            Assert.That(numberOfDeliveries == 2);
        }

        [Test]
        public void GetNumberOfDeliveries_TwoSameCategoryProduct_ReturnsOne()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);
            _shoppingCart.AddProduct(product1, 5);
            _shoppingCart.AddProduct(product2, 3);

            // act
            var numberOfDeliveries = _shoppingCart.GetNumberOfDeliveries();

            // assert
            Assert.That(numberOfDeliveries == 1);
        }

        #endregion

        #region RemoveItem

        [Test]
        public void RemoveItem_EmptyList_ReturnsException()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            // act

            // assert
            Assert.Throws<Exception>(() => _shoppingCart.RemoveProduct(product1, 5));
        }

        [Test]
        public void RemoveItem_ProductAllQuantity_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product1, 5);

            // act
            _shoppingCart.RemoveProduct(product1, 5);

            // assert
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();
            Assert.That(numberOfProducts == 0);
        }

        [Test]
        public void RemoveItem_ProductOneQuantity_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product1, 5);

            // act
            _shoppingCart.RemoveProduct(product1, 1);

            // assert
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();
            var shoppingCartProduct = _shoppingCart.Products[product1];
            Assert.That(numberOfProducts == 1);
            Assert.That(shoppingCartProduct == 4);
        }

        [Test]
        public void RemoveItem_ProductMoreQuantityThanProductQuantity_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product1, 5);

            // act
            _shoppingCart.RemoveProduct(product1, 6);

            // assert
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();
            Assert.That(numberOfProducts == 0);
        }

        [Test]
        public void RemoveItem_AddTwoProductRemoveOneProduct_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1299.99, category);

            _shoppingCart.AddProduct(product1, 5);
            _shoppingCart.AddProduct(product1, 6);

            // act
            _shoppingCart.RemoveProduct(product1, 5);

            // assert
            var numberOfProducts = _shoppingCart.GetNumberOfProducts();
            Assert.That(numberOfProducts == 1);
        }

        #endregion

        #region GetTotalAmount

        [Test]
        public void GetTotalPrice_EmptyList_ReturnsZero()
        {
            // arrange

            // act
            var totalPrice = _shoppingCart.GetTotalPrice();

            // assert
            Assert.That(totalPrice == 0);
        }

        [Test]
        public void GetTotalPrice_OneProduct_Successful()
        {
            // arrange
            const double price = 1599.99;
            const int amount = 5;
            const double expectedResult = amount * price;

            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", price, category);

            _shoppingCart.AddProduct(product1, amount);

            // act
            var totalPrice = _shoppingCart.GetTotalPrice();

            // assert
            const int tolerance = 2;
            Assert.That(Math.Abs(totalPrice - expectedResult) < tolerance);
        }

        [Test]
        public void GetTotalPrice_TwoProduct_Successful()
        {
            // arrange
            const double price1 = 1599.99;
            const double price2 = 1299.99;
            const int amountForProduct1 = 5;
            const int amountForProduct2 = 1;
            const double expectedResult = (price1 * amountForProduct1) + (price2 + amountForProduct2);

            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", price1, category);
            var product2 = new Product("J7 Prime", price2, category);

            _shoppingCart.AddProduct(product1, amountForProduct1);
            _shoppingCart.AddProduct(product2, amountForProduct2);

            // act
            var result = _shoppingCart.GetTotalPrice();

            // assert
            const int tolerance = 2;
            Assert.That(Math.Abs(result - expectedResult) < tolerance);
        }

        #endregion

        #region GetCampaignDiscount

        [Test]
        public void GetCampaignDiscount_NoCampaign_ReturnsZero()
        {
            // arrange

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignOneProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product, 3);

            var campaign = new Campaign(category, 2, 5, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 5);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignForParentOneProductOnSubCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var laptopCategory = new Category("Laptop");
            var msiLaptopCategory = new Category("MSI", laptopCategory);

            var product = new Product("PE60", 4999, msiLaptopCategory);
            _shoppingCart.AddProduct(product, 3);

            var campaign = new Campaign(laptopCategory, 2, 5, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 5);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignOneProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // assert
            var msiLaptopCategory = new Category("MSI");
            var product = new Product("PE60", 4999, msiLaptopCategory);
            _shoppingCart.AddProduct(product, 3);

            var campaign = new Campaign(msiLaptopCategory, 5, 5, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignTwoProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 5);
            _shoppingCart.AddProduct(product2, 5);


            var campaign = new Campaign(category, 5, 10, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 10);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignTwoProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var campaign = new Campaign(category, 6, 10, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignWithRateOneProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 3);
            var expected = _shoppingCart.GetTotalPrice() * 0.05;

            var campaign = new Campaign(category, 2, 5, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == expected);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignWithRateOneProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 3);

            var campaign = new Campaign(category, 5, 5, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignWithTwoProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var expected = _shoppingCart.GetTotalPrice() * 0.1;

            var campaign = new Campaign(category, 5, 10, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == expected);
        }

        [Test]
        public void GetCampaignDiscount_OneCampaignWithRateTwoProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var campaign = new Campaign(category, 6, 10, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_TwoCampaignsWithRateOneProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 3);

            var campaign1 = new Campaign(category, 6, 10, DiscountType.Rate);
            var campaign2 = new Campaign(category, 4, 5, DiscountType.Rate);

            _shoppingCart.ApplyCampaigns(campaign1, campaign2);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_TwoCampaignsWithRateOneProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 3);

            var expected = _shoppingCart.GetTotalPrice() * 0.05;

            var campaign1 = new Campaign(category, 6, 10, DiscountType.Rate);
            var campaign2 = new Campaign(category, 3, 5, DiscountType.Rate);

            _shoppingCart.ApplyCampaigns(campaign1, campaign2);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == expected);
        }

        [Test]
        public void GetCampaignDiscount_TwoCampaignsWithRateTwoProductOnCategoryLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var campaign1 = new Campaign(category, 7, 10, DiscountType.Rate);
            var campaign2 = new Campaign(category, 6, 5, DiscountType.Rate);

            _shoppingCart.ApplyCampaigns(campaign1, campaign2);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == 0);
        }

        [Test]
        public void GetCampaignDiscount_TwoCampaignsWithRateTwoProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var expected = (_shoppingCart.GetTotalPrice() * 0.1);
            expected = expected + (_shoppingCart.GetTotalPrice() - expected) * 0.05;

            var campaign1 = new Campaign(category, 5, 10, DiscountType.Rate);
            var campaign2 = new Campaign(category, 3, 5, DiscountType.Rate);

            _shoppingCart.ApplyCampaigns(campaign1, campaign2);

            // act
            var campaignDiscount = _shoppingCart.GetCampaignDiscount();

            // assert
            Assert.That(campaignDiscount == expected);
        }

        #endregion

        #region GetCouponDiscount

        [Test]
        public void GetCouponDiscount_NoCoupon_ReturnsZero()
        {
            // arrange

            //act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == 0);
        }

        [Test]
        public void GetCouponDiscount_CouponOneProductGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product, 3);

            var coupon = new Coupon( 2, 5, DiscountType.Amount);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == 5);
        }

        [Test]
        public void GetCouponDiscount_CouponOneProductLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product = new Product("J7 Pro", 1599.99, category);
            _shoppingCart.AddProduct(product, 3);

            var coupon = new Coupon( 50000, 100, DiscountType.Amount);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // arrange
            Assert.That(couponDiscount == 0);
        }

        [Test]
        public void GetCouponDiscount_CouponTwoProductGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 4);
            _shoppingCart.AddProduct(product2, 2);

            var coupon = new Coupon( 500, 100, DiscountType.Amount);
            _shoppingCart.ApplyCoupon(coupon);

            // act 
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == 100);
        }

        [Test]
        public void GetCouponDiscount_CouponTwoProductLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 3);
            _shoppingCart.AddProduct(product2, 2);

            var coupon = new Coupon( 50000, 100, DiscountType.Amount);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert 
            Assert.That(couponDiscount == 0);
        }

        [Test]
        public void GetCouponDiscount_CampaignWithRateOneProductOnCategoryGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 1);

            var expected = _shoppingCart.GetTotalPrice() * 0.5;

            var coupon = new Coupon( 200, 50, DiscountType.Rate);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == expected);
        }

        [Test]
        public void GetCouponDiscount_CouponWithRateOneProductLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);

            var coupon = new Coupon( 50000, 50, DiscountType.Rate);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert 
            Assert.That(couponDiscount == 0);
        }

        [Test]
        public void GetCouponDiscount_CouponWithTwoProductGreaterThanMinimumAmount_Successful()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 4);
            _shoppingCart.AddProduct(product2, 2);

            var expected = _shoppingCart.GetTotalPrice() * 0.1;

            var coupon = new Coupon( 500, 10, DiscountType.Rate);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == expected);
        }

        [Test]
        public void GetCouponDiscount_CouponWithRateTwoProductLessThanMinimumAmount_ReturnsZero()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);
            var product2 = new Product("J7 Prime", 1399.99, category);

            _shoppingCart.AddProduct(product1, 4);
            _shoppingCart.AddProduct(product2, 2);

            var coupon = new Coupon( 50000, 10, DiscountType.Rate);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var couponDiscount = _shoppingCart.GetCouponDiscount();

            // assert
            Assert.That(couponDiscount == 0);
        }

        #endregion

        #region GetTotalAmountAfterDiscounts

        [Test]
        public void GetTotalPriceAfterDiscounts_NoProduct_ReturnsZero()
        {
            // arrange

            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // assert
            Assert.That(totalPriceAfterDiscounts == 0);
        }

        [Test]
        public void GetTotalPriceAfterDiscounts_OneProductNoDiscount_ReturnsTotalAmount()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);
            
            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // arrange
            var totalPrice = _shoppingCart.GetTotalPrice();
            Assert.That(totalPrice == totalPriceAfterDiscounts);
        }

        [Test]
        public void GetTotalPriceAfterDiscounts_OneProductOneCampaignNoCoupon_ReturnsTotalAmountMinusCampaignDiscount()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);

            var campaign = new Campaign(category,  4, 5, DiscountType.Amount);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // assert
            var campaignDiscount = (_shoppingCart.GetTotalPrice() - _shoppingCart.GetCampaignDiscount());
            Assert.That(campaignDiscount == totalPriceAfterDiscounts);
        }

        [Test]
        public void GetTotalPriceAfterDiscounts_OneProductOneCampaignWithRateNoCoupon_ReturnsTotalAmountMinusCampaignDiscount()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);

            var campaign = new Campaign(category,  4, 5, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // arrange
            var campaignDiscount = (_shoppingCart.GetTotalPrice() - _shoppingCart.GetCampaignDiscount());
            Assert.That(campaignDiscount == totalPriceAfterDiscounts);
        }

        [Test]
        public void GetTotalPriceAfterDiscounts_OneProductOneCampaignWithCoupon_ReturnsTotalAmountMinusCampaignDiscountAndCouponDiscount()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);

            var campaign = new Campaign(category, 4, 5, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            var coupon = new Coupon( 50, 5, DiscountType.Amount);
            _shoppingCart.ApplyCoupon(coupon);

            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // assert
            var discount= (_shoppingCart.GetTotalPrice() - (_shoppingCart.GetCampaignDiscount() + _shoppingCart.GetCouponDiscount()));
            Assert.That(discount == totalPriceAfterDiscounts);
        }

        [Test]
        public void GetTotalPriceAfterDiscounts_OneProductOneCampaignWithCouponWithRate_ReturnsTotalAmountMinusCampaignDiscountAndCouponDiscount()
        {
            // arrange
            var category = new Category("Smart Phone");
            var product1 = new Product("J7 Pro", 1599.99, category);

            _shoppingCart.AddProduct(product1, 4);

            var campaign = new Campaign(category, 4, 5, DiscountType.Rate);
            _shoppingCart.ApplyCampaigns(campaign);

            var coupon = new Coupon( 50, 5, DiscountType.Rate);
            _shoppingCart.ApplyCoupon(coupon);

            var totalAmountAfterCamping = _shoppingCart.GetTotalPrice() - _shoppingCart.GetCampaignDiscount();
            var expected = totalAmountAfterCamping - (totalAmountAfterCamping * 0.05);

            // act
            var totalPriceAfterDiscounts = _shoppingCart.GetTotalPriceAfterDiscounts();

            // assert
            Assert.That(expected == totalPriceAfterDiscounts);
        }
        
        #endregion
    }
}