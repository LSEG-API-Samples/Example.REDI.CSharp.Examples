using CommandLine;
namespace REDI.Csharp.Examples.SingleOptionsTrade
{
    //Define all available options for the application
    //The first parameter for the Option attribute is a short name, such as -s, -q
    //The second parameter for the Option attrubute is a long name, such as --symbol, --quantity
    //The symbol (-s, --symbol) is a required argument. 
    //However, price (-p, --price) or stop price (-t, --stopprice) can be required depending on the value of price type (-r, --pricetype). 

    public class Options
    {
        [Option('s',"symbol", Required =true,HelpText = "The symbol of an option") ]
       
        public string Symbol { get; set; }

        [Option('q', "quantity",Default = 1, HelpText = "Options contract size")]
        public int Quantity { get; set; }
        [Option('l', "limitprice", HelpText = "Limit Price of an order (Required by \"Limit\", \"Stop Limit\", \"Limit Close\")")]
        public decimal Price { get; set; }
        [Option('t', "stopprice", HelpText = "Stop Price of an order (Required by \"Stop\", \"Stop Limit\")")]
        public decimal StopPrice { get; set; }

        [Option('y', "type", Default = "Call", HelpText = "Options Type (Call or Put)")]
        public string Type { get; set; }
        [Option('x', "expdate", HelpText = "Options expiration date in REDI date format (e.g. \"Oct 05 '18\")")]
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

        [Option('p', "pricetype", Default = "Limit", HelpText = "Order type of an order (Limit, Stop, Stop Limit, Market Close, Market, or Limit Close)")]
        public string PriceType { get; set; }

        [Option('a', "account", HelpText = "The account used for this order")]
        public string Account { get; set; }


        [Option('c', "ticket", Default ="Bypass", HelpText = "The ticket associated in this order (Bypass, Direct, Autoticket, Autocreate")]
        public string Ticket { get; set; }

      
    }
}
