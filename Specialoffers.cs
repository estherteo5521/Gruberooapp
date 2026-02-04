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
    public class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string OfferDesc { get; set; }
        public double Discount { get; set; }

        public SpecialOffer(string code, string desc, double discount)
        {
            OfferCode = code;
            OfferDesc = desc;
            Discount = discount;
        }

        public override string ToString()
        {
            return $"{OfferCode}: {OfferDesc} ({Discount * 100}% off)";
        }
    }


}
