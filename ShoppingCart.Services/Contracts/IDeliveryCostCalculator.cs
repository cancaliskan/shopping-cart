namespace ShoppingCart.Service.Contracts
{
    public interface IDeliveryCostCalculator
    {
        double CalculateFor(IShoppingCart cart);
    }
}