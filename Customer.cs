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
    public class Customer
    {
        //public propeties
        public string emailAddress {  get; set; }
        public string customerName { get; set; }
        

        //create list for orderlist
        public List<Order> orderList { get; set; } = new List<Order>();
        //constructor
        public Customer (string custname, string custemail)
        {
            emailAddress = custemail;
            customerName = custname;
        }
        
        //method
        public void AddOrder(Order order)
        {
            orderlist.Add (order);
        }
        public void DisplayAllOrders()
        {
            Console.WriteLine($"Order History for:{customerName} ({emailAddress})");
            if (orderlist.Count == 0)
            {
                Console.WriteLine("No orders found for this customer.");
            }
            else
            {
                foreach (Order order in orderlist)
                {
                    Console.WriteLine(order.ToString());
                    order.DisplayOrderedFoodItem();
                }
            }
        }

        public bool RemoveOrder(Order order)
        {
            if (order == null)
            {
                return false;
            }
            bool removed = orderlist.Remove(order);
            return removed;
        }
        public override string ToString()
        {
            return "Customer:" + customerName + emailAddress;
        }
    }
}
