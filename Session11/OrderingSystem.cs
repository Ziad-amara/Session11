using System;
using System.Collections.Generic;
using System.Text;

namespace Session11
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } =string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public override string ToString()
        {
            return $"Order #{Id} | Customer: {CustomerName} | Price: {Price} x {Quantity}";
        }
    }

    public delegate decimal PriceCalculator(Order order);

    // Two named methods matching the PriceCalculator signature.
    public static class PricingMethods
    {
        public static decimal CalculateTotal(Order order)
        {
            return order.Price * order.Quantity;
        }

        public static decimal CalculateTotalWithDiscount(Order order)
        {
            decimal total = order.Price * order.Quantity;
            decimal discount = total * 0.10m; // flat 10% discount
            return total - discount;
        }
    }

    public class OrderService
    {
        // External code can subscribe/unsubscribe, but only OrderService can raise this event.
        public event Action<Order>? OrderProcessed;

        public decimal CalculateOrderPrice(Order order, PriceCalculator calculator) => calculator(order);

        public decimal CalculateOrderPrice(Order order, Func<Order, decimal> calculator) => calculator(order);

        public bool ValidateOrder(Order order, Predicate<Order> validationRule) => validationRule(order);

        public void ProcessOrder(Order order, Action<Order> action)
        {
            Console.WriteLine($"Order {order.Id} is being processed.");
            action(order);
        }

        public void ProcessOrder(Order order)
        {
            Console.WriteLine($"Order {order.Id} completed.");
            OrderProcessed?.Invoke(order);
        }
    }
}
