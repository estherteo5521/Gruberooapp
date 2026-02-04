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
    internal class Customer
    {
        //public propeties
        public string emailAddress {  get; set; }
        public string customerName { get; set; }

        //create list for orderlist
        public List<Order> orderList
        {
            get { return orderList; }
            set { orderList = value;}
        }

        //constructor
        public Customer (string custemail, string custname)
        {
            emailAddress = custemail;
            customerName = custname;
        }
        
        //method
        public void AddOrder(Order order)
        {
            orderList.Add (order);
        }
        public void DisplayAllOrders()
        {
            Console.WriteLine($"Order History for:{customerName} ({emailAddress})");
            if (orderList.Count == 0)
            {
                Console.WriteLine("No orders found for this customer.");
            }
            else
            {
                foreach (Order order in orderList)
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
            bool removed = orderList.Remove(order);
            return removed;
        }
        public override string ToString()
        {
            return "Customer:" + customerName + emailAddress;
        }
    }
}
