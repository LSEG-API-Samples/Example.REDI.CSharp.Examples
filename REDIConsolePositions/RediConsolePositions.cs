using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using System.Windows;
using System.Reflection;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using RediLib;
using RediConsolePositions.MessageTypes;

namespace RediConsolePositions
{
    public delegate TResult FuncRef<T1, T2, TResult>(T1 param1, ref T2 param2);
    public delegate TResult FuncRef<T, TResult>(ref T param);

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

        private ObservableCollection<PositionMessage> _positionmessages;
        public ObservableCollection<PositionMessage> PositionMessages
        {
            get { return _positionmessages; }
            set
            {
                if (!Equals(this._positionmessages, value)) { }
                this._positionmessages.Clear();
                foreach (var item in value)
                {
                    this._positionmessages.Add(item);
                }
            }
        }

        public bool init()
        {
            try
            {

                this.PositionAccountList = new ObservableCollection<string>();

                positionCache = new CacheControl();
                positionCache.CacheEvent += PosCacheEvent;
                object pos_result = positionCache.Submit("Position", "true", ref pos_err);

                this.PositionAccountList.Add("EQUITY-TR");
                this.PositionAccountList.Add("All");

                Console.WriteLine("Position result is " + pos_result);

                foreach (string i in PositionAccountList)
                {
                    positionCache.AddWatch("2", "", i, acct_err);
                }


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

            Console.WriteLine("PosCacheEve.action: (" + action.ToString() + ") "+((CacheControlActions)action).ToString() );
            if (action == (int)CacheControlActions.Snapshot)
            {
                for (int row = 0; row < rowIndex; row++)
                {
                    var newMsg = new PositionMessage();
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

                        newMsg.DisplaySymbol = symbol;
                        newMsg.Account = account;
                        newMsg.Position = Int32.Parse(position);
                        newMsg.Value = Double.Parse(value);
                        Console.WriteLine("Row=" + row + " Position: " + newMsg);
                    } catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }
                }
            }
            if ((action == (int)CacheControlActions.Add) || (action == (int)CacheControlActions.Update))
            {
                var newMsg2 = new PositionMessage();
                var type = typeof(PositionMessage);
                var properties = type.GetProperties();
                Console.Write("Row=" + rowIndex +": ");
                foreach (PropertyInfo property in properties)
                {
                    variable = null;
                    err = null;
                    try { 
                        object val = positionCache.GetCell(rowIndex, property.Name, ref variable, ref err);
                        Console.Write(property.Name + "=" + variable + " ");
                        property.SetValue(newMsg2, variable, null);
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

