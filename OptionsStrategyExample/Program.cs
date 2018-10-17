using System;
using System.Collections.Generic;
using System.Linq;
using RediLib;
using CommandLine;
namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    class Program
    {
        private Options options;
      
        private IStrategy strategy = null;
        private bool VerifyArguments()
        {
            bool ret = true;

            if(strategy.VerifyArguments() == false)
            {
                ret = false;
            }
           
           
            //Verify if the value of Price Type is valid. Otherwise, the application will exit
            if (!Utils.PriceTypeList.Contains(options.PriceType))
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
            strategy = StrategyFactory.Create(options.Strategy);
            if (strategy == null)
            {               
                Console.WriteLine("Invalid Value ({0}):\n\t --strategy          The strategy for the spread option (Vertical, Ratio, Butterfly, or Buy-Write)", options.Strategy);
                return;
            }
            strategy.SetOptions(options);
            if (VerifyArguments() == false)
            {
                return;
            }
            
            strategy.SendOrder();
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
