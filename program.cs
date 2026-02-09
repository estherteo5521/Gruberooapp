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
                
                //create order object
                Order newOrder = new Order (id,orderdatetime,total,status,deliverydatetime,adddress,status,orderPaid);
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
                        //ListAllRestaurantsAndMenu();
                        break;
                    case "2":
                        //ListAllOrders();
                        break;
                    case "3":
                        // CreateNewOrder();
                        break;
                    case "4":
                        // ProcessOrders();
                        break;
                    case "5":
                        //ModifyOrder();
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
    }
}

