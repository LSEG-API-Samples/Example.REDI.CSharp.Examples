using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RediLib;
using CommandLine;
namespace REDI.Csharp.Examples.SingleOptionsTrade
{
    class Program
    {
        private Options options;

        //Define the possible values for type, position, side, and price type
        //They are used by the VerifyArguments function to verify the values passed in the paramaters
        private List<string> typeList = new List<string> { "Call", "Put" };
        private List<string> postionList = new List<string> { "Open", "Close" };
        private List<string> sideList = new List<string> { "Buy", "Sell" };
        private List<string> priceTypeList = new List<string> {
            "Limit",
            "Stop",
            "Stop Limit",
            "Market Close",
            "Market",
            "Limit Close"};

        public Program(Options op)
        {
            options = op;

        }

        //Verify values in the parameters
        private bool VerifyArguments()
        {
            bool ret = true;

            //Verify if the value of Type is equal to Call or Put. Otherwise, the application will exit
            if (!typeList.Contains(options.Type))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t -t, --type         (Default: Call) Options Type (Call or Put)", options.Type);
            }

            //Verify if the value of Position is equal to Open or Close. Otherwise, the application will exit
            if (!postionList.Contains(options.Position))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t -o, --position     (Default: Open) Options order position (Open or Close)", options.Position);
            }

