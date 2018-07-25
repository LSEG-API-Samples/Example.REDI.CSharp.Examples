# Trading Single Options in C#
## Introduction
With REDI’s powerful options capabilities, users can easily trade single and complex spread strategies globally through multiple brokers. The options trade can be sent via REDIPlus UI or REDIPlus API. This tutorial demonstrates how to trade single option via REDIPlus API with C# and Visual Studio 2017. REDIPLus and the tutorial example run on the same machine, side by side.  

Please note that this example doesn't demonstrate how to retrieve real-time price from REDIPlus API. It just sends an options order to a demo server. 

## Prerequisites

* REDIPlus is installed
* REDIPlus valid credentials are used to login
* Visual Studio 2017
* REDIPlus API

Note: To install REDIPlus and REDIPlus API, please refer to [REDIPlus & API Installation Guide](https://developers.thomsonreuters.com/transactions/redi-api/quick-start).

## Implementation
**1. Create a project:**

We start by creating a new Visual Studio project:

File->New->Project-> Visual C# -> Console App (.NET Framework)

**2. Add the Redi 1.0 Type library**

REDIPlus API is a COM library. To use it, we need to create a reference to this library. In the solution explorer, right click on **References** and select **Add Reference...** . 

On the left menu, select **COM** and then check on **Redi 1.0 Type Library**. 

![](AddReference.png)

Click on **OK** button to add REDIPlus API to the project.

If **Redi 1.0 Type Library** doesn't appear on the list, click **Browser...** button to locate the **Redi.tlb** file. Typically, this file should be found in **%LOCALAPPDATA%\REDI Tech\Primary** folder.

After adding the **Redi 1.0 Type Library**, expand the **References** in the solution explorer. The **RediLib** will be appeared in the list.

![](References.png)



Then, add the **using** directive to the code to allow the use of types in the RediLib namespace.

```csharp
using RediLib;
```
**3. Get expiration dates**

The expiration date is a required field for trade options. **RediLib.OPTIONORDER** can be used to get the list of expiration dates for an option.

First, the application creates a new instance of **RediLib.OPTIONORDER** and set an option in its **Symbol** property.

```csharp
OPTIONORDER objOrder = new OPTIONORDER();
objOrder.Symbol = "IBM";
```

Next, it calls **GetExpirationDatesCount** to get the count of option expiration dates based on the order object. The count is returned in the reference variable passed as an argument.
```csharp
object objExpirationCount = null;
objOrder.GetExpirationDatesCount(ref objExpirationCount);

```
Then, it calls **GetExpirationDateAt** for each index to get the options expiration date in REDI format from the expiration date list.
```csharp
for (int i = 0; i < (int)objExpirationCount; i++)
{
    object objExpiration = null;
    objExpiration = objOrder.GetExpirationDateAt(i);
    ...
}
```

**4. Get strike prices**

The strike price is a required field for trade options. **RediLib.OPTIONORDER** can be used to get the list of strike prices for an option.

First, the application creates a new instance of **RediLib.OPTIONORDER** and populate it with an option symbol, option type (Call or Put), and expiration date. For example:

```csharp
OPTIONORDER objOrder = new OPTIONORDER();
objOrder.Symbol = "IBM";
objOrder.type = "Call";
objOrder.Date = "Jul 27 '18";
```

Next, it calls **GetStrikesCount** to get the count of options strike prices based on the order object. The count is returned in the reference variable passed as an argument.
```csharp
object objStrikeCount = null;
objOrder.GetStrikesCount(ref objStrikeCount);
```
Then, it calls **GetStrikeAt** for each index to get the options strike price from the strike price list.
```csharp
for (int i = 0; i < (int)objStrikeCount; i++)
{
    object objStrike = null;
    objStrike = objOrder.GetStrikeAt(i);
    ...
}
```

**5. Submit a single option order**

An instance of **OPTIONORDER** can be used to submit an order. First, the application needs to create an instance of **OPTIONORDER**. Then, populate it with the following order information:

|Property Name|Description|Example|
|-------------|-----------|-------|
|Symbol|The symbol of an option|IBM|
|Quantity|Options contract size|1|
|Price|Limit Price of an order|10.50|
|type|Options Type (Call or Put)|Call|
|Date|Options expiration date in REDI date format. **OPTIONORDER.GetExpirationDateAt** can be used to retrieve expiration dates, as shown in step 3. |Jul 27 '18|
|Strike|The strike price of an option **OPTIONORDER.GetStrikeAt** can be used to retrieve expiration dates, as shown in step 4.|185.00|
|Position|Options order position (Open or Close)|Open|
|Side|Side of an order (Buy or Sell)|Buy|
|Exchange|Broker (or Exchange) Destination. The application can use "DEM1 DMA" for a demo server. **OPTIONORDER.GetExchangeAt** can be used to retrieve the broker/exchange destination name from the exchange list|DEM1 DMA|
|PriceType|Order type of an order (Limit, Stop, Stop Limit, Market Close, Market, or Limit Close). **OPTIONORDER.GetPriceTypeAt** can be used to retrieve the options price type name from the price type list |Limit|
|TIF|Time In Force for an order (Day). **OPTIONORDER.GetTIFAt** can be used to retrieve the TIF (time in force) from the TIF list |Day|
|Account|The account used for this order. **OPTIONORDER.GetAccountAt** can be used to the account name from the account list|EQUITY-TR|
|Ticket|The ticket associated in this order. Bypass can be use to avoid the ticket|Bypass|

For example, the below code is **Buy to Open** a contract for IBM Jul 27 '18 $185.00 Call at 10.50. Time in force is Day and the order type is Limit. The order will be sent to a demo server with bypass ticket.

```csharp
OPTIONORDER objOrder = new OPTIONORDER();

objOrder.Symbol = "IBM";
objOrder.Quantity = "1";
objOrder.Price = "10.50";
objOrder.type = "Call";
objOrder.Date = "Jul 27 '18";          
objOrder.Strike = "185.00";           
objOrder.Position = "Open";         
objOrder.Side = "Buy";            
objOrder.Exchange = "DEM1 DMA";
objOrder.PriceType = "Limit";            
objOrder.TIF = "Day";
objOrder.Account = "EQUITY-TR";
objOrder.Ticket = "Bypass";
```
Then, **OPTIONORDER.Submit** is called to submit an order. 
```csharp
object ord_err = null;
bool status;
status = objOrder.Submit(ref ord_err);
if (!status)
{
    Console.WriteLine($"{(string)ord_err}");
}
```
This method returns **True** if order submission was successfully. Otherwise it will return **False**. If it returns **False**, the failure reason will be populated in the reference variable passed as a string argument. 

## Example
The simple application called **SingleOptionsTrade** is developed to demonstrate how to send a single options trade.

**1. Parsing Command Line Arguments**

The application depends on the **CommandLineParser** package to manipulate command line arguments. It supports the following parameters:

```
  -s, --symbol       Required. The symbol of an option

  -q, --quantity     (Default: 1) Options contract size

  -p, --price        Limit Price of an order

  -t, --stop         Stop Price of an order

  -y, --type         (Default: Call) Options Type (Call or Put)

  -x, --expdate      Options expiration date in REDI date format

  -k, --strike       The strike price of an option

  -o, --position     (Default: Open) Options order position (Open or Close)

  -d, --side         (Default: Buy) Side of an order (Buy or Sell)

  -e, --exchange     (Default: DEM1 DMA) Broker (or Exchange) Destination

  -f, --tif          (Default: Day) Time In Force for an order

  -r, --pricetype    (Default: Limit) Order type of an order (Limit, Stop, Stop Limit,
                     Market Close, Market, or Limit Close)

  -a, --account      The account used for this order

  -c, --ticket       (Default: Bypass) The ticket associated in this order

  --help             Display this help screen.

  --version          Display version information.
```

The symbol (-s, --symbol) is a required argument. However, price (-p, --price) or stop price (-t, --stopprice) can be required depending on the value of price type (-r, --pricetype). For example, if the price type is Limit, the price (-p, --price) is required. 

**2. Get the Expiration Date**

If the date (-d, --date) is not specified, the application will use the first available expiration date. It uses a function called **GetExpirationDate** which accepts an option symbol and returns the first expiration date for that option. If there is no expiration data for that option, it will return null.

The function calls **GetExpirationDatesCount** to get the count of option expiration dates based on the order object and then calls **GetExpirationDateAt** with the first index (0) to get and return the first options expiration date in REDI format from the expiration date list.


```csharp
private string GetExpirationDate(string symbol)
{
    OPTIONORDER objOrder = new OPTIONORDER();
    objOrder.Symbol = symbol;

    object objExpirationCount = null;
    objOrder.GetExpirationDatesCount(ref objExpirationCount);
    if (objExpirationCount == null || (int)objExpirationCount == 0)
    {
        return null;
    }
    else
    {
        return (string)(objOrder.GetExpirationDateAt(0));
    }
}
```

**3. Get Strike Price**

If the strike price (-k, --strike) is not specified, the application will use the first available strike price. It uses a function called **GetStrikePrice** which accepts an option symbol, option type, and expiration date and returns the first price for that option. If there is no strike price for that option, it will return null.

The function calls **GetStrikesCount** to get the count of options strike prices based on the order object, and then calls **GetStrikeAt** with the first index (0) to get and return the first options strike price from the strike price list.

```csharp
private string GetStrikePrice(string symbol, string type, string expirationDate)
{
    OPTIONORDER objOrder = new OPTIONORDER();
    objOrder.Symbol = symbol;
    objOrder.type = type;
    objOrder.Date = expirationDate;

    object objStrikeCount = null;
    objOrder.GetStrikesCount(ref objStrikeCount);

    if (objStrikeCount == null || (int)objStrikeCount == 0)
    {
        return null;
    }
    else
    {              
        return (string)objOrder.GetStrikeAt(0);
    }
}

```

**4. Get Account**

If the account (-a, --account) is not specified, the application will use the first available account. It uses a function called **GetAccount** which returns the first available account. If there is no available account, it will return null.

The function calls **GetAccountCount** to get the count of account based on the order object, and then calls **GetAccountAt** with the first index (0) to get and return the first account from the account list.

```csharp
private string GetAccount()
{
    OPTIONORDER objOrder = new OPTIONORDER();
    object objAccountCount = null;
    objOrder.GetAccountCount(ref objAccountCount);
    if(objAccountCount == null || (int)objAccountCount == 0)
    {
        return null;
    }
    else
    {              
        return (string)objOrder.GetAccountAt(0);
    }            
}

```
**5. Submit an Order**

After the values have been verified, the application submits an order. 
```csharp
private void SendOrder()
{
    OPTIONORDER objOrder = new OPTIONORDER();   

    objOrder.Symbol = options.Symbol;
    objOrder.Quantity = options.Quantity.ToString();
    objOrder.Price = options.Price.ToString();
    objOrder.StopPrice = options.StopPrice.ToString();           
    objOrder.type = options.Type;
    objOrder.Date = options.Date;          
    objOrder.Strike = options.Strike;           
    objOrder.Position = options.Position;         
    objOrder.Side = options.Side;            
    objOrder.Exchange = options.Exchange;
    objOrder.PriceType = options.PriceType;            
    objOrder.TIF = options.TIF;
    objOrder.Account = options.Account;
    objOrder.Ticket = options.Ticket;
   
    object ord_err = null;
    bool status;
    status = objOrder.Submit(ref ord_err);
    if (!status)
    {
        Console.WriteLine($"Error: {(string)ord_err}");
    }
    else
    {
        Console.WriteLine("Order has been submitted properly");
    }

}

```
It creates a new instance of OPTIONORDER and populates its value. Then, it calls **OPTIONORDER.Submit** to submit the order. If the submit returns false which indicates failure, the error will be printed to the console.  

## Test and Run

**1. Buy to Open a call contract for IBM at 15.20**
```
OptionTrade.exe -s IBM -p 15.20
```
The command runs with symbol (-s) and price (-p) options. Therefore, it uses the first expiration date, strike price, and account retrieved from the REDIPlus API. For other options, the default values are used.

```
Send an order with the following options:
Symbol: IBM
Quantity: 1
Price: 15.20
Stop Price:0
Type: Call
Date: Jul 27 '18
Strike: 100.00
Position: Open
Exchange: DEM1 DMA
PriceType: Limit
TIF: Day
Account: EQUITY-TR
Ticket: Bypass
==============================
Order has been submitted properly
```
The order can be verified from the Message Monitor.

![](Order1.png)

**2. Buy to Open a put contract for IBM at the best available current price**
```
OptionTrade.exe -s IBM -y Put -r Market
```
The command runs with symbol (-s), type (-y), and price type (-r) options. Therefore, it uses the first expiration date, strike price, and account retrieved from the REDIPlus API. For other options, the default values are used.

```
Send an order with the following options:
Symbol: IBM
Quantity: 1
Price: 0
Stop Price:0
Type: Put
Date: Jul 27 '18
Strike: 100.00
Position: Open
Exchange: DEM1 DMA
PriceType: Market
TIF: Day
Account: EQUITY-TR
Ticket: Bypass
==============================
Order has been submitted properly
```
The order can be verified from the Message Monitor.

![](Order2.png)

**3. Buy to Open two call contracts for IBM Aug 03 '18 at 15.20**
```
OptionTrade.exe -s IBM -p 15.20 -q 2 -x "Aug 03 '18"
```
The command runs with symbol (-s), price (-p), quantity (-q), and expiration date (-x) options. Therefore, it uses the first strike price, and account retrieved from the REDIPlus API. For other options, the default values are used.

```
Send an order with the following options:
Symbol: IBM
Quantity: 2
Price: 15.20
Stop Price:0
Type: Call
Date: Aug 03 '18
Strike: 110.00
Position: Open
Exchange: DEM1 DMA
PriceType: Limit
TIF: Day
Account: EQUITY-TR
Ticket: Bypass
==============================
Order has been submitted properly
```
The order can be verified from the Message Monitor.

![](Order3.png)
## Summary

The options trade can be sent via REDIPlus UI or REDIPlus API. With REDIPlus API, **RediLib.OPTIONORDER** class is used to populate and submit an order. This class can also be used to get the list of accounts, exchanges, expiration dates, strike prices, price types, and time in force.

The usage is simple. The application creates a new instance of **RediLib.OPTIONORDER** class, populates its values, and then call submit to send an order.

## References
* [REDI API SPECIFICATION](https://developers.thomsonreuters.com/transactions/redi-api/docs?content=25822&type=documentation_item)
