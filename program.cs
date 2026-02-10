using Gruberooapp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Gruberooapp
{
    class Program
    {
        static List<Restaurant> restaurants = new();
        static List<Customer> customersList = new();
        static List<Order> orderList = new();
        static int foodItemNumber = 0;
        static Stack<Order> refundStack = new Stack<Order>();

        static void Main()
        {
            LoadRestaurants();
            LoadFoodItems();
            LoadCustomers(customersList);
            LoadOrders(orderList,customersList,restaurants);



            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
            Console.WriteLine($"{restaurants.Count} restaurants loaded!");
            Console.WriteLine($"{foodItemNumber} food items loaded!");
            Console.WriteLine($"{customersList.Count} customers loaded!");
            Console.WriteLine($"{orderList.Count} orders loaded!");
            

            ShowMenu(); 
        }

        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================
        static void LoadRestaurants()
        {
            
            var lines = File.ReadAllLines("restaurants.csv").Skip(1).Take(16);

            foreach (var line in lines)
            {
                var p = line.Split(',');

                if (p.Length < 3)
                {
                    Console.WriteLine("Skipping invalid restaurant line: " + line);
                    continue;
                }

                restaurants.Add(new Restaurant(p[0], p[1], p[2]));
            }
        }
        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================
        static void LoadFoodItems()
        {
            foreach (var line in File.ReadAllLines("fooditems.csv").Skip(1))
            {
                var p = line.Split(',');

                if (p.Length < 3)
                {
                    Console.WriteLine("Skipping invalid restaurant line: " + line);
                    continue;
                }

                foodItemNumber++;

            }

        }

        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void LoadCustomers(List<Customer> customerList)
        {
            string[] customerLines = File.ReadAllLines("customers.csv");
            for (int i = 1; i < customerLines.Length; i++)
            {
                string[] data = customerLines[i].Split(',');
                Customer c = new Customer(data[1].Trim(), data[0].Trim());
                customerList.Add(c);
            }
        }




        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void LoadOrders(List<Order> orderList, List<Customer> customersList, List<Restaurant> restaurants)
        {

            string[] lines = File.ReadAllLines("orders.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                int id = int.Parse(data[0]);
                string email = data[1];
                string restId = data[2];
                string deliverydate = data[3] + " " + data[4];
                DateTime deliverydatetime = DateTime.Parse(deliverydate);
                string adddress = data[5];
                DateTime orderdatetime = DateTime.Parse(data[6]);
                double total = double.Parse(data[7]); 
                string status = data[8];
                bool orderPaid = true;
                
                
                //create order object
                Order newOrder = new Order(id, orderdatetime, total, status, deliverydatetime, adddress, "", orderPaid);
                orderList.Add(newOrder);

                //link to customersList
                foreach (Customer c in customersList)
                {
                    if (c.emailAddress == email)
                    {
                        c.orderList.Add(newOrder);
                        break;
                    }
                }
                
                //link to restaurantqueue
                foreach (Restaurant rest in restaurants)
                {
                    if (rest.RestaurantId == restId)
                    {
                        rest.OrderQueue.Enqueue(newOrder);
                        break;
                    }
                }
            }
        }
        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================

        static void ListAllOrders()
        {
            Console.WriteLine("\nAll Orders");
            Console.WriteLine("==========");
            Console.WriteLine($"{"Order ID",-10} {"Customer",-15} {"Restaurant",-20} {"Delivery Date/Time",-20} {"Amount",-10} {"Status",-12}");
            Console.WriteLine($"{new string('-', 8),-10} {new string('-', 10),-15} {new string('-', 13),-20} {new string('-', 18),-20} {new string('-', 6),-10} {new string('-', 9),-12}");

            foreach (Order order in orderList)
            {
                // FIX: Compare OrderId instead of using Contains()
                string customerName = "";
                foreach (Customer c in customersList)
                {
                    foreach (Order custOrder in c.orderList)
                    {
                        if (custOrder.OrderId == order.OrderId)
                        {
                            customerName = c.customerName;
                            break;
                        }
                    }
                    if (customerName != "") break;
                }

                // Find the restaurant for this order
                string restaurantName = "";
                foreach (Restaurant r in restaurants)
                {
                    foreach (Order restOrder in r.OrderQueue)
                    {
                        if (restOrder.OrderId == order.OrderId)
                        {
                            restaurantName = r.RestaurantName; // Check if it's r.Name or r.RestaurantName
                            break;
                        }
                    }
                    if (restaurantName != "") break;
                }

                // Format delivery date/time and amount
                string formattedDateTime = $"{order.DeliveryDateTime:dd/MM/yyyy HH:mm}";
                string formattedAmount = $"${order.OrderTotal:F2}";

                Console.WriteLine($"{order.OrderId,-10} {customerName,-15} {restaurantName,-20} {formattedDateTime,-20} {formattedAmount,-10} {order.OrderStatus,-12}");
            }
            Console.WriteLine();
        }


        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================
        static void ProcessOrders()
        {
            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine();

            // Find restaurant
            Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId == restId);
            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found.");
                return;
            }

            if (restaurant.OrderQueue.Count == 0)
            {
                Console.WriteLine("No orders in queue for this restaurant.");
                return;
            }

            // Refund stack
            Stack<Order> refundStack = new Stack<Order>();

            // Temporary queue to loop through orders
            Queue<Order> tempQueue = new Queue<Order>(restaurant.OrderQueue);

            while (tempQueue.Count > 0)
            {
                Order order = tempQueue.Dequeue();

                // Ensure the order is Pending (student version)
                if (string.IsNullOrEmpty(order.OrderStatus))
                {
                    order.OrderStatus = "Pending";
                }

                // Find customer
                string customerName = "Unknown";
                foreach (var c in customersList)
                {
                    if (c.orderList.Contains(order))
                    {
                        customerName = c.customerName;
                        break;
                    }
                }

                // Display order info
                Console.WriteLine($"\nOrder {order.OrderId}:");
                Console.WriteLine($"Customer: {customerName}");
                Console.WriteLine("Ordered Items:");
                for (int i = 0; i < order.orderedfooditem.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {order.orderedfooditem[i]}");
                }

                DateTime deliveryTime = DateTime.Now; 
                Console.WriteLine($"Delivery date/time: {deliveryTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Total Amount: {order.CalculateOrderTotal():C}");
                Console.WriteLine($"Order Status: {order.OrderStatus}");

                Console.Write("Enter [C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "C":
                        if (order.OrderStatus == "Pending")
                        {
                            order.OrderStatus = "Preparing";
                            Console.WriteLine($"Order {order.OrderId} confirmed. Status: {order.OrderStatus}");
                        }
                        else
                        {
                            Console.WriteLine("Cannot confirm. Order is not pending.");
                        }
                        break;

                    case "R":
                        if (order.OrderStatus == "Pending")
                        {
                            order.OrderStatus = "Rejected";
                            refundStack.Push(order);
                            Console.WriteLine($"Order {order.OrderId} rejected. Refund processed.");
                        }
                        else
                        {
                            Console.WriteLine("Cannot reject. Order is not pending.");
                        }
                        break;

                    case "S":
                        if (order.OrderStatus == "Cancelled")
                        {
                            Console.WriteLine($"Order {order.OrderId} skipped.");
                        }
                        else
                        {
                            Console.WriteLine("Skipping only allowed for cancelled orders.");
                        }
                        break;

                    case "D":
                        if (order.OrderStatus == "Preparing")
                        {
                            order.OrderStatus = "Delivered";
                            Console.WriteLine($"Order {order.OrderId} delivered. Status: {order.OrderStatus}");
                        }
                        else
                        {
                            Console.WriteLine("Cannot deliver. Order is not preparing.");
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid choice. Skipping order.");
                        break;
                }
            }
        }


        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================

        static void DeleteOrder()
        {
            Console.WriteLine("\nDelete Order");
            Console.WriteLine("============");

            // Prompt for customer email
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();

            // Find customer
            Customer customer = null;
            foreach (Customer c in customersList)
            {
                if (c.emailAddress.Equals(email, StringComparison.OrdinalIgnoreCase))
                {
                    customer = c;
                    break;
                }
            }

            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            // Display all pending orders for this customer
            List<Order> pendingOrders = new List<Order>();
            foreach (Order order in customer.orderList)
            {
                if (order.OrderStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    pendingOrders.Add(order);
                }
            }

            if (pendingOrders.Count == 0)
            {
                Console.WriteLine("No pending orders found for this customer.");
                return;
            }

            Console.Write("Pending Orders: ");
            foreach (Order order in pendingOrders)
            {
                Console.Write(order.OrderId + " ");
            }
            Console.WriteLine();

            // Prompt for Order ID
            Console.Write("Enter Order ID: ");
            int orderId;
            if (!int.TryParse(Console.ReadLine(), out orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                return;
            }

            // Find the order
            Order selectedOrder = null;
            foreach (Order order in pendingOrders)
            {
                if (order.OrderId == orderId)
                {
                    selectedOrder = order;
                    break;
                }
            }

            if (selectedOrder == null)
            {
                Console.WriteLine("Order ID not found in pending orders.");
                return;
            }

            // Display order details
            Console.WriteLine($"Customer: {customer.customerName}");
            Console.WriteLine("Ordered Items:");
            int itemNumber = 1;
            foreach (string item in selectedOrder.orderedfooditem)
            {
                Console.WriteLine($"{itemNumber}. {item}");
                itemNumber++;
            }
            Console.WriteLine($"Delivery date/time: {selectedOrder.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${selectedOrder.OrderTotal:F2}");
            Console.WriteLine($"Order Status: {selectedOrder.OrderStatus}");

            // Confirm deletion
            Console.Write("Confirm deletion? [Y/N]: ");
            string confirmation = Console.ReadLine().Trim().ToUpper();

            if (confirmation == "Y")
            {
                // Update status to Cancelled
                selectedOrder.OrderStatus = "Cancelled";

                // Add to refund stack (assuming you have a refundStack variable)
                refundStack.Push(selectedOrder);

                Console.WriteLine($"Order {selectedOrder.OrderId} cancelled. Refund of ${selectedOrder.OrderTotal:F2} processed.");
            }
            else
            {
                Console.WriteLine("Order deletion cancelled.");
            }
        }

        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================
        // ================= MENU =================
        static void ShowMenu()
        {
            while (true)
            {
                Console.WriteLine("\n===== Gruberoo Food Delivery System =====");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all orders");
                Console.WriteLine("3. Create a new order");
                Console.WriteLine("4. Process an order");
                Console.WriteLine("5. Modify an existing order");
                Console.WriteLine("6. Delete an existing order");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        //ListAllRestaurantsAndMenu();
                        break;
                    case "2":
                        ListAllOrders();
                        break;
                    case "3":
                        // CreateNewOrder();
                        break;
                    case "4":
                         ProcessOrders();
                        break;
                    case "5":
                        //ModifyOrder();
                        break;
                    case "6":
                        DeleteOrder();
                        break;
                    case "0":
                        return;
                    default:
                        break;
                }
            }
        }
    }
}

