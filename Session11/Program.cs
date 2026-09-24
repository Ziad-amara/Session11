using System.Globalization;

namespace Session11
{
    public static class Program
    {
        public static void Main()
        {
            DemonstrateBookDelegates();
            DemonstrateOrderProcessing();
        }

        #region Section 01 Setup
        private static void DemonstrateBookDelegates()
        {
            Console.WriteLine("=== Section 01: Book delegates ===");
            List<Book> books =
            [
                new("978-0132350884", "Clean Code", ["Robert C. Martin"], new DateTime(2008, 8, 1), 42.50m),
                new("978-0201633610", "Design Patterns", ["Erich Gamma", "Richard Helm", "Ralph Johnson", "John Vlissides"], new DateTime(1994, 10, 31), 54.99m)
            ];

            #region Section 01a User Defined Delegate
            Console.WriteLine("Titles (custom delegate):");
            LibraryEngine.ProcessBooks(books, new BookFormatter(BookFunctions.GetTitle));
            #endregion

            #region Section 01b Built In Func Delegate
            Console.WriteLine("Authors (Func):");
            LibraryEngine.ProcessBooks(books, new Func<Book, string>(BookFunctions.GetAuthors));
            #endregion

            #region Section 01c Anonymous Method
            Console.WriteLine("ISBNs (anonymous method):");
            LibraryEngine.ProcessBooks(books, new Func<Book, string>(delegate (Book book)
            {
                return BookFunctions.GetISBN(book);
            }));
            #endregion

            #region Section 01d Lambda Expression
            Console.WriteLine("Publication dates (lambda):");
            LibraryEngine.ProcessBooks(books, new Func<Book, string>(book => BookFunctions.GetPublicationDate(book)));
            Console.WriteLine();
            #endregion
        }
        #endregion

        #region Section 02 Setup
        private static void DemonstrateOrderProcessing()
        {
            Console.WriteLine("=== Section 02: Order processing ===");
            Order order = new() { Id = 101, CustomerName = "Ziyad", Price = 120m, Quantity = 2 };
            OrderService service = new();

            #region Part 1 User Defined Delegate
            PriceCalculator normalCalculator = CalculateTotal;
            PriceCalculator discountedCalculator = CalculateTotalWithDiscount;
            Console.WriteLine($"Custom delegate - normal: {service.CalculateOrderPrice(order, normalCalculator):C}");
            Console.WriteLine($"Custom delegate - 10% discount: {service.CalculateOrderPrice(order, discountedCalculator):C}");
            #endregion

            #region Part 2 Func Delegate and Bonus Pricing Strategies
            Dictionary<string, Func<Order, decimal>> pricingStrategies = new()
            {
                ["Normal"] = x => x.Price * x.Quantity,
                ["10% Discount"] = x => x.Price * x.Quantity * 0.90m,
                ["20% Discount"] = x => x.Price * x.Quantity * 0.80m,
                ["VIP Discount"] = x => x.Price * x.Quantity * 0.70m
            };

            foreach ((string name, Func<Order, decimal> strategy) in pricingStrategies)
            {
                Console.WriteLine($"Func - {name}: {service.CalculateOrderPrice(order, strategy):C}");
            }
            #endregion

            #region Part 3 Predicate Validation
            List<Predicate<Order>> validationRules =
            [
                x => x.Quantity > 0,
                x => x.Price > 0,
                x => !string.IsNullOrWhiteSpace(x.CustomerName)
            ];
            Console.WriteLine($"Order valid: {validationRules.All(rule => service.ValidateOrder(order, rule))}");
            #endregion

            #region Part 4 Action Delegate
            List<Action<Order>> postProcessingActions =
            [
                x => Console.WriteLine($"Print: Order {x.Id} for {x.CustomerName}; quantity {x.Quantity}."),
                x => Console.WriteLine($"Confirmation: Email sent to {x.CustomerName} for order {x.Id}."),
                x => Console.WriteLine($"Audit: Order {x.Id} recorded at {DateTime.Now:T}.")
            ];

            foreach (Action<Order> action in postProcessingActions)
            {
                service.ProcessOrder(order, action);
            }
            #endregion

            #region Parts 5 and 6 Events and Event Subscription
            Action<Order> handler1 = x => Console.WriteLine($"Handler 1: dashboard updated for order {x.Id}.");
            Action<Order> handler2 = x => Console.WriteLine($"Handler 2: notification sent for order {x.Id}.");
            Action<Order> handler3 = x => Console.WriteLine($"Handler 3: audit event written for order {x.Id}.");

            service.OrderProcessed += handler1;
            service.OrderProcessed += handler2;
            service.OrderProcessed += handler3;
            Console.WriteLine("Event with all three handlers:");
            service.ProcessOrder(order);

            service.OrderProcessed -= handler1;
            Console.WriteLine("Event after Handler 1 is unsubscribed:");
            service.ProcessOrder(order);
            #endregion
        }
        #endregion

        #region Part 1 Price Calculator Methods
        public static decimal CalculateTotal(Order order) => order.Price * order.Quantity;

        public static decimal CalculateTotalWithDiscount(Order order) => order.Price * order.Quantity * 0.90m;
        #endregion
    }
}