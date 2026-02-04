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
    public abstract class FoodItem

    {
       //public properties
        public string itemName {  get; set; }
        public string itemDesc { get; set; }
        public double itemPrice { get; set; }
        public string customise { get; set; }

        //constructor
        public FoodItem (string itemname, string itemdesc, double itemprice,string Customise)
        {
            itemName = itemname;
            itemDesc = itemdesc;
            itemPrice = itemprice;
            customise = Customise;
        }
        
        //methods
        public override string ToString()
        {
            return "Food item" + itemName + itemDesc + itemPrice + customise;
        }
    }
}
