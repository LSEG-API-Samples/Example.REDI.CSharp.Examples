using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RediLib;

/**
 * Example code 
 * class DisplayRediOrders
 * For TR -> REDI -> Monitor Orders In C# 
 * Tutorial
 * **/
namespace RediConsoleOrders
{
    class DisplayRediOrders
    {
        private CacheControl orderCache;
        object exec_err;

        public enum CacheControlActions
        {
            Snapshot = 1,
            Add = 4,
            Update = 5,
            Delete = 8
        }

        public bool init()
        {
            try { 
                orderCache = new CacheControl();    // create CacheControl 
                orderCache.CacheEvent += orderCacheHandler;  // register handler
                orderSubmit();    // this order submit
                return true;
            } catch (System.Runtime.InteropServices.COMException come)
            {
                Console.WriteLine("\nException <<< IS REDIPlus RUNNING? >>>\n\n" + come);
                Console.WriteLine("Press any key to continue . . .");
                Console.ReadLine();
                return false;
            }
        }

        public void orderSubmit()
        {
            // this message initializes interaction with Order cache
            // that should include the initial snapshot of the orders and consecutive deltas  
          object result = orderCache.Submit("Message", /*""*/"(msgtype == 10)", ref exec_err);
            // object result = orderCache.Submit("Message", /*""*/"(msgtype == 14)", ref exec_err);
        }

        static void Main(string[] args)
        {

            Console.WriteLine("Start of RediConsoleOrders");
            DisplayRediOrders myOrders = new DisplayRediOrders();
            if (myOrders.init()) {  
                Console.WriteLine("Init completed");  
                while (true)
                {
                }
            }
        }

        public static object GetCell(CacheControl cc, int row, string columnName, out int errorCode)
        {
            object value = null;
            object errCode = null;
            try
            {
                cc.GetCell(row, columnName, ref value, ref errCode);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message + " " + e.StackTrace);
            }
            errorCode = (int)errCode;
            if (value != null)
            {
                return value;
            }
            return string.Empty;
        }


