# REDIPlus Order Monitor
# Tutorial (C#)

### Introduction

In this tutorial we will discuss how to use Redilink library to monitor REDIPlus orders.  REDIPLus and the tutorial example run on the same machine, side by side.  

We will go through the steps of creating a compact console example:

* On startup, retrieve the current orders from REDIPlus
* Consequently, monitor new order additions
* In addition, monitor existent order modifications

Please note, that this example does not initiate REDIPlus order-related events.  It continuously monitors their results.

### Prerequisites

* REDIPLus is installed
* Valid credentials are used to login
* VisualStudio is installed (we used VS 2017)

### Implementation

1\. Create project:    
We start by creating a Visual Studio project:
File->New->Project-> Visual C# -> Console App (.NET Framework)PickRediTlb
and saving it.

2\. Add the Redi 1.0 type library

In Solution Explorer, right-click on **References** and pick **Add Reference**:  

![AddReference](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/AddReference.gif)

In Reference Manager, pick **COM** and select **REDILib Type Library** if it is available on COM library list:

![PickRediTlb](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/PickRediTlb.gif)

Otherwise, if **Redi 1.0 Type Library** is not on the COM library list, click **Browse** andlocate the Redi.tlb file in %LOCALAPPDATA%\REDI Tech\Primar**y** folder.

3\. Create Order class that emulates REDI order,
by including the required fields:

    ```
    ...
    private string _side;
            public string Side {
                get { return _side; }
                set { this._side= value; }
            }
            private string _quantity;
            public string Quantity {
                get { return _quantity; }
               set { this._quantity = value; }
            }
    ...
    public override String ToString() {
                return "Ref="+OrderRefKey+"|Symbol=" + Symbol + "|Side=" + Side + "|Quantity=" + Quantity + "|ExecQuantity=" + ExecQuantity + "|PriceDesc=" + PriceDesc 
                    + "|PctCmp=" + PctCmp + "|Lvs=" + Lvs + "|ExecPr=" + ExecPr + "|Account=" + Account + "|Status=" + Status;
            }        
    ```

4\. Next, we create the main class, that we have named "RediConsoleOrders" 

The key steps of it are:
4a. Initialize CacheControl that will contain the state of all orders within REDIPlus
```
orderCache = new CacheControl();
```
4b. Specify a callback to call when a cache change event is triggered:
```
orderCache.CacheEvent += orderCacheHandler;
```
4c. Within callback, handle events of types:
 
 * CacheControlActions.Add - triggered by a new order is added to the order cache
 * CacheControlActions.Update - triggered by a modification (such as cancelation) of an existent order
 * CacheControlActions.Snapshot - triggered on inititialization in order to synchronize the current state

4d. To start interacting with REDI CacheControl we call:
```
orderCache.Submit()
```
### Testing

Build and start the application without starting REDIPlus first.

This should result in an exception:

![IsREDIRunning](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/IsREDIRunning.gif)

Next we start REDIPlus, login, and open up Message Monitor.

![REDIOpenMessageMonitor](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/REDIOpenMessageMonitor.gif)

It will come up empty, as no orders have been placed yet.

Now we create a couple of test orders:

![REDIOrderCreate](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/REDIOrderCreate.gif)

The new Orders should appear in Message Monitor as they arrive
As well as on console:

![REDIOrdersSnapshot](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/REDIOrdersSnapshot.gif)

if we click on "Cancel" on Message Monitor the order update is reflected on console:

![REDIOrderCancelOnConsole](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/REDIOrderCancelOnConsole.gif)

At this point, if we restart our Monitor application

The current state of orders is fully synchronized on startup,
via snapshot:

![REDIOrderSnapshotOnRestart](https://github.com/TR-API-Samples/Example.REDI.CSharp.Examples/blob/master/REDIConsoleOrders/REDIOrderSnapshotOnRestart.gif)

This concludes our tutorial designed to help to get started with REDI C# coding
and serve as a stepping stone toward more advanced REDI learning materials available on this portal
