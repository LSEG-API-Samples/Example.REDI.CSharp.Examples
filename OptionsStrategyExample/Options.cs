using CommandLine;
namespace REDI.Csharp.Examples.ComplexOptionsTrade
{
    class Options
    {
        [Option("strategy", Required = true, HelpText = "The strategy for the spread option (Vertical, Ratio, Butterfly, or Buy-Write")]
        public string Strategy { get; set; }
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

        [Option("type1", Default = "Call", HelpText = "Options Type of the first leg (Call or Put) for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Type1 { get; set; }

        [Option("type2", Default = "Call", HelpText = "Options Type of the second leg (Call or Put) for the Ratio")]
        public string Type2 { get; set; }

        //[Option("type3", Default = "Call", HelpText = "Options Type of the third leg (Call or Put)")]
        //public string Type3 { get; set; }

        [Option("side1", Default = "Buy", HelpText = "Side of the first leg (Buy or Sell) for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Side1 { get; set; }

        [Option("side2", Default = "Sell", HelpText = "Side of the second leg (Buy or Sell) for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Side2 { get; set; }
        [Option("side3", Default = "Buy", HelpText = "Side of the third leg (Buy or Sell) for the Butterfly strategy")]
        public string Side3 { get; set; }

        [Option("position1", Default = "Open", HelpText = "Options order position of the first leg (Open or Close) for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Position1 { get; set; }

        [Option("position2", Default = "Open", HelpText = "Options order position of the second leg (Open or Close) for the Vertical, Ratio, and Butterfly strategies")]
        public string Position2 { get; set; }
        [Option("position3", Default = "Open", HelpText = "Options order position of the third leg (Open or Close) for the Butterfly strategy")]
        public string Position3 { get; set; }

        [Option("date1", HelpText = "Options expiration date in REDI date format of the first leg (e.g. \"Oct 05 '18\") for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Date1 { get; set; }

        [Option("date2", HelpText = "Options expiration date in REDI date format of the second leg (e.g. \"Oct 05 '18\") for the Ratio strategy")]
        public string Date2 { get; set; }

        //[Option("date3", HelpText = "Options expiration date in REDI date format of the third leg (e.g. \"Oct 05 '18\")")]
        //public string Date3 { get; set; }

        [Option("strike1", HelpText = "The strike price of the first leg for the Buy-Write, Vertical, Ratio, and Butterfly strategies")]
        public string Strike1 { get; set; }

        [Option("strike2", HelpText = "The strike price of the second leg for the Vertical, Ratio, and Butterfly strategies")]
        public string Strike2 { get; set; }

        [Option("strike3", HelpText = "The strike price of the third leg for the Butterfly strategy")]
        public string Strike3 { get; set; }
        [Option("ratio1", Default = 1, HelpText = "The ratio of the first leg for the Ratio strategy")]
        public int Ratio1 { get; set; }
        [Option("ratio2", Default = 1,  HelpText = "The ratio of the second leg for the Ratio strategy")]
        public int Ratio2 { get; set; }

        //[Option("account1", HelpText = "The account used for the first leg")]
        //public string Account1 { get; set; }

        //[Option("account2", HelpText = "The account used for the second leg")]
        //public string Account2 { get; set; }
    }
}