            //Verify if the value of Side is equal to Buy or Sell. Otherwise, the application will exit
            if (!sideList.Contains(options.Side))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t -d, --side         (Default: Buy) Side of an order (Buy or Sell)", options.Side);
            }

            //Verify if the value of Price Type is valid. Otherwise, the application will exit
            if (!priceTypeList.Contains(options.PriceType))
            {
                ret = false;
                Console.WriteLine("Invalid Value {0}:\n\t -r, --pricetype    (Default: Limit) Order type of an order (Limit, Stop, Stop Limit, Market Close, Market, Limit Close)", options.PriceType);
            }

            //Verify the requirement of price (-l, --limitprice) or stop price (-t, --stopprice) according to the price type
            switch (options.PriceType)
            {
                //When the price type is Limit or Limit Close, the limit price is required and the stop price will be ignored 
                case "Limit":
                case "Limit Close":
                    if(options.Price == 0)
                    {
                        ret = false;
                        Console.WriteLine("Limit price ({0}) is required:\n\t -l, --limitprice    Limit Price of an order (Required by \"Limit\", \"Stop Limit\", \"Limit Close\")", options.Price);
                    }
                    if(options.StopPrice != 0)
                    {
                        Console.WriteLine("Stop price ({0}) will be ignored.", options.StopPrice);
                    }
                    break;

                //When the price type is Stop, the stop price is required and the limit price will be ignored 
                case "Stop":
                    if (options.StopPrice == 0)
                    {
                        ret = false;
                        Console.WriteLine("Stop price ({0}) is required:\n\t -t, --stop         Stop Price of an order (Required by \"Stop\", \"Stop Limit\")", options.StopPrice);
                    }
                    if (options.Price != 0)
                    {
                        Console.WriteLine("Limit price ({0}) will be ignored.", options.Price);
                    }
                    break;

                //When the price type is Stop Limit, both the stop price and limit price are required
                case "Stop Limit":
                    if (options.Price == 0)
                    {
                        ret = false;
                        Console.WriteLine("Limit price ({0}) is required:\n\t -l, --limitprice    Limit Price of an order (Required by \"Limit\", \"Stop Limit\", \"Limit Close\")", options.Price);

                    }
                    if (options.StopPrice == 0)
                    {
                        ret = false;
                        Console.WriteLine("Stop price ({0}) is required:\n\t -t, --stop         Stop Price of an order (Required by \"Stop\", \"Stop Limit\")", options.StopPrice);
                    }
                  
                    break;

                //When the price type is Market or Market Close, both the stop price and limit price are ignored
                case "Market":
                case "Market Close":
                    if (options.Price != 0)
                    {
                        Console.WriteLine("Limit price ({0}) will be ignored", options.Price);
                    }
                    if (options.StopPrice != 0)
                    {
                        //ret = false;
                        Console.WriteLine("Stop price ({0}) will be ignored", 
                            options.StopPrice);                        
                    }
                    break;

            }
            return ret;
        }
        public void Run()
        {
            
            if (VerifyArguments() == false)
            {
                return;
            }

            //If there is no expiration date passed in the parameter, 
            //the application calls GetExpirateDate function to get the first valid expiration date according to the symbol
            if (string.IsNullOrEmpty(options.Date))
            {
                string tmpDate = GetExpirationDate(options.Symbol);
                if(tmpDate == null)
                {
                    Console.WriteLine("Can't find expiration date for {0}", options.Symbol);
                    return;
                }
                options.Date = tmpDate;
            }

            //If there is no strike price passed in the parameter, 
            //the application calls GetStrikePrice function to get the first valid expiration date according to the symbol, type and expiration date
            if (string.IsNullOrEmpty(options.Strike))
            {
                string tmpStrike = GetStrikePrice(options.Symbol, options.Type, options.Date);
                if (tmpStrike == null )
                {
                    Console.WriteLine("Can't find strike price for {0} {1} {2}", options.Symbol, options.Type, options.Date);
                    return;
                }
                options.Strike = tmpStrike;
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

            SendOrder();
        }
        //Call OPTIONORDER.GetAccountCount and OPTIONORDER.GetAccountAt methods to get the first available account
        private string GetAccount()
        {
            OPTIONORDER objOrder = new OPTIONORDER();
            object objAccountCount = null;
            objOrder.GetAccountCount(ref objAccountCount);
            if(objAccountCount == null || (int)objAccountCount == 0)
            {
                return null;
            }
            else
            {              
                return (string)objOrder.GetAccountAt(0);
            }            
        }


        //Call OPTIONORDER.GetStrikesCount and OPTIONORDER.GetStrikeAt methods 
        //to get the first available stike price according to the symbol, type and expiration date
        private string GetStrikePrice(string symbol, string type, string expirationDate)
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
                return (string)objOrder.GetStrikeAt(0);
            }
        }
        //Call OPTIONORDER.GetExpirationDatesCount and OPTIONORDER.GetExpirationDateAt methods 
        //to get the first available expiration date  according to the symbol, type and expiration date
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
        //Print all options used when sending an order
        private void PrintOrder()
        {
            Console.WriteLine("\nSend an order with the following options:");
            Console.WriteLine("Symbol: {0}", options.Symbol);
            Console.WriteLine("PriceType: {0}", options.PriceType);

            //Print Limit Price or Stop Price according to price type
            switch (options.PriceType)
            {
                case "Limit":
                case "Limit Close":
                    Console.WriteLine("Limit Price: {0}", options.Price.ToString());
                    break;
                case "Stop":
                    Console.WriteLine("Stop Price:{0}", options.StopPrice.ToString());
                    break;
                case "Stop Limit":
                    Console.WriteLine("Limit Price: {0}", options.Price.ToString());
                    Console.WriteLine("Stop Price:{0}", options.StopPrice.ToString());
                    break;              

            }
            Console.WriteLine("Quantity: {0}", options.Quantity.ToString());            
            Console.WriteLine("Type: {0}", options.Type);
            Console.WriteLine("Date: {0}", options.Date);
            Console.WriteLine("Strike: {0}", options.Strike);
            Console.WriteLine("Position: {0}", options.Position);
            Console.WriteLine("Exchange: {0}", options.Exchange);            
            Console.WriteLine("TIF: {0}", options.TIF);
            Console.WriteLine("Account: {0}", options.Account);
            Console.WriteLine("Ticket: {0}", options.Ticket);
            Console.WriteLine("==============================");
        }
        //Submit an order to REDI
        private void SendOrder()
        {
            OPTIONORDER objOrder = new OPTIONORDER();

            PrintOrder();

            objOrder.Symbol = options.Symbol;
            objOrder.PriceType = options.PriceType;
            objOrder.Quantity = options.Quantity.ToString();

            //Set the limit price or stop price according to the price type
            switch (options.PriceType)
            {
                case "Limit":
                case "Limit Close":
                    objOrder.Price = options.Price.ToString();
                    break;
                case "Stop":
                    objOrder.StopPrice = options.StopPrice.ToString();
                    break;
                case "Stop Limit":
                    objOrder.Price = options.Price.ToString();
                    objOrder.StopPrice = options.StopPrice.ToString();
                    break;

            }         
            objOrder.type = options.Type;
            objOrder.Date = options.Date;          
            objOrder.Strike = options.Strike;           
            objOrder.Position = options.Position;         
            objOrder.Side = options.Side;            
            objOrder.Exchange = options.Exchange;            
            objOrder.TIF = options.TIF;
            objOrder.Account = options.Account;
            objOrder.Ticket = options.Ticket;
           
            object ord_err = null;
            bool status;

            //Submit an order
            status = objOrder.Submit(ref ord_err);
            if (!status)
            {                
                Console.WriteLine("Error: {0}", (string)ord_err);
                string error = (string)ord_err;

                //If the error is "Invalid Date.", show the first availble expiration date
                if(error == "Invalid Date.")
                {
                    Console.WriteLine("The first valid expiration date is: \"{0}\". Please try with this expiration date.", GetExpirationDate(options.Symbol));                   
                }
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
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
