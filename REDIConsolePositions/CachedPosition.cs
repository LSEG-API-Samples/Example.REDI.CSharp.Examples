using System;

namespace RediConsolePositions
{
    class CachedPosition
    {
        private string _account;
        public string Account
        {
            get { return _account; }
            set {_account = value; }
        }
        private string _displaysymbol;
        public string DisplaySymbol
        {
            get { return _displaysymbol; }
            set { _displaysymbol = value; }
        }

        private int _position;
        public int Position
        {
            get { return _position; }
            set { _position = value; }
        }
        private double _value;
        public double Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public override String ToString()
        {
            return "Symbol=" + DisplaySymbol + "|Account=" + Account + "|Postion=" + Position
                + "|Value=" + Value;
        }
    }
}
