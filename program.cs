using Gruberooapp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;


namespace Gruberooapp
{
    class Program
    {
        static List<Restaurant> restaurants = new();
        static List<Customer> customersList = new();
        static List<Order> orderList = new();
        static List<FoodItem> foodItems = new();
        static List<FoodItem> Menu = new();
        static List<Menu> menus = new();
        static List<Restaurant> restaurantlist = new();
        static int foodItemNumber = 0;
        static Stack<Order> refundStack = new Stack<Order>();

        static void Main()
        {
            LoadRestaurants();
            LoadFoodItems();
            LoadCustomers(customersList);
            LoadOrders(orderList, customersList, restaurants);



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
        static void LoadCustomers(List<Customer> customersList)
        {
            string[] customerLines = File.ReadAllLines("customers.csv");
            for (int i = 1; i < customerLines.Length; i++)
            {
                string[] data = customerLines[i].Split(',');
                Customer c = new Customer(data[0].Trim(), data[1].Trim());
                customersList.Add(c);
            }
        }




        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void LoadOrders(List<Order> orderlist, List<Customer> customerslist, List<Restaurant> restaurants)
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
                        //c.orderlist.Add(newOrder);
                        c.AddOrder(newOrder);
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

                // Fill order with default items if empty
                if (order.orderedfooditem.Count == 0)
                {
                    order.orderedfooditem.Add("Chicken Rice - 2");
                    order.orderedfooditem.Add("Beef Burger - 1");
                }

                var customer = customersList.FirstOrDefault(c => c.orderList.Any(o => o.OrderId == order.OrderId));
                string name = customer?.customerName ?? "Unknown";
                string address = order.DeliveryAddress ?? "No Address Found";

                // Display order details
                Console.WriteLine($"\nOrder {order.OrderId}:");
                Console.WriteLine($"Customer: {name}");
                Console.WriteLine($"Delivery Address: {address}");
                Console.WriteLine("Ordered Items:");
                for (int i = 0; i < order.orderedfooditem.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {order.orderedfooditem[i]}");
                }
                Console.WriteLine($"Delivery date/time: {order.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                Console.WriteLine($"Total Amount: {order.OrderTotal:C}");
                Console.WriteLine($"Order Status: {order.OrderStatus}");

                
                Console.Write("\n[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
                string choice = (Console.ReadLine() ?? "").Trim().ToUpper();

                switch (choice)
                {
                    case "C":
                        if (order.OrderStatus == "Pending")
                        {
                            order.OrderStatus = "Preparing"; // Confirm order
                            Console.WriteLine($"\nOrder {order.OrderId} confirmed and is now 'Preparing'.");
                        }
                        else
                        {
                            Console.WriteLine("\nCannot confirm order. Status is not 'Pending'.");
                        }
                        break;

                    case "R":
                        if (order.OrderStatus == "Pending")
                        {
                            order.OrderStatus = "Rejected"; // Reject order
                            refundStack.Push(order);        // Add to refund stack
                            Console.WriteLine($"\nOrder {order.OrderId} rejected. Refund processed.");
                        }
                        else
                        {
                            Console.WriteLine("\nCannot reject order. Status is not 'Pending'.");
                        }
                        break;

                    case "S":
                        if (order.OrderStatus == "Cancelled")
                        {
                            Console.WriteLine("\nOrder skipped (already cancelled).");
                        }
                        else
                        {
                            Console.WriteLine("\nCannot skip order. Status is not 'Cancelled'.");
                        }
                        break;

                    case "D":
                        if (order.OrderStatus == "Preparing")
                        {
                            order.OrderStatus = "Delivered"; // Deliver order
                            Console.WriteLine($"\nOrder {order.OrderId} has been successfully delivered.");
                        }
                        else
                        {
                            Console.WriteLine("\nCannot deliver order. Status is not 'Preparing'.");
                        }
                        break;

                    default:
                        Console.WriteLine("\nInvalid action.");
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

        //================Advanced Feature B=============================
        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================
        static void DisplayTotalOrderAmount()
        {
            Console.WriteLine("\nFinancial Summary Report");
            Console.WriteLine("========================");

            double grandTotalOrders = 0;
            double grandTotalRefunds = 0;
            const double DELIVERY_FEE = 5.00;

            foreach (Restaurant rest in restaurants)
            {
                double restaurantOrderTotal = 0;
                double restaurantRefundTotal = 0;


                foreach (Order order in orderList)
                {

                    bool isRestOrder = rest.OrderQueue.Any(o => o.OrderId == order.OrderId);

                    if (isRestOrder)
                    {
                        if (order.OrderStatus == "Delivered")
                        {

                            restaurantOrderTotal += (order.OrderTotal - DELIVERY_FEE);
                        }
                        else if (order.OrderStatus == "Rejected" || order.OrderStatus == "Cancelled")
                        {

                            restaurantRefundTotal += order.OrderTotal;
                        }
                    }
                }

                Console.WriteLine($"Restaurant: {rest.RestaurantName,-20}");
                Console.WriteLine($"  - Total Successful Orders (less fees): ${restaurantOrderTotal:F2}");
                Console.WriteLine($"  - Total Refunds:                       ${restaurantRefundTotal:F2}");
                Console.WriteLine();

                grandTotalOrders += restaurantOrderTotal;
                grandTotalRefunds += restaurantRefundTotal;
            }

            double finalEarnings = grandTotalOrders - grandTotalRefunds;

            Console.WriteLine("-------------------------------------------");
            Console.WriteLine($"Total Order Amount (All): ${grandTotalOrders:F2}");
            Console.WriteLine($"Total Refunds (All):      ${grandTotalRefunds:F2}");
            Console.WriteLine($"Final Gruberoo Earnings:  ${finalEarnings:F2}");
            Console.WriteLine("-------------------------------------------");
        }

        //================Advanced Feature C=============================
        //==========================================================
        // Student Number : S10275496C
        // Student Name : Esther Teo
        // Partner Name : Yap Jia Xuan 
        //==========================================================

        static void CreateOrderWithSpecialOffer()
        {
            Console.WriteLine("Create New Order (Special Offer)");
            Console.WriteLine("=============================== ");


            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine();
            Customer customer = customersList.Find(c => c.emailAddress == email);
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }


            Console.Write("Enter Restaurant ID: ");
            string resId = Console.ReadLine();
            Restaurant restaurant = restaurants.Find(r => r.RestaurantId == resId);
            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found.");
                return;
            }



            restaurant.Menu.Clear();
            string[] foodLines = File.ReadAllLines("fooditems.csv");
            for (int i = 1; i < foodLines.Length; i++)
            {
                string[] data = foodLines[i].Split(',');
                if (data[0] == resId)
                {
                    string name = data[1];
                    string desc = data[2];
                    double price = double.Parse(data[3]);
                    restaurant.Menu.Add(new OrderFoodItem(name, desc, price, " ", 0, 0.0));
                }
            }
            // -------------------------------------------------------

            if (restaurant.Menu.Count == 0)
            {
                Console.WriteLine("This restaurant currently has no items available.");
                return;
            }


            Console.WriteLine("\nAvailable Food Items:");
            for (int i = 0; i < restaurant.Menu.Count; i++)
            {
                var item = restaurant.Menu[i];
                Console.WriteLine($"{i + 1}. {item.itemName} - ${item.itemPrice:F2}");
            }

            double subtotal = 0;
            List<string> itemNames = new List<string>();

            while (true)
            {
                Console.Write("Enter item number (0 to finish): ");
                string input = Console.ReadLine();
                if (input == "0" || string.IsNullOrEmpty(input)) break;

                if (int.TryParse(input, out int choice) && choice > 0 && choice <= restaurant.Menu.Count)
                {
                    Console.Write("Enter quantity: ");
                    int qty = int.Parse(Console.ReadLine());

                    var foodItem = restaurant.Menu[choice - 1];
                    subtotal += (foodItem.itemPrice * qty);
                    itemNames.Add($"{foodItem.itemName} x{qty}");
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            if (subtotal == 0) return;


            double discountRate = 0.0;
            Console.Write("\nEnter Promo Code (SAVE10 / GRUB20) or press Enter to skip: ");
            string promo = Console.ReadLine().ToUpper();

            if (promo == "SAVE10") discountRate = 0.10;
            else if (promo == "GRUB20") discountRate = 0.20;


            double discountAmount = subtotal * discountRate;
            double finalTotal = (subtotal - discountAmount) + 5.00;


            Console.WriteLine("\n--- Order Summary ---");
            Console.WriteLine($"Original Subtotal: ${subtotal:F2}");
            if (discountRate > 0) Console.WriteLine($"Discount ({discountRate * 100}%): -${discountAmount:F2}");
            Console.WriteLine($"Delivery Fee:      $5.00");
            Console.WriteLine($"Total to Pay:      ${finalTotal:F2}");


            Console.Write("\nConfirm and Pay? [Y/N]: ");
            if (Console.ReadLine().ToUpper() == "Y")
            {
                int newID = new Random().Next(1000, 9999);
                Order newOrder = new Order(newID, DateTime.Now, finalTotal, "Pending", DateTime.Now.AddHours(1), "Delivery Address", "CC", true);

                foreach (string name in itemNames)
                    newOrder.AddOrderedFoodItem(name);

                customer.AddOrder(newOrder);
                restaurant.OrderQueue.Enqueue(newOrder);

                Console.WriteLine($"Order {newID} created successfully!");
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
                Console.WriteLine("7. Display Order Total Amount");
                Console.WriteLine("8. Create Order With Special Offer");
                Console.WriteLine("9. Process Bulk Order");
                Console.WriteLine("10. Add Order to Favourite:");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        ListAllRestaurantAndMenu(restaurants,foodItems);
                        break;
                    case "2":
                        ListAllOrders();
                        break;
                    case "3":
                        CreateNewOrder(customersList,restaurantlist);
                        break;
                    case "4":
                        ProcessOrders();
                        break;
                    case "5":
                        ModifyOrder(customersList,orderList);
                        break;
                    case "6":
                        DeleteOrder();
                        break;
                    case "7":
                        DisplayTotalOrderAmount();
                        break;
                    case "8":
                        CreateOrderWithSpecialOffer();
                        break;
                    case "9":
                        ProcessBulkOrder();
                        break;
                    case "10":
                        AddToFavourite();
                        break;

                    case "0":
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        break;
                }
            }
        }
        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void ListAllRestaurantAndMenu(List<Restaurant> restaurants, List<FoodItem> foodItems)
        {
            string[] lines = File.ReadAllLines("fooditems.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                string itemname = data[1];
                string itemdesc = data[2];
                double itemprice = double.Parse(data[3]);
                string cust = " ";
                int qty = 0;
                double subtotal = 0.0;


                FoodItem foodItem = new OrderFoodItem(itemname, itemdesc, itemprice, cust, qty, subtotal);
                foodItems.Add(foodItem);
            }

            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("============================== ");

            foreach (var res in restaurants)
            {
                Console.WriteLine($"Restaurant: {res.RestaurantName} ({res.RestaurantId})");
                res.DisplayMenu();

                foreach (var fooditem in foodItems)
                {
                    Console.WriteLine($"   - {fooditem.itemName} - {fooditem.itemPrice}");
                }

                Console.WriteLine();
            }
        }

        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan
        // Partner Name : Esther Teo Hui Min
        //==========================================================
       static void CreateNewOrder(List<Customer> customerList, List<Restaurant> restaurantlist)
            {
            Console.WriteLine("Create New Order");
            Console.WriteLine("================ ");
            Console.WriteLine("Enter Customer Email:");
            string email = Console.ReadLine();
            Customer customer = customerList.Find(c => c.emailAddress == email);
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }
            string[] lines = File.ReadAllLines("restaurants.csv");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                string restid = data[0];
                string restname = data[1];
                string restemail = data[2];

                Restaurant restaurantt = new Restaurant(restid, restname, restemail);
                restaurantlist.Add(restaurantt);
            }

            Console.WriteLine("Enter Restaurant ID:");
            string resId = Console.ReadLine();
            Restaurant restaurant = restaurantlist.Find(r => r.RestaurantId == resId);
            if (restaurant == null)
            {
                Console.WriteLine("Error: Restaurant ID not found.");
                return;
            }

            Console.WriteLine("Enter Delivery Date (dd/mm/yyyy):");
            string dateinput = Console.ReadLine();

            Console.WriteLine("Enter Delivery Time (hh:mm)");
            string timeinput = Console.ReadLine();
            DateTime deliverydatetime = DateTime.Parse(dateinput + " " + timeinput);

            Console.WriteLine("Enter Delivery Address:");
            string address = Console.ReadLine();

            Console.WriteLine("\nAvaibale Food Items:");
            string[] line = File.ReadAllLines("fooditems.csv");
            for (int i = 1; i < line.Length; i++)
            {
                string[] foodData = line[i].Split(',');
                string foodRestId = foodData[0];
                string name = foodData[1];
                string desc = foodData[2];
                double price = double.Parse(foodData[3]);

                Restaurant target = restaurantlist.Find(r => r.RestaurantId == foodRestId);

                if (target != null)
                {
                    target.Menu.Add(new OrderFoodItem(name, desc, price, " ", 0, 0.0));
                }
            }
            for (int i = 0; i < restaurant.Menu.Count; i++)
            {
                var item = restaurant.Menu[i];
                Console.WriteLine($"{i + 1}. {item.itemName}: {item.itemDesc} - ${item.itemPrice:F2}");
            }

            double subtotal = 0;
            List<OrderFoodItem> selecteditem = new List<OrderFoodItem>();

            while (true)
            {
                Console.WriteLine("Enter item number (0 to finish):");
                int choice = int.Parse(Console.ReadLine());
                if (choice == 0) break;

                Console.WriteLine("Enter quantity:");
                int quantity = int.Parse(Console.ReadLine());

                FoodItem chosen = restaurant.Menu[choice - 1];
                OrderFoodItem ordered = new OrderFoodItem(chosen.itemName, chosen.itemDesc, chosen.itemPrice, " ", quantity, 0.0);
                selecteditem.Add(ordered);

                subtotal += ordered.CalculateSubtotal();
            }

            Console.WriteLine("Any special request? [Y/N]:");
            string special = Console.ReadLine().ToUpper();
            if (special == "Y")
            {
                Console.WriteLine("Enter your special request:");
            }
            else if (special == "N")
            {
            }

            double totalwithfee = subtotal + 5.00;
            Console.WriteLine($"\nOrder Total: ${subtotal:F2} + $5.00 (delivery) = ${totalwithfee:F2}");

            Console.WriteLine("Proceed to payment? [Y/N]");
            string proceed = Console.ReadLine().ToUpper();
            if (proceed == "Y")
            {
                Console.WriteLine("[CC] Credit Card / [CD] Cash on Delivery:");
                string method = Console.ReadLine().ToUpper();

                int newID = 0427;
                Order neworder = new Order(newID, DateTime.Now, totalwithfee, "Pending", deliverydatetime, address, method, true);

                foreach (var item in selecteditem)
                {
                    neworder.AddOrderedFoodItem(item.itemName);
                }

                customer.AddOrder(neworder);
                restaurant.OrderQueue.Enqueue(neworder);

                Console.WriteLine($"Order {newID} created successfully! Status: Pending");
            }
        }

        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void ModifyOrder(List<Customer> customerList, List<Order> orderList)
        {
            Console.WriteLine("Modify Order");
            Console.WriteLine("============ ");

            Console.WriteLine("Enter Customer Email:");
            string custemail = Console.ReadLine();
            Customer customer = customersList.Find(c => c.emailAddress == custemail);
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.WriteLine("Pending Orders:");
            bool pending = false;
            foreach (Order o in customer.orderList)
            {
                if (o.OrderStatus == "Pending")
                {
                    Console.WriteLine(o.OrderId);
                    pending = true;
                }
            }
            if (!pending)
            {
                Console.WriteLine("No pending orders found.");
                return;
            }

            Console.WriteLine("Enter Order ID:");
            int orderid = int.Parse(Console.ReadLine());
            Order order = customer.orderList.Find(o => o.OrderId == orderid && o.OrderStatus == "Pending");
            if (order == null)
            {
                Console.WriteLine("Invalid Order ID / Order not pending.");
                return;
            }

            Console.WriteLine("Order Items:");


            Console.WriteLine($"Address: {order.DeliveryAddress}");
            Console.WriteLine($"Delivery Date/Time: {order.DeliveryDateTime:dd/MM/yyyy, HH:mm}");

            Console.WriteLine("Modify: [1] Items [2] Address [3] Delivery Time: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ModifyOrder(customerList, orderList);
            }
            else if (choice == "2")
            {
                Console.WriteLine("Enter new Delivery Address: ");
                order.DeliveryAddress = Console.ReadLine();
                Console.WriteLine($"Order {order.OrderId} updated. New address: {order.DeliveryAddress}");
            }
            else if (choice == "3")
            {
                Console.WriteLine("Enter new Delivery Time (hh:mm):");
                string newtime = Console.ReadLine();
                DateTime current = order.DeliveryDateTime;
                TimeSpan time = TimeSpan.Parse(newtime);
                order.DeliveryDateTime = current.Date + time;
                Console.WriteLine($"Order {order.OrderId} updated. New Delivery Time:{newtime}");
            }

        }
        //================Advanced Feature A=============================
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void ProcessBulkOrder()
        {
            Console.WriteLine("\n--- Bulk Processing Pending Order ---");
            Console.WriteLine("Enter Restaurant ID:");
            string restaurantid = Console.ReadLine().Trim().ToUpper();

            Restaurant restaurant = restaurants.FirstOrDefault(r => r.RestaurantId.ToUpper() == restaurantid);

            if (restaurant == null)
            {
                Console.WriteLine("Restaurant not found!");
                return;
            }

            if (restaurant.OrderQueue.Count == 0)
            {
                Console.WriteLine("The queue for this restaurant is empty!");
                return;
            }

            int totalPending = 0;
            int accepted = 0;
            int rejected = 0;
            int queuestart = restaurant.OrderQueue.Count;

            Queue<Order> tempqueue = new Queue<Order>();

            while (restaurant.OrderQueue.Count > 0)
            {
                Order order = restaurant.OrderQueue.Dequeue();
                if (order.OrderStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase)) //StringComparison.OrdinalIgnoreCase is to ignore the upper/lower case
                {
                    totalPending++;
                    TimeSpan timetodelivery = order.DeliveryDateTime - DateTime.Now; //TimeSpan is data type used to represent time interval;
                    if (timetodelivery.TotalHours < 1)
                    {
                        order.OrderStatus = "Rejected";
                        rejected++;
                        refundStack.Push(order);
                    }
                    else
                    {
                        order.OrderStatus = "Preparing";
                        accepted++;
                    }
                }
                tempqueue.Enqueue(order);
            }
            foreach (Order order in tempqueue)
            {
                restaurant.OrderQueue.Enqueue(order);
            }

            double successrate = totalPending > 0 ? ((double)accepted / totalPending) * 100 : 0;
            //to explain line 820:
            //if totalPending > 0  , then perform ((double)accepted / totalPending) * 100 : 0
            //if totalPending <= 0, set successrate = 0;
            // part after "?" represent Condition = YES;
            //part after ":" represent Condition = NO'

            Console.WriteLine($"\nBulk Processing Summary for {restaurant.RestaurantName}:");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Total Orders in Queue: {queuestart}");
            Console.WriteLine($"Pending Orders Identified: {totalPending}");
            Console.WriteLine($"Auto Accepte: {accepted} (Status: Preparing)");
            Console.WriteLine($"Auto Reject: {rejected} (Status: Rejected)");
            Console.WriteLine($"Acceptance Rate: {successrate:F2}%");
            Console.WriteLine("--------------------------------------------------");
        }

        //================Advanced Feature C =============================
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void AddToFavourite()
        {
            Console.WriteLine("Enter Customer Email:");
            string email = Console.ReadLine();
            Customer c = customersList.Find(cust => cust.emailAddress == email);
            if (c == null)
            {
                Console.WriteLine("Customer not found!");
                return;
            }

            if (c.orderList.Count == 0)
            {
                Console.WriteLine("No past favourite order found!");
                return;
            }

            Console.WriteLine("\n--- Your Past Orders ---");
            for (int i = 0; i < c.orderList.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Order ID: {c.orderList[i].OrderId} | Date: {c.orderList[i].OrderDateTime:dd/MM/yyyy}");
            }

            Console.WriteLine("Enter number of the order to favourite:");
            int num = int.Parse(Console.ReadLine());

            if (num >= 0 && num < c.orderList.Count)
            {
                c.favouriteorder.Add(c.orderList[num]);
                Console.WriteLine("Order successfully added to Favourites!");
            }
        }

    }
}

