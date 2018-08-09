# REDIPlus Position Monitor
# Tutorial (C#)

### Introduction

In this tutorial we will discuss how to use Redilink library to monitor REDIPlus positions. REDIPLus and the tutorial example run on the same machine, side by side.

We will go through the steps of creating a compact console example:

*   On startup, retrieve the current positions from REDIPlus (Positions Snapshot)
*   Consequently, monitor new positions being added (Positions Add)
*   Monitor existent positions changes (Positions Update)

Please note, that this example does not initiate REDIPlus position-related events. It continuously monitors positions that are in REDIPlus cache throughout their lifecycle.

### Prerequisites

*   REDIPLus is installed
*   Valid credentials are used to login
*   VisualStudio is installed (we used VS 2017)

### Implementation

1\. Create project:  
We start by creating a Visual Studio project:  
File->New->Project-> Visual C# -> Console App (.NET Framework)  
and saving it.

2\. Add the Redi 1.0 type library

In Solution Explorer, right-click on **References** and pick **Add Reference**:  
![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/AddReference.gif) 

In Reference Manager, pick **COM** and select **REDILib Type Library** if it is available on COM library list:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/PickRediTlb.gif) 

Otherwise, if **Redi 1.0 Type Library** is not on the COM library list, click **Browse** andlocate the Redi.tlb file in %LOCALAPPDATA%\REDI Tech\Primar**y** folder.

3\. Next we create a very simple class CachedPosition that will encapsulate a position in cache:

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
        }...

4\. Next, we create the main class, that we have named "RediConsolePositions"

The key features are are :  

4a. Initialize CacheControl that will contain the state of all orders within REDIPlus

     positionCache = new CacheControl();

4b. Specify a callback to call when a cache change event is triggered

    positionCache.CacheEvent += PosCacheEvent;

Within callback, we handle events of type

*   CacheControlActions.Add - triggered by a new order is added to the order cache
*   CacheControlActions.Update - triggered by a modification (such as cancelation) of an existent order
*   CacheControlActions.Snapshot - triggered on inititialization in order to synchronize the current state

4c. Next we create list of accounts and populate it with acount(s) that we are about to monitor

    this.PositionAccountList = new ObservableCollection<string>();this.PositionAccountList.Add("ACCOUNT_TO_MONITOR");foreach (string i in PositionAccountList){   positionCache.AddWatch("2", "", i, acct_err);}

_Please note, this step is not necessary for Order Monitoring, it's required for Position Monitoring._

4d. Now we are ready to start monitoring REDIPlus Positions, and to commence the interaction with the cache we call

    positionCache.Submit()

### Testing

Testing is important for Positions Monitoring.  It allows to assertain that position monitoring as implemented is working properly for the given use case.

First, let us build and start our monitor application without starting REDIPlus.

This should result in the exception:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/IsREDIRunningPositions.gif)

Next we start REDIPlus, login, and open up Positions Monitor window:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIOpenPositionMonitor.gif)

If at present account holds positions in securities, there will be positions populated in Monitor, may need to wait a second for them to populate.

If there are no positions, the monitor will be empty.  In order to test our application, we place a couple of orders that should result in positions.

We can run Trading -> Montage II

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIMontage2.gif)

In the newly opened Montage II window we create a test order or more:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIBuyTRI.gif)

In this specific example we bought TRI.

We should see the Positions updated in REDIPlus Position Monitor:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIPlusPositions1.gif)

Now we start our Position Monitor C# application and we should see the same postions in the Snapshot that we see in REDIPlus Position Monitor:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIPositionsSnapshot.gif)

Let's observe how a valid position updates in value, and we immediately receive some Updates as well.

Next, we intruduce another test order, buy 888 shares of instrument T, that should result in a new position,

and will result first in .Add and next in Updates:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIPositionBuyT.gif)

If we restart our application, the newly acquired position will be part of the initial Snapsot:

![](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsolePositions/REDIPositionsSnapshotUpdated.gif)

### Odds and Ends

*   If the value of a position in REDIPlus Positions window is 0, the value may appear in Positions window, but may not necessarily be part a of Snapshot.  Adds and Updates may continue to arrive on these positions.
*   Positions, value of which is 0 that do not appear in REDIPlus Positions window, may be part of Snapshot.
*   On initialization of the monitorring, the first cache retrival may fail with exception, however, the consecutive attempt will succeseed
*   Snapshot does not always consistently report the full contents of Positions cache on the initial connection.  It can be empty, or missing positions.  We are working to fully investigate this behavior.  A restart of the application, i.e. next initialization, always resolves the issue.

This concludes our tutorial designed to help to get started with understanding of REDI C# Positions monitoring.

The complete project code is available for download:

[https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/tree/master/REDIConsolePositions](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/tree/master/REDIConsolePositions)