        private void orderCacheHandler(int action, int row)
        // callback for OrderCache interaction
        {
            string Time;

            string Account;
            string Side;
            string Quantity;
            string ExecQuantity;
            string Symbol;
            string PriceDesc;
            string ExecPrice;
            string PctCmp;
            string Lvs;
            string Status;
            string OrderRefKey;
            String BranchCode;
            String BranchSeq;
            string Exchange;
            string Customs = "";

            int errCode;

//            Console.WriteLine("action: " + action.ToString() + "; row: " + row.ToString());

            if (row >= 0)
            {           
                //Console.WriteLine("action: " + action.ToString());
                 switch (action)
                {
                    case (int)CacheControlActions.Add: // new order is added to cache
 //                       Console.WriteLine("Add");
                        try
                        {
                            Time = GetCell(orderCache, row, "Time", out errCode).ToString().TrimStart();
                            Account = GetCell(orderCache, row, "Account", out errCode).ToString().TrimStart();
                            Side = GetCell(orderCache, row, "Side", out errCode).ToString().TrimStart();
                            Quantity = GetCell(orderCache, row, "Quantity", out errCode).ToString().TrimStart();
                            ExecQuantity = GetCell(orderCache, row, "ExecQuantity", out errCode).ToString().TrimStart();
                            Symbol = GetCell(orderCache, row, "DisplaySymbol", out errCode).ToString().TrimStart();
                            if ((Symbol.Contains("DEFAULT")) || (Symbol.Equals("")))
                                break; // do not display orders that resulated in errors on REDIPlus
                            PriceDesc = GetCell(orderCache, row, "PriceDesc", out errCode).ToString().TrimStart();
                            ExecPrice = GetCell(orderCache, row, "AvgExecPrice", out errCode).ToString().TrimStart();
                           if (((null == ExecQuantity) || (null == Quantity)) ||
                                    ((ExecQuantity.Equals("")) || (Quantity.Equals(""))))
                                break; // do not display orders with quantities not populated, wait for the valid order
                            decimal Pct = Convert.ToDecimal(ExecQuantity) / Convert.ToDecimal(Quantity);
                            if (ExecQuantity.Equals("0"))
                            {
                                PctCmp = "0.00%";
                            }
                            else
                            {
                                PctCmp = Pct.ToString() + " %";
                            }
                            Lvs = GetCell(orderCache, row, "Leaves", out errCode).ToString().TrimStart();
                            Status = GetCell(orderCache, row, "Status", out errCode).ToString().TrimStart();
                            OrderRefKey = GetCell(orderCache, row, "OrderRefKey", out errCode).ToString().TrimStart();
                            BranchCode = GetCell(orderCache, row, "BranchCode", out errCode).ToString().TrimStart();
                            BranchSeq = GetCell(orderCache, row, "BranchSeq", out errCode).ToString().TrimStart();
                            Customs = "";
                            for (int k = 0; k < 15; k++)
                            {
                                String cus = GetCell(orderCache, row, "Custom" + k, out errCode).ToString().TrimStart();
                                if (!cus.Equals(""))
                                    Customs += ("<" + k + "=" + cus + ">");
                            }


                            var Ord = new Order();
                            Time = Time.Remove(0, 9);
                            Ord.Time = Time;

                            Ord.Account = Account;
                            Ord.Side = Side;
                            Ord.Quantity = Quantity;
                            Ord.ExecQuantity = ExecQuantity;
                            Ord.Symbol = Symbol;
                            Ord.PriceDesc = PriceDesc;
                            Ord.ExecPr = ExecPrice;
                            Ord.PctCmp = PctCmp;
                            Ord.Lvs = Lvs;
                            Ord.Status = Status;
                            Ord.OrderRefKey = OrderRefKey;
                            Ord.BranchCode = BranchCode;
                            Ord.BranchSeq = BranchSeq;
                            Ord.Customs = Customs;

                            Console.WriteLine("Add to Orders: "+ Ord.ToString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception: " + e.Message + " " + e.StackTrace);
                        }
                        break;
                    case (int)CacheControlActions.Update:  // modification or cancellation
                        try
                        {
                            //                           Console.WriteLine("Update");
                            
                            Time = GetCell(orderCache, row, "Time", out errCode).ToString().TrimStart();
                            Account = GetCell(orderCache, row, "Account", out errCode).ToString().TrimStart();
                            Side = GetCell(orderCache, row, "Side", out errCode).ToString().TrimStart();
                            Quantity = GetCell(orderCache, row, "Quantity", out errCode).ToString().TrimStart();
                            ExecQuantity = GetCell(orderCache, row, "ExecQuantity", out errCode).ToString().TrimStart();
                            Symbol = GetCell(orderCache, row, "DisplaySymbol", out errCode).ToString().TrimStart();
                            PriceDesc = GetCell(orderCache, row, "PriceDesc", out errCode).ToString().TrimStart();
                            ExecPrice = GetCell(orderCache, row, "AvgExecPrice", out errCode).ToString().TrimStart();
                            //PctCmp = GetCell(orderCache, row, "ProgressValue", out errCode).ToString().TrimStart();
                            if (((null == ExecQuantity) || (null == Quantity)) ||
                                    ((ExecQuantity.Equals("")) || (Quantity.Equals(""))))
                                break; //do not display orders with quantities not populated, wait for the valid order
                            decimal Pct = Convert.ToDecimal(ExecQuantity) / Convert.ToDecimal(Quantity);
                            if (ExecQuantity.Equals("0"))
                            {
                                PctCmp = "0.00%";
                            }
                            else
                            {
                                PctCmp = Pct.ToString() + " %";
                            }
                            Lvs = GetCell(orderCache, row, "Leaves", out errCode).ToString().TrimStart();
                            Status = GetCell(orderCache, row, "Status", out errCode).ToString().TrimStart();
                            OrderRefKey = GetCell(orderCache, row, "OrderRefKey", out errCode).ToString().TrimStart();
                            BranchCode = GetCell(orderCache, row, "BranchCode", out errCode).ToString().TrimStart();
                            BranchSeq = GetCell(orderCache, row, "BranchSeq", out errCode).ToString().TrimStart();
                            Customs = "";
                            for (int k = 0; k < 15; k++)
                            {
                                String cus = GetCell(orderCache, row, "Custom" + k, out errCode).ToString().TrimStart();
                                if (!cus.Equals(""))
                                    Customs += ("<" + k + "=" + cus + ">");
                            }

                            var Ord = new Order();

                            Time = Time.Remove(0, 9);

                            //                       var order = Ord;
                            Ord.Time = Time;
                            Ord.Account = Account;
                            Ord.Side = Side;
                            Ord.Quantity = Quantity;
                            Ord.ExecQuantity = ExecQuantity;
                            Ord.Symbol = Symbol;
                            Ord.PriceDesc = PriceDesc;
                            Ord.ExecPr = ExecPrice;
                            Ord.PctCmp = PctCmp;
                            Ord.Lvs = Lvs;
                            Ord.Status = Status;
                            Ord.OrderRefKey = OrderRefKey;
                            Ord.BranchCode = BranchCode;
                            Ord.BranchSeq = BranchSeq;
                            Ord.Customs = Customs;
                            
                            //                                                       } 
                            Console.WriteLine("Update to Order: " + Ord);

                            /*                       if (long.Parse(Ord.ExecQuantity) > 0)
                                                   {
                                                       Console.WriteLine("Ord.ExecQuantity greater then 0");
                                                   } */

                        }
                        catch
                        {

                        }
                        break;

                    case (int)CacheControlActions.Snapshot:
                        // Snapshot of what happened throughout this day 
                        // expect this first
                        try
                        {
                            Console.WriteLine("Snapshot of orders:");
                            
                            for (int i = 0; i < row; i++)
                            {
                                Time = GetCell(orderCache, i, "Time", out errCode).ToString().TrimStart();
                                Account = GetCell(orderCache, i, "Account", out errCode).ToString().TrimStart();
                                Side = GetCell(orderCache, i, "Side", out errCode).ToString().TrimStart();
                                Quantity = GetCell(orderCache, i, "Quantity", out errCode).ToString().TrimStart();
                                ExecQuantity = GetCell(orderCache, i, "ExecQuantity", out errCode).ToString().TrimStart();
                                Symbol = GetCell(orderCache, i, "DisplaySymbol", out errCode).ToString().TrimStart();
                                PriceDesc = GetCell(orderCache, i, "PriceDesc", out errCode).ToString().TrimStart();
                                ExecPrice = GetCell(orderCache, i, "AvgExecPrice", out errCode).ToString().TrimStart();
                                Customs = "";
                                for (int k = 0; k < 15; k++)
                                {
                                    String cus = GetCell(orderCache, i, "Custom" + k, out errCode).ToString().TrimStart();
                                    if (!cus.Equals(""))
                                        Customs += ( "<"+ k + "=" + cus+">");
                                }
                                if (((null == ExecQuantity) || (null == Quantity)) ||
                                   ((ExecQuantity.Equals("")) || (Quantity.Equals(""))))
                                    continue; //do not display orders with quantities not populated, wait for the valid order
                                decimal Pct = 100 * Convert.ToDecimal(ExecQuantity) / Convert.ToDecimal(Quantity);
                                if (ExecQuantity.Equals("0"))
                                {
                                    PctCmp = "0.00%";
                                }
                                else
                                {
                                    PctCmp = Pct.ToString() + " %";
                                }
                                Lvs = GetCell(orderCache, i, "Leaves", out errCode).ToString().TrimStart();
                                Status = GetCell(orderCache, i, "Status", out errCode).ToString().TrimStart();
                                OrderRefKey = GetCell(orderCache, i, "OrderRefKey", out errCode).ToString().TrimStart();
                                BranchCode = GetCell(orderCache, i, "BranchCode", out errCode).ToString().TrimStart();
                                BranchSeq = GetCell(orderCache, i, "BranchSeq", out errCode).ToString().TrimStart();
                                Exchange = GetCell(orderCache, i, "Exchange", out errCode).ToString().TrimStart();

                                var Ord = new Order();

                                Time = Time.Remove(0, 9);
                                Ord.Time = Time;

                                Ord.Account = Account;
                                Ord.Side = Side;
                                Ord.Quantity = Quantity;
                                Ord.ExecQuantity = ExecQuantity;
                                Ord.Symbol = Symbol;
                                Ord.PriceDesc = PriceDesc;
                                Ord.ExecPr = ExecPrice;
                                Ord.PctCmp = PctCmp;
                                Ord.Lvs = Lvs;
                                Ord.Status = Status;
                                Ord.OrderRefKey = OrderRefKey;
                                Ord.BranchCode = BranchCode;
                                Ord.BranchSeq = BranchSeq;
                                Ord.Exchange = Exchange;
                                Ord.Customs = Customs;

                                Console.WriteLine("<Orders row=" + i + "|time="+Time+">\n" + Ord.ToString());

                                /*                         RediLib.ORDER newOrder = new RediLib.ORDER();
                                                         newOrder.SetOrderKey("r151681", OrderRefKey);

                                                         object counter = null;
                                                         newOrder.GetMBFieldCount(ref counter);
                                                         Console.WriteLine("Counter=" + counter);
                                                         Console.WriteLine("Exchange=" + newOrder.Exchange+ "Symbol="+ newOrder.Symbol);

                                                         if (newOrder.GetMBFieldCount(ref counter) == null)
                                                         {
                                                             for (int index = 0; index < ((int)counter); index++)
                                                             {
                                                                 object name = null; object value = null; object type = null; if (newOrder.GetMBFieldX(index, ref name, ref value, ref type) == null)
                                                                 {
                                                                     Console.WriteLine("Name=" + name + " value=" + value);
                                                                 }
                                                             }
                                                         } */
                            }
                            Console.WriteLine("End of orders");

                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("Exception: " + e);
                        }
                        break;

                }
            }
        }
    }
}
