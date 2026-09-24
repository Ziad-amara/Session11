Assignment questions (Q1–Q8)

Q1 — Difference between PriceCalculator and Func<Order, decimal>? 

They have the same signature and are interchangeable at the call site, but PriceCalculator is a distinct,
named delegate type declared with delegate decimal PriceCalculator(Order order);,
while Func<Order, decimal> is a generic delegate type already provided by .NET. 
A custom delegate is useful when the name itself adds meaning or when you want a type that's clearly distinct from any other method with the same shape;
Func<> is useful when you just need "a method with this signature" with no extra ceremony.


Q2 — Difference between Action<Order> and Func<Order, decimal>?

Action<Order> represents a method that takes an Order and returns nothing (void)
— used for side effects like printing, logging, sending a message. 
Func<Order, decimal> represents a method that takes an Order and returns a decimal 
— used when you need a computed result back.


Q3 — Why does Predicate<T> return bool? What problem is it designed to represent?

Predicate<T> exists specifically to represent a yes/no test — "does this item satisfy some condition?" 
Returning bool is what makes it a predicate in the logical sense (as in predicate logic).
It's designed for filtering/validation scenarios (e.g. List<T>.Find, List<T>.RemoveAll, or, here, order validation rules),
where the only meaningful output is true or false.


Q4 — Difference between a delegate and an event?

A delegate is a type — a reference to one or more methods that can be invoked directly by anyone who holds it, 
and freely reassigned with =. An event is a controlled wrapper around a delegate: code outside the declaring class can only += or -= (subscribe/unsubscribe);
it cannot invoke the event or overwrite its subscriber list. Only the class that declares the event can call Invoke on it.


Q5 — Why can't external code normally invoke an event declared in another class?

Because the event keyword restricts access to the delegate's invoke mechanism to the declaring class.
Outside code only sees the +=/-= operations; the compiler doesn't expose Invoke() (or plain = assignment) to it. 
This protects the class's internal notification mechanism 
— nobody outside OrderService can fake an "order processed" notification or wipe out other subscribers by accident.


Q6 — What happens when multiple handlers subscribe to the same event?

The event becomes a multicast delegate: it holds an internal invocation list of every subscribed method. 
When the event is raised, every method in that list runs in the order it was added. In this project, all three handlers (Handler1_PrintMessage, Handler2_SendNotification, Handler3_AuditLog) run one after another from a single OrderProcessed?.Invoke(order) call.


Q7 — Explain orderService.OrderProcessed += HandleOrderProcessed;

OrderProcessed — the event field on orderService, a multicast delegate of type Action<Order>.
+= — "add this method to the event's invocation list" (subscribe); it does not replace existing subscribers, it appends.
HandleOrderProcessed — the method being added; its signature must match Action<Order> (takes an Order, returns void).
Together the line means: "whenever orderService raises OrderProcessed, also call HandleOrderProcessed as one of the handlers."


Q8 — Challenge: Action<Order> vs event Action<Order>

— why use an event? A plain Action<Order> field can be invoked and reassigned by any code that has access to it 
— someObject.SomeAction(order) or even someObject.SomeAction = someOtherDelegate (silently discarding all previous subscribers) are both legal from outside the class.
An event Action<Order> locks that down: outside code can only add/remove itself from the invocation list, 
and only the declaring class can actually raise the notification or clear the list. Using event protects the publish/subscribe contract 
— it guarantees OrderService is the sole authority over when "order processed" actually fires, which is exactly the safety a notification mechanism needs.
