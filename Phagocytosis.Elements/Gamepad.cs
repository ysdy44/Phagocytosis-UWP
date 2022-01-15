using System;
using System.Collections.Generic;
using System.Numerics;
using Windows.UI.Xaml;

namespace Phagocytosis.Elements
{
    public sealed class Gamepad
    {

        //@Instance
        public static Lazy<Gamepad> Lazy = new Lazy<Gamepad>();
        public static Gamepad Instance => Gamepad.Lazy.Value;

        public bool IsGamepad { get; private set; }

        //@Construct
        /// <summary>
        /// Initializes a Gamepad. 
        /// </summary>
        public Gamepad()
        {
            this.IsGamepad = Windows.Gaming.Input.Gamepad.Gamepads.Count != 0;
            Windows.Gaming.Input.Gamepad.GamepadRemoved += this.GamepadRemoved;
            Windows.Gaming.Input.Gamepad.GamepadAdded += this.GamepadAdded;
        }
        ~Gamepad()
        {
            Windows.Gaming.Input.Gamepad.GamepadRemoved -= this.GamepadRemoved;
            Windows.Gaming.Input.Gamepad.GamepadAdded -= this.GamepadAdded;
        }

        private void GamepadRemoved(object sender, Windows.Gaming.Input.Gamepad e)
        {
            if (this.IsGamepad == false) return;
            this.IsGamepad = false;
        }
        private void GamepadAdded(object sender, Windows.Gaming.Input.Gamepad e)
        {
            if (this.IsGamepad == true) return;
            this.IsGamepad = true;
        }

        public Windows.Gaming.Input.GamepadReading GetReading(FlowDirection direction)
        {
            foreach (Windows.Gaming.Input.Gamepad item in Windows.Gaming.Input.Gamepad.Gamepads)
            {
                return item.GetCurrentReading();
            }

            return default;
        }

    }
}