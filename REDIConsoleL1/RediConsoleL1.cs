using System;
using System.Collections.Generic;
using RediLib;


namespace RediConsoleL1
{
    class DisplayRediL1Quotes
    {

        private RediLib.CacheControl quoteCacheControl;
   
        Dictionary<string, QuoteCache> QuotesDict = new Dictionary<string, QuoteCache>();
        // as options are soon outdated, if required, please replace the below option instrument with a valid option instrument
        List<string> myInstrumentList = new List<string>(new string[] { "GOOG", "MSFT", "BA", "SPX   211217C03800000" ,"AAPL US EQUITY"});
        List<string> myOptionsList = new List<string>(new string[] { "SPX   211217C03800000" });  //these are options included in myInstrumentList
        public bool Init()
        {
            try
            {    
                foreach (string i in myInstrumentList)
                {
                    if (!QuotesDict.ContainsKey(i))
                    {
                        quoteCacheControl = new CacheControl();
                        QuoteCache qCache = new QuoteCache(quoteCacheControl, i, myOptionsList.Contains(i));
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
                myL1.QuotesDict[myL1.myInstrumentList[0]].Submit();
                System.Threading.Thread.Sleep(5000);
                if (myL1.myInstrumentList.Count >= 2)
                {
                    System.Threading.Thread.Sleep(5000);
                    QuoteCache qc1 = myL1.QuotesDict[myL1.myInstrumentList[1]];
                    qc1.Unsubscribe();
                    qc1.Submit();
                    Console.WriteLine("...Deleted... " + myL1.myInstrumentList[1]);
                }
 /*               if (myL1.myInstrumentList.Count >= 2)
                {
                    System.Threading.Thread.Sleep(5000);
                    QuoteCache qc1 = myL1.QuotesDict[myL1.myInstrumentList[1]];
                    qc1.Subscribe();
                    qc1.Submit();
                    Console.WriteLine("...Subscribed... " + myL1.myInstrumentList[1]);
                } */
                while (true)
                {  //
                }
            }
        }
    }
    
}

