using System;
using System.Linq;
using RediLib;

namespace RediConsoleL1
{
    // Auxiliary class that implements QuoteCache interaction per Symbol
    class QuoteCache 
    {
        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { this._symbol =  value; }
        }
        private readonly CacheControl quoteCache;
        private readonly Boolean _isOption;
        private object err = null;

        public QuoteCache(CacheControl qc, string Symbol, Boolean isOption)
        {
            quoteCache = qc;
            _symbol = Symbol;
            _isOption = isOption;
        }

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

        public void Subscribe()
        {
            if (Symbol != null)
            {
                err = null;
                quoteCache.CacheEvent += quoteCacheHandler;
                if (_isOption)  //is an option                 
                    quoteCache.AddWatch(WatchType.L1OPT, Symbol, null, ref err);
                else // is an equity
                    quoteCache.AddWatch(WatchType.L1, Symbol, null, ref err);
                if ((null != err) && ((int)err != 0))
                    Console.WriteLine("On AddWatch err=" + err);
            }

        }

        public void Submit()
        {
            err = null;
            quoteCache.Submit("L1", "", ref err);
            if ((null != err) && ((int)err != 0))
            {
                Console.WriteLine("On Submit err=" + err);
            }
        }

        public void Unsubscribe()
        {
            if (Symbol != null)
            {
                err = null;
                if (_isOption)
                   quoteCache.DeleteWatch(WatchType.L1OPT, Symbol, null, ref err);
                else
                   quoteCache.DeleteWatch(WatchType.L1, Symbol, null, ref err);
                if ((null != err) && ((int)err != 0))
                    Console.WriteLine("On DeleteWatch err=" + err);
            }

        }







        public static object GetCell(CacheControl cc, int row, string columnName, out int errorCode)
        {
            object value = null;
            object errCode = null;
            cc.GetCell(row, columnName, ref value, ref errCode);
            errorCode = (int)errCode;
            if (value != null)
            {
                return value;
            }
            return string.Empty;
        }

        private void quoteCacheHandler(int action, int row)
        {
            string Symbol;
            string Bid = "";
            string Ask = "";
            string Last = "";
            string LastTradeSize = "";
            string Volume = "";
            int errCode;

 
            if (row >= 0)
            {

                switch (action)
                {
                    case (int)CacheControlActions.Add:
                    case (int)CacheControlActions.Update:
                    case (int)CacheControlActions.Snapshot:
                        try
                        {
                            if (action == (int)CacheControlActions.Snapshot)
                                row = 0;

                            Symbol = GetCell(quoteCache, row, "Symbol", out errCode).ToString().TrimStart();
                            if (Symbol.Length == 0)
                            {
                                Symbol = GetCell(quoteCache, row, "DisplaySymbol", out errCode).ToString().TrimStart();
                            }

                            if (Symbol.Contains(' '))
                            {
                                Bid = GetCell(quoteCache, row, "BidPrice", out errCode).ToString();
                                Ask = GetCell(quoteCache, row, "AskPrice", out errCode).ToString();
                            }
                            else
                            {
                                Bid = GetCell(quoteCache, row, "Bid", out errCode).ToString();
                                Ask = GetCell(quoteCache, row, "Ask", out errCode).ToString();
                            }

                            Last = GetCell(quoteCache, row, "Last", out errCode).ToString();
                            LastTradeSize = GetCell(quoteCache, row, "LastTradeSize", out errCode).ToString();
                            Volume = GetCell(quoteCache, row, "Volume", out errCode).ToString();

                            if(Symbol.Trim().Length != 0)
                                Console.WriteLine("Symbol=" + Symbol + " Action=" + ((CacheControlActions)action).ToString()+ 
                                    " Bid="+Bid + " Ask="+Ask+" Last="+Last + " LastTradeSize=" + LastTradeSize + 
                                    " Volume=" + Volume);

                        }
                        catch
                        {
                        }
                        break;
                    default:
                        Symbol = GetCell(quoteCache, row, "Symbol", out errCode).ToString().TrimStart();
                        if (Symbol.Length == 0)
                        {
                            Symbol = GetCell(quoteCache, row, "DisplaySymbol", out errCode).ToString().TrimStart();
                        }

                        Console.WriteLine("Symbol=" + Symbol + " Action int=" + action);
                        break;                        
                }
            }
        }
        
    
    }
}
