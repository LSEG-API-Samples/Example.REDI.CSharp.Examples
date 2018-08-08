using System;
using System.Collections.ObjectModel;
using System.Reflection;
using RediLib;


namespace RediConsolePositions
{
    class DisplayRediPositions
    {

        private RediLib.CacheControl positionCache;

        private object pos_err = null;
        private object acct_err = null;
        private object err = null;
        private object variable = null;


        public enum CacheControlActions
        {
            Snapshot = 1,
            Add = 4,
            Update = 5,
            Delete = 8
        }

        public ObservableCollection<string> PositionAccountList { get; private set; }

        public bool init()
        {
            try
            {
                positionCache = new CacheControl();
                positionCache.CacheEvent += PosCacheEvent;

                this.PositionAccountList = new ObservableCollection<string>();
                this.PositionAccountList.Add("EQUITY-TR");
 //               this.PositionAccountList.Add("All");

                foreach (string i in PositionAccountList)
                {
                    positionCache.AddWatch("2", "", i, acct_err);
                }

                object pos_result = positionCache.Submit("Position", "true", ref pos_err);
                Console.WriteLine("Position result is " + pos_result);

                return true;
            }
            catch (System.Runtime.InteropServices.COMException come)
            {
                Console.WriteLine("\nException <<< IS REDIPlus RUNNING? >>>\n\n" + come);
                return false;
            }
        }

        private void PosCacheEvent(int action, int rowIndex)
        {
            string account, position, symbol, value;

            DateTime dt = DateTime.Now;

            Console.WriteLine(dt+">> PosCacheEve.action: (" + action.ToString() + ") "+((CacheControlActions)action).ToString() );
 
            if (action == (int)CacheControlActions.Snapshot) // on initial connection
            {
                for (int row = 0; row < rowIndex; row++)
                {
                    var newPos = new CachedPosition();
                    variable = null; err = null;
                    try
                    {
                        positionCache.GetCell(row, "Account", ref variable, ref err).ToString().TrimStart();
                        account = variable.ToString(); variable = null; err = null;
                        positionCache.GetCell(row, "Symbol", ref variable, ref err).ToString().TrimStart();
                        symbol = variable.ToString(); variable = null; err = null;
                        positionCache.GetCell(row, "Position", ref variable, ref err).ToString().TrimStart();
                        position = variable.ToString(); variable = null; err = null;
                        positionCache.GetCell(row, "Value", ref variable, ref err).ToString().TrimStart();
                        value = variable.ToString(); variable = null; err = null;

                        newPos.DisplaySymbol = symbol;
                        newPos.Account = account;
                        newPos.Position = Int32.Parse(position);
                        newPos.Value = Double.Parse(value);
                        Console.WriteLine("Row=" + row + " Position: " + newPos);
                    } catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }
                }
            }
            if ((action == (int)CacheControlActions.Add) || (action == (int)CacheControlActions.Update))
            {  // .Add on new position added, .Update on modification of position by user or market
                var newPos2 = new CachedPosition();
                var type = typeof(CachedPosition);
                var properties = type.GetProperties();
                Console.Write("Row=" + rowIndex +": ");
                foreach (PropertyInfo property in properties)
                {
                    variable = null;
                    err = null;
                    try { 
                        object val = positionCache.GetCell(rowIndex, property.Name, ref variable, ref err);
                        Console.Write(property.Name + "=" + variable + " ");
                        property.SetValue(newPos2, variable, null);
                    } catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }
                }
                Console.WriteLine("");
            }

        }


        static void Main(string[] args)
        {
            Console.WriteLine("Start of RediConsolePositions");
            DisplayRediPositions myPositions = new DisplayRediPositions();
            if (myPositions.init())
            {
                Console.WriteLine("Init completed");

               while (true)
                {
                }
            }
        }
    }
}

