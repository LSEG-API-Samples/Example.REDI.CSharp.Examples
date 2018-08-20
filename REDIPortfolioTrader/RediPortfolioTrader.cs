//============================================================================
//REDI API Tutorial: Redi Portfolio Trader
//Goal:  Demo how to send a set of tickets for orders to Redi Portfolio Trader:
//       Read a list of tickets from an input file, submit them to REDI.
//       Report results to screen and log file.
//       Includes some rudimentory error checking.
//============================================================================
//To facilitate the learning process, the member declarations are at the start,
//followed by the main and finally the methods.
//============================================================================
using System;
using System.IO;
using RediLib;

namespace RediPortfolioTrader
{
    class PortfolioTraderLoadList
    {
        //=====================================================================
        //Member declarations
        //=====================================================================
        //Fill these according to your credentials and environment:
        private static string rediUserName = "YourREDIUserId";
        private static string rediAccount = "YourREDIAccount";
        
        private static string rediPortfolioTraderListName = "MyTestList1";

        private static string inputDirectory = "C:\\REDI\\Tutos\\RediPortfolioTrader\\";
        private static string ticketInputFile = inputDirectory + "PTListDemo.csv";
        //File format defined for this sample (this is not a standard, just an example):
        //1 ticket per line.
        //Line format: <symbol>, <side (Buy or Sell)>, <quantity>
        //Line starting with # is a comment
        //Example lines:
        //#Symbol,Side,Qty
        //WEN,Buy,10
        //INTC,Sell,20

        private static string outputDirectory = inputDirectory;
        private static string logFileName = "RediPortfolioTrader";  //This code will add a timestamp and .log

