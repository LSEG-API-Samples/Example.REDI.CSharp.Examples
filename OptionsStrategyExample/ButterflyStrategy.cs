using RediLib;
using System;

namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    class ButterflyStrategy : IStrategy
    {
        Options options;
        private string name = "Butterfly";

        public void SendOrder()
        {
            //If there is no expiration date for the first leg passed in the parameter, 
            //the application calls GetExpirateDate function to get the first valid expiration date according to the symbol
            if (string.IsNullOrEmpty(options.Date1))
            {
                string tmpDate = Utils.GetExpirationDate(options.Symbol);
                if (tmpDate == null)
                {
                    Console.WriteLine("Can't find expiration date for {0}", options.Symbol);
                    return;
                }
                options.Date1 = tmpDate;
            }
           
            //If there is no strike price for the first leg passed in the parameter, 
            //the application calls GetStrikePrice function to get the first valid expiration date according to the symbol, type and expiration date
            if (string.IsNullOrEmpty(options.Strike1))
            {
                string tmpStrike = Utils.GetStrikePrice(options.Symbol, options.Type1, options.Date1);
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
                string tmpStrike = Utils.GetStrikePrice(options.Symbol, options.Type1, options.Date1, 1);
                if (tmpStrike == null)
                {
                    Console.WriteLine("Can't find strike price for {0} {1} {2}", options.Symbol, options.Type1, options.Date1);
                    return;
                }
                options.Strike2 = tmpStrike;
            }

            if (string.IsNullOrEmpty(options.Strike3))
            {
                string tmpStrike = Utils.GetStrikePrice(options.Symbol, options.Type1, options.Date1, 2);
                if (tmpStrike == null)
                {
                    Console.WriteLine("Can't find strike price for {0} {1} {2}", options.Symbol, options.Type1, options.Date1);
                    return;
                }
                options.Strike3 = tmpStrike;
            }
            //If there is no account passed in the parameter, 
            //the application calls GetAccount function to get the first available account
            if (string.IsNullOrEmpty(options.Account))
            {
                string tmpAccount = Utils.GetAccount();
                if (tmpAccount == null)
                {
                    Console.WriteLine("Can't find account");
                    return;
                }
                options.Account = tmpAccount;
            }
            SendOrderInt();

        }
        private void SendOrderInt()
        {
            COMPLEXORDER objOrder = new COMPLEXORDER();
            PrintOrder();
            objOrder.Strategy = name;

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
            objOrder.SetOptType(2, options.Type1);
            objOrder.SetMonth(2, options.Date1);
            objOrder.SetStrike(2, options.Strike2.ToString());
            objOrder.SetAccount(2, options.Account);

            //Third Leg
            objOrder.SetSide(3, options.Side3);
            objOrder.SetPosition(3, options.Position3);
            objOrder.SetOptType(3, options.Type1);
            objOrder.SetMonth(3, options.Date1);
            objOrder.SetStrike(3, options.Strike3.ToString());
            objOrder.SetAccount(3, options.Account);

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

        public void SetOptions(Options options)
        {
            this.options = options;
        }

        public bool VerifyArguments()
        {
            bool ret = true;

            if (options == null)
            {
                Console.WriteLine("Can't find the application arguments, please call the SetOptions() method");
                return false;
            }
            //Verify if the value of Type of the first leg is equal to Call or Put. Otherwise, the application will exit
            if (!Utils.TypeList.Contains(options.Type1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --type1             (Default: Call) Options Type for the first leg (Call or Put)", options.Type1);
            }


            //Verify if the value of Position of the first leg is equal to Open or Close. Otherwise, the application will exit
            if (!Utils.PostionList.Contains(options.Position1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --position1     (Default: Open) Options order position of the first (Open or Close)", options.Position1);
            }
            //Verify if the value of Position of the second leg is equal to Open or Close. Otherwise, the application will exit
            if (!Utils.PostionList.Contains(options.Position2))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --position2     (Default: Open) Options order position of the second (Open or Close)", options.Position2);
            }
            if (!Utils.PostionList.Contains(options.Position3))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --position3     (Default: Open) Options order position of the second (Open or Close)", options.Position2);
            }
            //Verify if the value of Side of the first leg is equal to Buy or Sell. Otherwise, the application will exit
            if (!Utils.SideList.Contains(options.Side1))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --side1         (Default: Buy) Side of the first leg (Buy or Sell)", options.Side1);
            }

            //Verify if the value of Side of the second leg is equal to Buy or Sell. Otherwise, the application will exit
            if (!Utils.SideList.Contains(options.Side2))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --side2         (Default: Sell) Side of the first leg (Buy or Sell)", options.Side2);
            }

            //Verify if the value of Side of the second leg is equal to Buy or Sell. Otherwise, the application will exit
            if (!Utils.SideList.Contains(options.Side3))
            {
                ret = false;
                Console.WriteLine("Invalid Value ({0}):\n\t --side3         (Default: Sell) Side of the first leg (Buy or Sell)", options.Side2);
            }
            return ret;
        }
        private void PrintOrder()
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
            Console.WriteLine("Leg 1: 1 {0} {1} {2} {3} {4}",
                options.Type1,
                options.Side1,
                options.Position1,
                options.Date1 == null ? "" : options.Date1,
                options.Strike1 == null ? "" : options.Strike1.ToString());
            Console.WriteLine("Leg 2: 2 {0} {1} {2} {3} {4}",
                options.Type1,
                options.Side2,
                options.Position2,
                options.Date1 == null ? "" : options.Date1,
                options.Strike2 == null ? "" : options.Strike2.ToString());
            Console.WriteLine("Leg 3: 1 {0} {1} {2} {3} {4}",
                options.Type1,
                options.Side3,
                options.Position3,
                options.Date1 == null ? "" : options.Date1,
                options.Strike3 == null ? "" : options.Strike3.ToString());

        }
    }
}

