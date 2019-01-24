using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using RediLib;
using System.Collections.Specialized;

namespace RediConsoleL1
{
    class QuoteCache //: INotifyPropertyChanged
    {
        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { this._symbol =  value; }
        }
        private readonly CacheControl quoteCache;
        private object err = null;


        //public event PropertyChangedEventHandler PropertyChanged;

        public QuoteCache(CacheControl qc, string Symbol)
        {
            quoteCache = qc;
            _symbol = Symbol;
        }

        public enum CacheControlActions
        {
            Snapshot = 1,
            Add = 4,
            Update = 5,
            Delete = 8
        }

        public void Subscribe()
        {
            if (Symbol != null)
            {
                err = null;
                if (Symbol.Contains(' '))
                {
                    quoteCache.CacheEvent += quoteCacheHandler;
                    quoteCache.AddWatch(WatchType.L1OPT, Symbol, null, ref err);
                    quoteCache.Submit("L1", Symbol, ref err);
                }
                else
                {
                    quoteCache.CacheEvent += quoteCacheHandler;
                    quoteCache.AddWatch(WatchType.L1, Symbol, null, ref err);
                    quoteCache.Submit("L1", "", ref err);
                }
            }

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
            string Bid;
            string Ask;
            string Last;
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

 //                           Console.WriteLine("Symbol=" + Symbol + " Action=" + ((CacheControlActions)action).ToString()); // + " Row=" + row.ToString());


                            Bid = "";
                            Ask = "";


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

                            /* var quote = _qw.Quotes.FirstOrDefault(X => X.Symbol == Symbol);

                             if (quote != null)
                             {
                                 quote.Bid = Bid;
                                 quote.Ask = Ask;
                                 quote.Last = Last;

                             } */
                            Console.WriteLine("Symbol=" + Symbol + " Action=" + ((CacheControlActions)action).ToString()+ 
                                " Bid="+Bid + " Ask="+Ask+" Last="+Last);


                        }
                        catch
                        {
                        }
                        break;

                }
            }
        }
        
    
    }
}
