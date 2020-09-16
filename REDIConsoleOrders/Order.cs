using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using RediLib;

/**
 * Example code 
 * class Order
 * For TR -> REDI -> Monitor Orders In C# 
 * Tutorial
 * **/

namespace RediConsoleOrders
{
    class Order 
    {
        // this class is designed to emulate a REDI order 
        // it encapsulates the fields of any order that exists in REDIPlus OrderCache

         private string _time;

        public string Time
        {
            get { return _time; }
            set { this._time=  value; }

        }

        private string _side;

        public string Side
        {
            get { return _side; }
            set { this._side= value; }

        }

        private string _quantity;

        public string Quantity
        {
            get { return _quantity; }
           set { this._quantity = value; }

        }

        private string _symbol;

        public string Symbol
        {
            get { return _symbol; }
            set { this._symbol = value; }
        }

        private string _pricedesc;

        public string PriceDesc
        {
            get { return _pricedesc; }
            set { this._pricedesc= value; }
        }

        private string _execquantity;

        public string ExecQuantity
        {
            get { return _execquantity; }
            set { this._execquantity= value; }
        }

        private string _pctcmp;

        public string PctCmp
        {
            get { return _pctcmp; }
            set { this._pctcmp= value; }
        }

        private string _lvs;

        public string Lvs
        {
            get { return _lvs; }
            set { this._lvs= value; }
        }



        private string _execpr;

        public string ExecPr
        {
            get { return _execpr; }
            set { this._execpr= value; }
        }


        private string _status;

        public string Status
        {
            get { return _status; }
            set { this._status= value; }
        }


        private string _account;

        public string Account
        {
            get { return _account; }
            set { this._account= value; }

        }

        private string _orderrefkey;

        public string OrderRefKey
        {
            get { return _orderrefkey; }
            set { this._orderrefkey= value; }

        }

        private string _branchCode;
        public string BranchCode
        {
            get { return _branchCode; }
            set { this._branchCode = value; }

        }

        private string _branchSeq;
        public string BranchSeq
        {
            get { return _branchSeq; }
            set { this._branchSeq = value; }

        }

        private string _exchange;
        public string Exchange
        {
            get { return _exchange; }
            set { this._exchange = value; }

        }

        private string _customs;

        public string Customs
        {
            get { return _customs; }
            set { this._customs = value; }
        }
        public override String ToString()
        {
            string retString = 
             "Ref="+OrderRefKey + "|BranchCode=" + BranchCode + "|BranchSeq=" + BranchSeq + "|Symbol=" + Symbol + "|Side=" + Side + "|Quantity=" + Quantity + "|ExecQuantity=" + ExecQuantity + "|PriceDesc=" + PriceDesc 
                + "|PctCmp=" + PctCmp + "|Lvs=" + Lvs + "|ExecPr=" + ExecPr /* + "|Exchange=" + Exchange*/ + "|Account=" + Account + "|Status=" + Status;
            if (!Customs.Equals(""))
                retString += ("|Customs=" + Customs);
            return retString;
        }
        
       
    }
}
