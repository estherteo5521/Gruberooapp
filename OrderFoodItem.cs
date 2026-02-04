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
    internal class OrderFoodItem:FoodItem
    {
        //public properties
        public int qtyOrdered {  get; set; }
        public double subTotal { get; set; }

        //constructor
        public OrderFoodItem(string itemname, string itemdesc, double itemprice, string Customise, int quantityorder, double subtotal) : base( itemname,itemdesc,itemprice,Customise)
        {
            qtyOrdered = quantityorder;
            subTotal = subtotal;
        }
        //method
        public double CalculateSubtotal ()
        {
            subTotal = qtyOrdered * itemPrice;
            return subTotal;
        }

       
    }
}

