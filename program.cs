using Gruberooapp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;


namespace Gruberoo
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
                string paymentmethod = "Credit Card";

                //create order object
                Order newOrder = new Order(id, orderdatetime, total, status, deliverydatetime, adddress, paymentmethod, orderPaid);
                orderlist.Add(newOrder);
                

                //link to customerslist
                foreach (Customer c in customerslist)
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
                        ListAllRestaurantAndMenu(restaurants,foodItems);
                        break;
                    case "2":
                        //ListAllOrders();
                        break;
                    case "3":
                        CreateNewOrder(customersList,restaurantlist);
                        break;
                    case "4":
                        // ProcessOrders();
                        break;
                    case "5":
                        ModifyOrder(customersList,orderList);
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
        static void ListAllRestaurantAndMenu(List<Restaurant> restaurants,List<FoodItem> foodItems )
        {
            string[] lines = File.ReadAllLines("fooditems.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');
                //string resid= data[0];
                string itemname = data[1];
                string itemdesc = data[2];
                double itemprice = double.Parse(data[3]);
                string cust = " ";
                int qty = 0;
                double subtotal = 0.0;
            

                FoodItem foodItem = new OrderFoodItem(itemname,itemdesc,itemprice,cust,qty,subtotal);
                foodItems.Add(foodItem);
            }

            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("============================== ");
            Console.WriteLine($"Debug: {foodItems.Count}");


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
            for (int i = 1; i < lines.Length; i ++)
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
                OrderFoodItem ordered = new OrderFoodItem(chosen.itemName,chosen.itemDesc,chosen.itemPrice," ", quantity, 0.0);
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
        static void ModifyOrder(List<Customer> customerList,List<Order> orderList)
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
            foreach (Order o in customer.orderlist)
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
            Order order = customer.orderlist.Find(o => o.OrderId == orderid && o.OrderStatus == "Pending");
            if (order == null)
            {
                Console.WriteLine("Invalid Order ID / Order not pending.");
                return;
            }

            Console.WriteLine("Order Items:");
            //order.DisplayOrderedFoodItem();

            Console.WriteLine($"Address: {order.DeliveryAddress}");
            Console.WriteLine($"Delivery Date/Time: {order.deliveryDateTime:dd/MM/yyyy, HH:mm}");

            Console.WriteLine("Modify: [1] Items [2] Address [3] Delivery Time: ");
            Console.WriteLine();
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                ModifyOrder(customerList,orderList);
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

