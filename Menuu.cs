//==========================================================
// Student Number : S10275496C
// Student Name : Esther Teo
// Partner Name : Yap Jia Xuan 
//==========================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Gruberooapp
{
    public class Menu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }

        public List<FoodItem> foodItems = new List<FoodItem>();

        public Menu(string id, string name)
        {
            MenuId = id;
            MenuName = name;
        }

        public int FoodItemCount => foodItems.Count;

        public void AddFoodItem(FoodItem item)
        {
            if (item != null)
            {
                foodItems.Add(item);
            }
        }

        public bool RemoveFoodItem(string itemName)
        {
            FoodItem item = foodItems.Find(f => f.itemName == itemName);

            if (item != null)
            {
                foodItems.Remove(item);
                return true;
            }
            return false;
        }

        public void DisplayFoodItems()
        {
            foreach (FoodItem item in foodItems)
            {
                Console.WriteLine(item);
            }
        }

        public override string ToString()
        {
            return $"Menu: {MenuName} ({MenuId})";
        }
    }
}