        //=====================================================================
        //Main program entry
        //=====================================================================
        static void Main()
        {
            Console.WriteLine("Start of Redi Portfolio Trader demo: load a list of tickets from a file\n");

            //-----------------------------------------------------------------
            //Check if the output directory exists:
            //-----------------------------------------------------------------
            if (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine("FATAL: cannot access directory " + outputDirectory + "\nCheck if it exists.");
                ConsolePrintAndWaitForEnter("Press Enter to exit");
                return;  //Exit main program
            }

            //-----------------------------------------------------------------
            //Log file: build full name, empty it if it exists, then open it for use:
            //-----------------------------------------------------------------
            string dateTimeNow = DateTime.Now.ToString("ddMMyyyyHHmm");
            string logFile = outputDirectory + logFileName + dateTimeNow + ".log";
            StreamWriter swLog = new StreamWriter(logFile, false);
            swLog.Close();
            swLog = new StreamWriter(logFile, true);

            //-----------------------------------------------------------------
            //Check if the input file exists:
            //-----------------------------------------------------------------
            if (!File.Exists(ticketInputFile))
            {
                DebugPrint("FATAL: cannot access " + ticketInputFile +
                           "\nCheck if file and directory exist.\n", swLog);
                swLog.Close();
                ConsolePrintAndWaitForEnter("Press Enter to exit");
                return;  //Exit main program
            }

            //-----------------------------------------------------------------
            //Check connection to REDIPlus:
            //-----------------------------------------------------------------
            try { ORDER testPtOrder = new ORDER(); }
            catch (System.Runtime.InteropServices.COMException comErr)
            {
                DebugPrint("FATAL: cannot connect to REDIPlus. Check if it is running and you are logged in.", swLog);
                DebugPrint("ERROR message:\n" + comErr, swLog);
                swLog.Close();
                ConsolePrintAndWaitForEnter("Press Enter to exit");
                return;  //Exit main program
            }

            //-----------------------------------------------------------------
            //Read input file, do some rudimentary validation on the entries.
            //For each valid entry: send order  to the Portfolio Trader list:
            //-----------------------------------------------------------------
            String symbol, side, qty;
            String fileLine = string.Empty;
            int fileLineNumber = 0;
            int validOrdersCount = 0;
            int failedToSubmitCount = 0;
            int quantity = 0;
            bool ignoreLine, success;
            bool endOfFile = false;

            //Open the input file:
            StreamReader sr = new StreamReader(ticketInputFile);

            //Loop through all lines until we get to the end of the file:
            while (!endOfFile)
            {
                //Read one line of the file, test if end of file:
                fileLine = sr.ReadLine();
                endOfFile = (fileLine == null);
                if (!endOfFile)
                {
                    fileLineNumber++;
                    try
                    {
                        //Ignore empty lines and comment lines (starting with #):
                        ignoreLine = String.IsNullOrEmpty(fileLine);
                        if (!ignoreLine)
                            if (!String.IsNullOrEmpty(fileLine)) ignoreLine = fileLine.Substring(0,1) == "#";
                        if (!ignoreLine)
                        {
                            //Parse the file line to extract the comma separated ticket parameters:
                            string[] splitLine = fileLine.Split(new char[] { ',' });
                            symbol = splitLine[0];
                            side = splitLine[1];
                            qty = splitLine[2];
                            //Validate orders parameters. If valid, submit order:
                            if (!String.IsNullOrEmpty(symbol))
                            {
                                if (side == "Buy" || side == "Sell")
                                {
                                    if (!String.IsNullOrEmpty(qty) && int.TryParse(qty, out quantity))
                                    {
                                        if (quantity >= 1)
                                        {
                                            validOrdersCount++;
                                            //Send order to the Portfolio Trader list:
                                            success = ptOrderSubmit(symbol, side, qty,
                                                                    rediAccount, rediUserName, rediPortfolioTraderListName,
                                                                    swLog);
                                            if (!success) failedToSubmitCount++;
                                        }
                                        else
                                        {
                                            DebugPrint("ERROR: input file line " + fileLineNumber +
                                                       ": qty must be greater than 0: " + fileLine, swLog);
                                        }
                                    }
                                    else
                                    {
                                        DebugPrint("ERROR: input file line " + fileLineNumber +
                                                   ": missing or invalid qty: " + fileLine, swLog);
                                    }
                                }
                                else
                                {
                                    DebugPrint("ERROR: input file line " + fileLineNumber +
                                               ": side must be Buy or Sell: " + fileLine, swLog);
                                }
                            }
                            else
                            {
                                DebugPrint("ERROR: input file line " + fileLineNumber +
                                           ": missing symbol: " + fileLine, swLog);
                            }
                        }
                    }
                    catch
                    {
                        DebugPrint("ERROR: input file line " + fileLineNumber + ": bad line format: " + fileLine, swLog);
                    }
                }
                else
                {
                    if (fileLineNumber == 0) DebugPrint("ERROR: empty file: " + ticketInputFile, swLog);
                }
            }  //End of while loop

            sr.Close();

            if (validOrdersCount == 0)
            {
                DebugPrint("\nFATAL: program exit due to no valid tickets in the list.", swLog);
                ConsolePrintAndWaitForEnter("Press Enter to exit");
                swLog.Close();
                return;  //Exit main program
            }

            DebugPrint("\nINFO: " + validOrdersCount +
                       " valid tickets submitted to REDI to load into Portfolio Trader list:\n" +
                       rediPortfolioTraderListName, swLog);
            if (failedToSubmitCount == 1)
                DebugPrint("WARNING: " + failedToSubmitCount + " of those tickets was refused", swLog);
            if (failedToSubmitCount > 1)
                DebugPrint("WARNING: " + failedToSubmitCount + " of those tickets were refused", swLog);
            swLog.Close();
            ConsolePrintAndWaitForEnter("\nPress a key to exit");
        }

        //=====================================================================
        //Helper methods
        //=====================================================================
        static void DebugPrint(string messageToPrintAndWriteToFile, StreamWriter sw)
        {
            Console.WriteLine(messageToPrintAndWriteToFile);
            sw.WriteLine(messageToPrintAndWriteToFile);
        }

        static void ConsolePrintAndWaitForEnter(string messageToPrint)
        {
            Console.WriteLine(messageToPrint);
            Console.ReadLine();
        }

        static bool ptOrderSubmit(String symbol, String side, String qty, String accnt, String tfUser, String tfList,
                                  StreamWriter sw)
        {
            ORDER ptOrder = new ORDER();
            Object err = null;
            bool success;

            ptOrder.Symbol = symbol;
            ptOrder.Side = side;
            ptOrder.Quantity = qty;
            ptOrder.Exchange = "*ticket";  //PT is a bunch of tickets
            ptOrder.Account = accnt;
            ptOrder.SetTFUser(tfUser);
            ptOrder.SetTFList(tfList);

            success = ptOrder.Submit(ref err);
            if (!success)
            {
                DebugPrint("ERROR: issue with ticket: " + ptOrder.Side + " " + ptOrder.Quantity + " " + ptOrder.Symbol, sw);
                //Note: the error message might be empty, it depends on the error case
                DebugPrint("ERROR message: " + err, sw);
            }
            else
            {
                DebugPrint("INFO: successfully submitted ticket to PT: " +
                           ptOrder.Side + " " + ptOrder.Quantity + " " + ptOrder.Symbol, sw);
            }
            return success;
        }
    }
}