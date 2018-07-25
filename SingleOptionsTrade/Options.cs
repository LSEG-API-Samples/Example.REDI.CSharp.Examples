using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
namespace OptionsTrade
{
    public class Options
    {
        [Option('s',"symbol", Required =true,HelpText = "The symbol of an option") ]
       
        public string Symbol { get; set; }

        [Option('q', "quantity",Default = 1, HelpText = "Options contract size")]
        public int Quantity { get; set; }
        [Option('p', "price", HelpText = "Limit Price of an order")]
        public decimal Price { get; set; }
        [Option('t', "stopprice", HelpText = "Stop Price of an order")]
        public decimal StopPrice { get; set; }

        [Option('y', "type", Default = "Call", HelpText = "Options Type (Call or Put)")]
        public string Type { get; set; }
        [Option('x', "expdate", HelpText = "Options expiration date in REDI date format")]
        public string Date { get; set; }

        [Option('k', "strike", HelpText = "The strike price of an option")]
        public string Strike { get; set; }

        [Option('o', "position", Default = "Open", HelpText = "Options order position (Open or Close)")]
        public string Position { get; set; }

        [Option('d', "side", Default = "Buy", HelpText = "Side of an order (Buy or Sell)")]
        public string Side { get; set; }

        [Option('e', "exchange", Default = "DEM1 DMA", HelpText = "Broker (or Exchange) Destination")]
        public string Exchange { get; set; }

        [Option('f', "tif", Default = "Day", HelpText = "Time In Force for an order")]
        public string TIF { get; set; }

        [Option('r', "pricetype", Default = "Limit", HelpText = "Order type of an order (Limit, Stop, Stop Limit, Market Close, Market, or Limit Close)")]
        public string PriceType { get; set; }

        [Option('a', "account", HelpText = "The account used for this order")]
        public string Account { get; set; }


        [Option('c', "ticket", Default ="Bypass", HelpText = "The ticket associated in this order")]
        public string Ticket { get; set; }

      
    }
}
