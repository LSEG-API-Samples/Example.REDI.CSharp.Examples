using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RediLib;
using CommandLine;
namespace REDI.Csharp.Examples.VerticalOptionsTrade
{
    class Program
    {
        private Options options;
        private List<string> typeList = new List<string> { "Call", "Put" };
        private List<string> postionList = new List<string> { "Open", "Close" };
        private List<string> sideList = new List<string> { "Buy", "Sell" };
        private List<string> priceTypeList = new List<string> {"Limit", "Market"};

        private bool VerifyArguments()
        {
            bool ret = true;

            //Verify if the value of Type of the first leg is equal to Call or Put. Otherwise, the application will exit
            if (!typeList.Contains(options.Type1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --type1             (Default: Call) Options Type for the first leg (Call or Put)", options.Type1);
            }
            //Verify if the value of Type of the second leg is equal to Call or Put. Otherwise, the application will exit
            if (!typeList.Contains(options.Type2))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --type2             (Default: Call) Options Type of the first leg (Call or Put)", options.Type2);
            }

            //Verify if the value of Position of the first leg is equal to Open or Close. Otherwise, the application will exit
            if (!postionList.Contains(options.Position1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --position1     (Default: Open) Options order position of the first (Open or Close)", options.Position1);
            }
            //Verify if the value of Position of the second leg is equal to Open or Close. Otherwise, the application will exit
            if (!postionList.Contains(options.Position2))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --position2     (Default: Open) Options order position of the second (Open or Close)", options.Position2);
            }

            //Verify if the value of Side of the first leg is equal to Buy or Sell. Otherwise, the application will exit
            if (!sideList.Contains(options.Side1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --side1         (Default: Buy) Side of the first leg (Buy or Sell)", options.Side1);
            }

            //Verify if the value of Side of the second leg is equal to Buy or Sell. Otherwise, the application will exit
            if (!sideList.Contains(options.Side2))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --side2         (Default: Sell) Side of the first leg (Buy or Sell)", options.Side2);
            }
            //Verify if the value of Price Type is valid. Otherwise, the application will exit
            if (!priceTypeList.Contains(options.PriceType))
            {
                ret = false;
                Console.WriteLine("Invalid Value {0}:\n\t -p, --pricetype    (Default: Limit) Order type of an order (Limit,Market)", options.PriceType);
            }

            //Verify the requirement of price (-l, --limitprice) or stop price (-t, --stopprice) according to the price type
            switch (options.PriceType)
            {
                //When the price type is Limit or Limit Close, the limit price is required
                case "Limit":
                    if (options.Price == 0)
                    {
                        ret = false;
                        Console.WriteLine("Limit price ({0}) is required:\n\t -l, --limitprice    Limit Price of an order (Required by \"Limit\") ", options.Price);
                    }                    
                    break;

               
                //When the price type is Market or Market Close, limit price is ignored
                case "Market":
                    if (options.Price != 0)
                    {
                        Console.WriteLine("Limit price ({0}) will be ignored", options.Price);
                    }                 
                    break;

            }
            return ret;
        }
        public Program(Options op)
        {
            options = op;

        }
        public void Run()
        {
            if (VerifyArguments() == false)
            {
                return;
            }
            //If there is no expiration date for the first leg passed in the parameter, 
            //the application calls GetExpirateDate function to get the first valid expiration date according to the symbol
            if (string.IsNullOrEmpty(options.Date1))
            {
                string tmpDate = GetExpirationDate(options.Symbol);
                if (tmpDate == null)
                {
                    Console.WriteLine("Can't find expiration date for {0}", options.Symbol);
                    return;
                }
                options.Date1 = tmpDate;
            }
            //If there is no expiration date for the second leg passed in the parameter, 
            //the application calls GetExpirateDate function to get the first valid expiration date according to the symbol
            if (string.IsNullOrEmpty(options.Date2))
            {
                string tmpDate = GetExpirationDate(options.Symbol);
                if (tmpDate == null)
                {
                    Console.WriteLine("Can't find expiration date for {0}", options.Symbol);
                    return;
                }
                options.Date2 = tmpDate;
            }
            //If there is no strike price for the first leg passed in the parameter, 
            //the application calls GetStrikePrice function to get the first valid expiration date according to the symbol, type and expiration date
            if (string.IsNullOrEmpty(options.Strike1))
            {
                string tmpStrike = GetStrikePrice(options.Symbol, options.Type1, options.Date1);
                if (tmpStrike == null)
                {
                    Console.WriteLine("Can't find strike price for {0} {1} {2}", options.Symbol, options.Type1, options.Date1);
                    return;
                }
                options.Strike1 = tmpStrike;
            }
            //If there is no strike price for the second leg passed in the parameter, 
            //the application calls GetStrikePrice function to get the second valid expiration date according to the symbol, type and expiration date
            if (string.IsNullOrEmpty(options.Strike2))
            {
                string tmpStrike = GetStrikePrice(options.Symbol, options.Type2, options.Date2, 1);
                if (tmpStrike == null)
                {
                    Console.WriteLine("Can't find strike price for {0} {1} {2}", options.Symbol, options.Type2, options.Date2);
                    return;
                }
                options.Strike2 = tmpStrike;
            }
            //If there is no account passed in the parameter, 
            //the application calls GetAccount function to get the first available account
            if (string.IsNullOrEmpty(options.Account))
            {
                string tmpAccount = GetAccount();
                if (tmpAccount == null)
                {
                    Console.WriteLine("Can't find account");
                    return;
                }
                options.Account = tmpAccount;
            }
            ////If there is no account for the second leg passed in the parameter, 
            ////the application calls GetAccount function to get the first available account
            //if (string.IsNullOrEmpty(options.Account2))
            //{
            //    string tmpAccount = GetAccount();
            //    if (tmpAccount == null)
            //    {
            //        Console.WriteLine("Can't find account");
            //        return;
            //    }
            //    options.Account2 = tmpAccount;
            //}

            SendOrder();
        }
        private string GetStrikePrice(string symbol, string type, string expirationDate, int index = 0)
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
        private string GetAccount()
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
        private string GetExpirationDate(string symbol)
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
        private void PrintOrder()
        {
            Console.WriteLine("\nSend a vertical spread order with the following options:");          
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
                options.Date2,
                options.Strike2.ToString());         
            
            
           
        }
        private void SendOrder()
        {
            COMPLEXORDER objOrder = new COMPLEXORDER();            
            PrintOrder();
            objOrder.Strategy = "Vertical";

            objOrder.SetSymbol(0, options.Symbol);
            objOrder.SetQuantity(0, options.Quantity.ToString());
            objOrder.SetExchange(0, options.Exchange);
            objOrder.SetPriceType(0, options.PriceType);
            objOrder.SetAccount(0, options.Account);
            objOrder.SetTIF(0, options.TIF);



            //First Leg
            objOrder.SetSide(1, options.Side1);
            objOrder.SetPosition(1, options.Position1);
            objOrder.SetOptType(1, options.Type1);
            objOrder.SetMonth(1, options.Date1);
            objOrder.SetStrike(1, options.Strike1.ToString());
            objOrder.SetAccount(1, options.Account);

            //Second Leg
            objOrder.SetSide(2, options.Side2);
            objOrder.SetPosition(2, options.Position2);
            objOrder.SetOptType(2, options.Type2);
            objOrder.SetMonth(2, options.Date2);
            objOrder.SetStrike(2, options.Strike2.ToString());
            objOrder.SetAccount(2, options.Account);

            //Set limit price when the price type is limit
            if (options.PriceType == "Limit")
                objOrder.SetPrice(0, options.Price.ToString());
            object ord_err = null;
            bool status;
            status = objOrder.Submit(ref ord_err);

            if (!status)
            {
                Console.WriteLine($"Error: {(string)ord_err}");
                string error = (string)ord_err;
            }
            else
            {
                Console.WriteLine("Order has been submitted properly");
            }
        }
        static void Main(string[] args)
        {
            Options options = null;

            //Use CommandLineParser to parse the arguments
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
     .WithParsed<Options>(opts => { options = opts; });

            if (options == null) return;
            try
            {
                Program prog = new Program(options);
                prog.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
