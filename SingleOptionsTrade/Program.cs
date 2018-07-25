using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RediLib;
using CommandLine;
namespace OptionsTrade
{
    class Program
    {
        private Options options;
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
        private bool VerifyArguments()
        {
            bool ret = true;

            if (!typeList.Contains(options.Type))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}): -t, --type         (Default: Call) Options Type (Call or Put)", options.Type);
            }
            if (!postionList.Contains(options.Position))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}): -o, --position     (Default: Open) Options order position (Open or Close)", options.Position);
            }
            if (!sideList.Contains(options.Side))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}): -d, --side         (Default: Buy) Side of an order (Buy or Sell)", options.Side);
            }
            if (!priceTypeList.Contains(options.PriceType))
            {
                ret = false;
                Console.WriteLine("Invalid Value {0}: -r, --pricetype    (Default: Limit) Order type of an order (Limit, Stop, Stop Limit, Market Close, Market, Limit Close)", options.PriceType);
            }
            switch (options.PriceType)
            {
                case "Limit":
                case "Limit Close":
                    if(options.Price == 0)
                    {
                        ret = false;
                        Console.WriteLine("Price is required {0}: -p, --price        Limit Price of an order", options.Price);
                    }
                    break;
                case "Stop":
                    if (options.StopPrice == 0)
                    {
                        ret = false;
                        Console.WriteLine("Stop price is required {0}: -t, --stop         Stop Price of an order", options.StopPrice);
                    }
                    break;
                case "Stop Limit":
                    if (options.StopPrice == 0 || options.Price == 0)
                    {
                        ret = false;
                        Console.WriteLine("Price is required {0}: -p, --price        Limit Price of an order", options.Price);
                        Console.WriteLine("Stop price is required {0}: -t, --stop         Stop Price of an order", options.StopPrice);
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
            if(string.IsNullOrEmpty(options.Date))
            {
                string tmpDate = GetExpirationDate(options.Symbol);
                if(tmpDate == null)
                {
                    Console.WriteLine("Can't find expiration date for {0}", options.Symbol);
                    return;
                }
                options.Date = tmpDate;
            }
           
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
            Console.WriteLine("Send an order with the following options:");
            Console.WriteLine("Symbol: {0}", options.Symbol);
            Console.WriteLine("Quantity: {0}", options.Quantity.ToString());
            Console.WriteLine("Price: {0}", options.Price.ToString());
            Console.WriteLine("Stop Price:{0}", options.StopPrice.ToString());
            Console.WriteLine("Type: {0}", options.Type);
            Console.WriteLine("Date: {0}", options.Date);
            Console.WriteLine("Strike: {0}", options.Strike);
            Console.WriteLine("Position: {0}", options.Position);
            Console.WriteLine("Exchange: {0}", options.Exchange);
            Console.WriteLine("PriceType: {0}", options.PriceType);
            Console.WriteLine("TIF: {0}", options.TIF);
            Console.WriteLine("Account: {0}", options.Account);
            Console.WriteLine("Ticket: {0}", options.Ticket);
            Console.WriteLine("==============================");
        }
        private void SendOrder()
        {
            OPTIONORDER objOrder = new OPTIONORDER();

            PrintOrder();

            objOrder.Symbol = options.Symbol;
            objOrder.Quantity = options.Quantity.ToString();
            objOrder.Price = options.Price.ToString();
            objOrder.StopPrice = options.StopPrice.ToString();           
            objOrder.type = options.Type;
            objOrder.Date = options.Date;          
            objOrder.Strike = options.Strike;           
            objOrder.Position = options.Position;         
            objOrder.Side = options.Side;            
            objOrder.Exchange = options.Exchange;
            objOrder.PriceType = options.PriceType;            
            objOrder.TIF = options.TIF;
            objOrder.Account = options.Account;
            objOrder.Ticket = options.Ticket;
           
            object ord_err = null;
            bool status;
            status = objOrder.Submit(ref ord_err);
            if (!status)
            {
                Console.WriteLine($"Error: {(string)ord_err}");
            }
            else
            {
                Console.WriteLine("Order has been submitted properly");
            }

        }
        static void Main(string[] args)
        {
            Options options = null;
            var result = CommandLine.Parser.Default.ParseArguments<Options>(args)
     .WithParsed<Options>(opts => { options = opts; });
           
            if (options == null) return;
            Program prog = new Program(options);
            prog.Run();
        }
    }
}
