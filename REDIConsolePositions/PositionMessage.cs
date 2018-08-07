using System;
using System.ComponentModel;

namespace RediConsolePositions.MessageTypes
{
    class PositionMessage //: INotifyPropertyChanged
    {
        #region Account
        private string _account;
        public string Account
        {
            get { return _account; }
            set
            {
                _account = value;
                RaisePropertyChanged("DisplaySymbol");
            }
        }
        #endregion

        #region DisplaySymbol
        private string _displaysymbol;
        public string DisplaySymbol
        {
            get { return _displaysymbol; }
            set
            {
                _displaysymbol = value;
                RaisePropertyChanged("DisplaySymbol");
            }
        }
        #endregion

        #region Position
        private int _position;
        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RaisePropertyChanged("Position");
            }
        }
        #endregion

        #region Value
        private double _value;
        public double Value
        {
            get { return _value; }
            set
            {
                _value = value;
                RaisePropertyChanged("Value");
            }
        }
        #endregion

        #region INotifyPropertyChanged Members
        private void RaisePropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        public override String ToString()
        {
            return "Symbol=" + DisplaySymbol + "|Account=" + Account + "|Postion=" + Position
                + "|Value=" + Value;
        }

    }
}
