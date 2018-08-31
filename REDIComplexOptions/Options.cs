using CommandLine;
namespace REDI.Csharp.Examples.VerticalOptionsTrade
{
    class Options
    {
        [Option('s', "symbol", Required = true, HelpText = "The symbol of an option")]

        public string Symbol { get; set; }

        [Option('q', "quantity", Default = 1, HelpText = "Options contract size")]
        public int Quantity { get; set; }
        [Option('l', "limitprice", HelpText = "Limit Price of an order (Required by \"Limit\")")]
        public decimal Price { get; set; }
        [Option('a', "account", HelpText = "The account used for this order")]
        public string Account { get; set; }

        [Option('e', "exchange", Default = "DEM2 DMA", HelpText = "Broker (or Exchange) Destination")]
        public string Exchange { get; set; }

        [Option('f', "tif", Default = "Day", HelpText = "Time In Force for an order")]
        public string TIF { get; set; }
        [Option('p', "pricetype", Default = "Limit", HelpText = "Order type of a complex order (Limit, or Market)")]
        public string PriceType { get; set; }

        [Option("type1", Default = "Call", HelpText = "Options Type of the first leg (Call or Put)")]
        public string Type1 { get; set; }

        [Option("type2", Default = "Call", HelpText = "Options Type of the second leg (Call or Put)")]
        public string Type2 { get; set; }

        [Option("side1", Default = "Buy", HelpText = "Side of the first leg (Buy or Sell)")]
        public string Side1 { get; set; }

        [Option("side2", Default = "Sell", HelpText = "Side of the second leg (Buy or Sell)")]
        public string Side2 { get; set; }

        [Option("position1", Default = "Open", HelpText = "Options order position of the first leg (Open or Close)")]
        public string Position1 { get; set; }

        [Option("position2", Default = "Open", HelpText = "Options order position of the second leg (Open or Close)")]
        public string Position2 { get; set; }

        [Option("date1", HelpText = "Options expiration date in REDI date format of the first leg")]
        public string Date1 { get; set; }

        [Option("date2", HelpText = "Options expiration date in REDI date format of the second leg")]
        public string Date2 { get; set; }

        [Option("strike1", HelpText = "The strike price of the first leg")]
        public string Strike1 { get; set; }

        [Option("strike2", HelpText = "The strike price of the second leg")]
        public string Strike2 { get; set; }



        //[Option("account1", HelpText = "The account used for the first leg")]
        //public string Account1 { get; set; }

        //[Option("account2", HelpText = "The account used for the second leg")]
        //public string Account2 { get; set; }
    }
}
