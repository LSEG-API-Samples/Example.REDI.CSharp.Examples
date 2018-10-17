using RediLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    static class Utils
    {
        static public List<string> TypeList = new List<string> { "Call", "Put" };
        static public List<string> PostionList = new List<string> { "Open", "Close" };
        static public List<string> SideList = new List<string> { "Buy", "Sell" };
        static public List<string> PriceTypeList = new List<string> { "Limit", "Market"};
        static public string GetStrikePrice(string symbol, string type, string expirationDate, int index = 0)
        {
            OPTIONORDER objOrder = new OPTIONORDER();
            objOrder.Symbol = symbol;
            objOrder.type = type;
            objOrder.Date = expirationDate;

            object objStrikeCount = null;
            objOrder.GetStrikesCount(ref objStrikeCount);

            if (objStrikeCount == null || (int)objStrikeCount == 0)
            {
                return null;
            }
            else
            {
                return (string)objOrder.GetStrikeAt(index);
            }
        }
        static public string GetAccount()
        {
            OPTIONORDER objOrder = new OPTIONORDER();
            object objAccountCount = null;
            objOrder.GetAccountCount(ref objAccountCount);
            if (objAccountCount == null || (int)objAccountCount == 0)
            {
                return null;
            }
            else
            {
                return (string)objOrder.GetAccountAt(0);
            }
        }
        static public string GetExpirationDate(string symbol)
        {
            OPTIONORDER objOrder = new OPTIONORDER();
            objOrder.Symbol = symbol;

            object objExpirationCount = null;
            objOrder.GetExpirationDatesCount(ref objExpirationCount);
            if (objExpirationCount == null || (int)objExpirationCount == 0)
            {
                return null;
            }
            else
            {
                return (string)(objOrder.GetExpirationDateAt(0));
            }
        }
        static public void PrintOrder(Options options)
        {
            Console.WriteLine("\nSend a {0} spread order with the following options:", options.Strategy);
            Console.WriteLine("Symbol: {0}", options.Symbol);
            Console.WriteLine("PriceType: {0}", options.PriceType);
            Console.WriteLine("Quantity: {0}", options.Quantity.ToString());
            Console.WriteLine("Exchange: {0}", options.Exchange);
            Console.WriteLine("TIF: {0}", options.TIF);
            Console.WriteLine("Account: {0}", options.Account);

            //Print Limit Price according to price type
            switch (options.PriceType)
            {
                case "Limit":
                    Console.WriteLine("Limit Price: {0}", options.Price.ToString());
                    break;

            }
            Console.WriteLine("==============================");
            Console.WriteLine("Leg 1: {0} {1} {2} {3} {4}",
                options.Type1,
                options.Side1,
                options.Position1,
                options.Date1,
                options.Strike1.ToString());
            Console.WriteLine("Leg 2: {0} {1} {2} {3} {4}",
                options.Type2,
                options.Side2,
                options.Position2,
                options.Date2 == null?"":options.Date2,
                options.Strike2 == null?"":options.Strike2.ToString());



        }
    }
}
