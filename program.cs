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
                Customer c = new Customer(data[0].Trim(), data[1].Trim());
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
            Console.WriteLine("Process Order");
            Console.WriteLine("=============");
            Console.Write("Enter Restaurant ID: ");
            string restId = Console.ReadLine();

            Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId == restId);

            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found.");
                return;
            }

            if (restaurant.OrderQueue.Count == 0)
            {
                Console.WriteLine("No orders in queue.");
                return;
            }

            
            Queue<Order> tempQueue = new Queue<Order>(restaurant.OrderQueue);

            while (tempQueue.Count > 0)
            {
                Order order = tempQueue.Dequeue();

                
                if (order.orderedfooditem.Count == 0)
                {
                    order.orderedfooditem.Add("Chicken Rice - 2");
                    order.orderedfooditem.Add("Beef Burger - 1");
                }

                
                var customer = customersList.FirstOrDefault(c => c.orderList.Any(o => o.OrderId == order.OrderId));

                string name = customer?.customerName ?? "Unknown";
                string address = order.DeliveryAddress ?? "No Address Found"; ;

               
                Console.WriteLine($"\nOrder {order.OrderId}:");
                Console.WriteLine($"\nCustomer:\n{name}");
                Console.WriteLine($"\nDelivery Address:\n{address}");

                Console.WriteLine("\nOrdered Items:");
                for (int i = 0; i < order.orderedfooditem.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {order.orderedfooditem[i]}");
                    Console.WriteLine(); // Spacing
                }

                Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"\nTotal Amount: {order.OrderTotal:C}");
                Console.WriteLine($"\nOrder Status: {order.OrderStatus}");

                
                Console.Write("\n[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string choice = Console.ReadLine().ToUpper();

                if (choice == "C" && order.OrderStatus == "Pending")
                {
                    order.OrderStatus = "Preparing";
                    Console.WriteLine($"\nOrder {order.OrderId} confirmed and is now 'Preparing'.");
                }
                else if (choice == "R" && order.OrderStatus == "Pending")
                {
                    order.OrderStatus = "Rejected";
                    refundStack.Push(order); 
                    Console.WriteLine($"\nOrder {order.OrderId} rejected. Refund processed.");
                }
                else if (choice == "D" && order.OrderStatus == "Preparing")
                {
                    order.OrderStatus = "Delivered";
                    Console.WriteLine($"\nOrder {order.OrderId} has been successfully delivered.");
                }
                else if (choice == "S")
                {
                    Console.WriteLine("\nOrder skipped.");
                }
                else
                {
                    Console.WriteLine("\nInvalid action for the current order status.");
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

            
            
            Console.Write("Confirm deletion? [Y/N]: ");
            
            string confirmation = (Console.ReadLine() ?? "").Trim().ToUpper();

            if (confirmation == "Y")
            {
                // Update status to Cancelled
                selectedOrder.OrderStatus = "Cancelled";

                
                refundStack.Push(selectedOrder);

                Console.WriteLine($"Order {selectedOrder.OrderId} cancelled. Refund of ${selectedOrder.OrderTotal:F2} processed.");
            }
            else if (confirmation == "N")
            {
                Console.WriteLine("Order deletion cancelled.");
            }
            else
            {
                Console.WriteLine("Invalid input. Operation aborted.");
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
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        break;
                }
            }
        }
    }
}

