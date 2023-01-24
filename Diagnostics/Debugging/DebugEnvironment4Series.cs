using System;
using System.Linq;
using Crestron.SimplSharp;

namespace Diagnostics.Debugging
{
    /// <summary>
    /// Implementation of <see cref="DebugEnvironment"/> for 4-series processors (not VC-4).
    /// </summary>
    public class DebugEnvironment4Series : DebugEnvironment
    {
        private const string DebugCommandHelp = "Specify the device name and true/false to enable/disable debugging. Use `debug send <device> <string>` to send data.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugEnvironment4Series"/> class.
        /// </summary>
        public DebugEnvironment4Series()
        {
            // subscribe to parent class' WriteToConsole event
            WriteToConsole += DebugEnvironment4Series_WriteToConsole;

            // create new console command to toggle debugging
            CrestronConsole.AddNewConsoleCommand(HandleDebugConsoleCommand, "debug", DebugCommandHelp, ConsoleAccessLevelEnum.AccessOperator);
        }

        /// <inheritdoc/>
        protected override void HandleDebugDataReceived(object sender, string e)
        {
            var castedObject = (ISendReceiveDebuggable)sender;
            CrestronConsole.PrintLine($"{castedObject.Name} Rx: {e}");
        }

        /// <inheritdoc/>
        protected override void HandleDebugDataSent(object sender, string e)
        {
            var castedObject = (ISendReceiveDebuggable)sender;
            CrestronConsole.PrintLine($"{castedObject.Name} Tx: {e}");
        }

        /// <summary>
        /// Writes content to the console when someone calls the static
        /// <see cref="DebugEnvironment.Write(string)"/> method.
        /// </summary>
        /// <param name="sender">Null.</param>
        /// <param name="e">Content to write.</param>
        private void DebugEnvironment4Series_WriteToConsole(object sender, string e)
        {
            CrestronConsole.PrintLine(e);
        }

        private void HandleDebugConsoleCommand(string args)
        {
            // split parameters by space
            string[] parameters = args.ToLower().Split(' ');

            // get the device specified by the first parameter
            var device = DebuggableObjects.FirstOrDefault(x => x.Name.ToLower() == parameters[0]);

            // if not null, do stuff
            if (device != null)
            {
                // if user wants to send data...
                if (parameters[1] == "send")
                {
                    // re-join remaining parameters (in case there are spaces in the command)
                    string toSend = string.Empty;
                    for (int i = 2; i < parameters.Length; i++)
                    {
                        toSend += parameters[i];
                    }

                    // tell the device to send the string
                    device.SendDataFromDebug(toSend);

                    // TODO: need a way to send byte arrays
                }
                else
                {
                    // try to parse the parameters[2] to a bool. If successful set debugging accordingly
                    bool enable;
                    bool success = bool.TryParse(parameters[2], out enable);
                    if (success)
                    {
                        CrestronConsole.ConsoleCommandResponse($"{(enable ? "Enabling" : "Disabling")} debugging on device {device.Name}");
                        device.EnableDebug = enable;
                    }

                    // if unsuccessful, report the device's debugging state to the user
                    else
                    {
                        CrestronConsole.ConsoleCommandResponse($"Debugging on device {device.Name} is {(device.EnableDebug ? "enabled" : "disabled")}");
                    }
                }
            }

            // otherwise alert the user they entered a bad device name
            else
            {
                CrestronConsole.ConsoleCommandResponse($"Object name \"{parameters[0]}\" is not a known debuggable object.");
            }
        }
    }
}
