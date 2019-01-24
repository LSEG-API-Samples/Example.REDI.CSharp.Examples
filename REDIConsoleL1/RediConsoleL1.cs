using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using RediLib;


namespace RediConsoleL1
{
    class DisplayRediL1Quotes
    {

        private RediLib.CacheControl quoteCacheControl;

        
        public enum CacheControlActions
        {
            Snapshot = 1,
            Add = 4,
            Update = 5,
            Delete = 8
        }

        public enum WatchType
        {
            L1,
            L2,
            PANDL,
            FIRM,
            L1OPT,
            TimesAndSales,
            HistoricalTimesAndSales
        }

        Dictionary<string, QuoteCache> QuotesDict = new Dictionary<string, QuoteCache>();
        List<string> myRICList = new List<string>(new string[] { "GOOG", "BA" ,"MSFT"});

        public bool Init()
        {
            try
            {    
                foreach (string i in myRICList)
                {
                    if (!QuotesDict.ContainsKey(i))
                    {
                        quoteCacheControl = new CacheControl();
                        QuoteCache qCache = new QuoteCache(quoteCacheControl, i);
                        qCache.Subscribe();
                        QuotesDict.Add(i, qCache);
                    }
                } 
                return true;
            }
            catch (System.Runtime.InteropServices.COMException come)
            {
                Console.WriteLine("\nException <<< IS REDIPlus RUNNING? >>>\n\n" + come);
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadLine();
                return false;
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("...Start of RediConsoleL1...");
            DisplayRediL1Quotes myL1 = new DisplayRediL1Quotes();
            if (myL1.Init())
            {
                Console.WriteLine("...Init completed...");

               while (true)
                {  //
                }
            }
        }
    }
    
}

