using System;
using System.Timers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserInteraction;
using Crestron.SimplSharp;

namespace MAVETesting
{
    public class ButtonHoldCheck
    {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public ushort DigitalJoin { get; set; }
        public ushort AnalogJoin { get; set; }
        public ushort SerialJoin { get; set; }
        public static int Deleted { get; set; }

        private UserInteractionEventArgs UIEventArgs;

        public ButtonHoldCheck(UserInteractionEventArgs args)
        {
            // initialize HoldTimer and subscribe to its events
            HoldTimer = new Timer(HoldTime);
            HoldTimer.Elapsed += HoldTimer_Elapsed;
            HoldTimer.AutoReset = false;

            // initialize RepeatTimer and subscribe to its events
            RepeatTimer = new Timer(RepeatTime);
            RepeatTimer.Elapsed += RepeatTimer_Elapsed;
            RepeatTimer.AutoReset = true;

            UIEventArgs = args;
        }

        private bool _buttonState;

        // flag to know if button was held so we know whether to raise
        // the momentary press event when the button is released
        private bool _buttonWasHeld;

        public bool ButtonState
        {
            get { return _buttonState; }
            set
            {
                
                _buttonState = value;

                // if true, button was pushed
                if (_buttonState)
                {
                    // do nothing other than start the timer
                    HoldTimer.Start();
                    RepeatTimer.Start();
                    UIEventArgs.ButtonArgs.DigitalValue = DigitalState.On;
                    MomentaryPress?.Invoke(this, UIEventArgs);
                }

                // button was released
                else
                {
                    // if the button had not been held, raise the momentary press event
                    if (!_buttonWasHeld)
                    {
                        
                    }

                    UIEventArgs.ButtonArgs.DigitalValue = DigitalState.Off;
                    MomentaryPress?.Invoke(this, UIEventArgs);


                    // stop the timers
                    HoldTimer.Stop();
                    RepeatTimer.Stop();

                    // reset the buttonWasHeld flag
                    _buttonWasHeld = false;
                    
                }
            }
        }

        // event raised when button is released before the HoldTimer expires
        public event EventHandler<UserInteractionEventArgs> MomentaryPress;

        // event raised if the button is still held after HoldTimer expires
        // repeatedly raised at the 'RepeatTime' interval until button is released
        public event EventHandler<UserInteractionEventArgs> Held;

        // initial time button must be held to raise the "Held" event in ms
        public int HoldTime { get; set; } = 2000;

        // interval at which the "Held" event will be raised while the button is still held
        public int RepeatTime { get; set; } = 1000;

        private Timer HoldTimer;
        private Timer RepeatTimer;

        private void HoldTimer_Elapsed(object sender, EventArgs e)
        {
            _buttonWasHeld = true;

            UIEventArgs.ButtonArgs.DigitalValue = DigitalState.Hold;

            // first raise the event that the button was held
            Held?.Invoke(this, UIEventArgs);

            // next start the HoldTimer
            //HoldTimer.Start();
        }

        private void RepeatTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // each time the HoldTimer ellapses, raise the Held event again
            UIEventArgs.ButtonArgs.DigitalValue = DigitalState.Hold;
            Held?.Invoke(this, UIEventArgs);
        }
    }
}
