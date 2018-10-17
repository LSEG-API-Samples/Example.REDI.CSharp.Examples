using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    static class StrategyFactory
    {
        //Support case insentitive with the following strategies (ratio, butterfly, buy-write, vertical)
        public static IStrategy Create(string name)
        {
            IStrategy strategy = null;
            switch (name.ToLower())
            {
                case "ratio":
                    strategy = new RatioStrategy(); ;
                    break;
                case "butterfly":
                    strategy = new ButterflyStrategy();
                    break;
                case "buy-write":
                    strategy = new BuyWriteStrategy();
                    break;
                case "vertical":
                    strategy = new VerticalStrategy();
                    break;
            }
            return strategy;
        }
    }
}
