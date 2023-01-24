# Diagnostics Logging Usage

## LogEntry class
The `LogEntry` class contains properties associated with a loggable event. All properties are required to be provided in the class constructor.

> **_NOTE:_** Creating an instance of `LogEntry` does __not__ actually save it to a log file.

## Creating Log Entries
To create a log entry, utilize the static `LogManager.CreateLogEntry` method. When this method is invoked, all instances of `LogManager` (its child classes) will be notified and will save log accordingly.

> **_NOTE:_** At least one instance of `LogManager` must exist in order to save log entries. These should be instantiated based on the application requirements (4-Series appliance, Virtual Control, etc.)

## Example SIMPL# Pro Setup
The below code is an example of how the `LogManager` class could be dynamically instantiated in SIMPL # Pro inside the `ControlSystem.InitializeSystem` method. Note that the `CrestronVc4CsvLog` class has not been written yet.

```c#
public override void InitializeSystem()
{
   try
   {
      // first determine the runtime environment
      switch (CrestronEnvironment.DevicePlatform)
      {
         // 4-series processor appliance
         case eDevicePlatform.Appliance:
         {
            // instantiate a 4-series appliance log manager object
            var fourSeriesManager = new Crestron4SeriesCsvLog();
            break;
         }
         // VC-4
         case eDevicePlatform.Server:
         {
            // instantiate a VC-4 log manager object
            var vc4Manager = new CrestronVc4CsvLog();
            break;
         }
      }
   }
   catch (Exception e)
   {
      ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
   }
}
```

From this point, the application can call `LogManager.CreateLogEntry` method as needed and either version of the Crestron log manager objects will respond accordingly.