using Gruberooapp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Gruberoo
{
    class Program
    {
        static List<Restaurant> restaurants = new();
        static List<Customer> customersList = new();
        static List<Order> orderList = new();
        static int foodItemNumber = 0;

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
            // Skip(1) removes the header, Take(16) limits the results to the first 16 data rows
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
                customersList.Add(c);
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
                string paymentmethod = "Credit Card";

                //create order object
                Order newOrder = new Order(id, orderdatetime, total, status, deliverydatetime, adddress, paymentmethod, orderPaid);
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
                        ListAllRestaurantAndMenu(restaurants);
                        break;
                    case "2":
                        //ListAllOrders();
                        break;
                    case "3":
                        //CreateNewOrder(customerList,restaurantlist);
                        break;
                    case "4":
                        // ProcessOrders();
                        break;
                    case "5":
                        //ModifyOrder(customerList,restaurantList);
                        break;
                    case "6":
                        // Delete order
                        break;
                    case "0":
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
        static void ListAllRestaurantAndMenu(List<Restaurant> restaurants)
        {
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("============================== ");
            foreach (var res in restaurants)
            {
                Console.WriteLine($"Restaurant: {res.RestaurantName} ({res.RestaurantId})");
                res.DisplayMenu();
            }
        }
        //========================================================== 
        // Student Number : S10272963E
        // Student Name : Yap Jia Xuan 
        // Partner Name : Esther Teo Hui Min
        //==========================================================
        static void CreateNewOrder(List<Customer> customerList,List<Restaurant> restaurantlist)
        {
            Console.WriteLine("Create New Order");
            Console.WriteLine("================ ");

            Console.WriteLine("Enter Customer Email:");
            string email = Console.ReadLine();
            Customer customer = customerList .Find(c => c.emailAddress == email);
            if (customer == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            Console.WriteLine("Enter Restaurant ID:");
            string resId = Console.ReadLine();
            Restaurant restaurant = restaurantlist.Find(r => r.RestaurantId == resId);

            Console.WriteLine("Enter Delivery Date (dd/mm/yyyy):");
            string dateinput = Console.ReadLine();

            Console.WriteLine("Enter Delivery Time (hh:mm)");
            string timeinput = Console.ReadLine();
            DateTime deliverydatetime = DateTime.Parse(dateinput + " " + timeinput);

            Console.WriteLine("Enter Delivery Address:");
            string address = Console.ReadLine();

            Console.WriteLine("\nAvaibale Food Items:");
            int i = 0;
            foreach (i < restaurant.Menu.Count; i++)
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
                OrderFoodItem ordered = new OrderFoodItem(chosen, quantity);
                selecteditem.Add(ordered);

                subtotal += ordered.CalculateSubtotal();
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
                    neworder.AddOrderedFoodItem(item);
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
        static void ModifyOrder(List<Customer> customerList, List<Restaurant> restaurantList,List<Order> orders)
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
            order.DisplayOrderedFoodItem();

            Console.WriteLine($"Address: {order.DeliveryAddress}");
            Console.WriteLine($"Delivery Date/Time: {order.deliveryDateTime:dd/MM/yyyy, HH:mm}");

            Console.WriteLine("Modify: [1] Items [2] Address [3] Delivery Time: ");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ModifyOrder(customerList,restaurantList,orders);
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
                DateTime current = order.deliveryDateTime;
                TimeSpan time = TimeSpan.Parse(newtime);
                order.deliveryDateTime = current.Date + time;
                Console.WriteLine($"Order {order.OrderId} updated. New Delivery Time:{newtime}");
            }

            //Console.WriteLine($"Order {order.OrderId} updated. New Delivery Time:{newtime}");

        }
    }
}

