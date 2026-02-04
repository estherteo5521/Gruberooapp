//========================================================== 
// Student Number : S10272963E
// Student Name : Yap Jia Xuan 
// Partner Name : Esther Teo Hui Min
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberooapp
{
    public class Order
    {
        //private properties
        private int orderId;
        private double orderTotal;
        private string orderStatus;
        private DateTime orderDateTime;
        
        //public propeties
        public int OrderId
        { 
            get { return orderId; } 
            set { orderId = value; } 
        }
        public string OrderStatus 
        { 
            get { return orderStatus; } 
            set { orderStatus = value; } 
        }
        
        //constructor
        public Order(int id, DateTime date, double total, string status)
        {
            orderId = id;
            orderDateTime = date;
            orderTotal = total;
            orderStatus = status;
        }
        //list for orderedfoooditems
        public List<string> orderedfooditem = new List<string>();

        //methods
        public double CalculateOrderTotal()
        {
            double total = 0;
            orderTotal = total + 5.00;
            return orderTotal;
        }
      
        public void AddOrderedFoodItem(string csv)
        {
            string[] items = csv.Split('|');
            foreach (string item in items)
            {
                orderedfooditem.Add(item.Trim());
            }
        }

        public bool RemoveOrderedFoodItem()
        {
            if (orderedfooditem.Count == 0)
            {
                Console.WriteLine("Order empty.");
                return false;
            }

            Console.WriteLine("Enter food name to remove:");
            string itemremove = Console.ReadLine();

            bool success = orderedfooditem.Remove(itemremove);
            if (success)
            {
                Console.WriteLine("Item removed successfully.");
                return true;
            }
            else
            {
                Console.WriteLine("Item not fount.Please enter food item name again.");
                return false;
            }
        }

        public int DisplayOrderedFoodItem()
        {
            Console.WriteLine("Display items for Order ID:"+orderId);
            foreach(string item in orderedfooditem)
            {
                Console.WriteLine("-"+item);
            }
            return orderedfooditem.Count;
        }

        public override string ToString()
        {
            return  "ID: " + orderId + ", Total: $" + orderTotal + ", Status: " + orderStatus;
        }
    }
}
