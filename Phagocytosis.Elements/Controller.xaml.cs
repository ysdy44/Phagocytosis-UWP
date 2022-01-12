﻿using System;
using System.Numerics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Represents a controller, that controls velocity.
    /// </summary>
    public sealed partial class Controller : UserControl
    {

        //@Delegate
        /// <summary> Occurs when vector changed. </summary>
        public event EventHandler<Vector2> VectorChanged;
        /// <summary> Occurs when moved. </summary>
        public event EventHandler<Vector2> Moved;
        /// <summary> Occurs when virtual key changed. </summary>
        public event EventHandler<VirtualKey> KeyChanged;
        /// <summary> Occurs when gamepad key B changed. </summary>
        public event EventHandler<bool> GamepadBChanged;

        //@Converter
        private double LeftConverter(Vector2 value) => (value.X + 1) * 100 - 25;
        private double TopConverter(Vector2 value) => (value.Y + 1) * 100 - 25;

        #region DependencyProperty


        /// <summary> Gets or sets Key of <see cref = "Controller" />. </summary>
        public VirtualKey Key
        {
            get => (VirtualKey)base.GetValue(KeyProperty);
            set => base.SetValue(KeyProperty, value);
        }
        /// <summary> Identifies the <see cref = "Controller.Key" /> dependency property. </summary>
        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(nameof(Key), typeof(VirtualKey), typeof(Controller), new PropertyMetadata(VirtualKey.None, (sender, e) =>
        {
            Controller control = (Controller)sender;

            if (e.NewValue is VirtualKey value)
            {
                control.KeyChanged?.Invoke(control, value); // Delegate
            }
        }));


        /// <summary> Gets or sets vector of <see cref = "Controller" />. </summary>
        public Vector2 Vector
        {
            get => (Vector2)base.GetValue(VectorProperty);
            set => base.SetValue(VectorProperty, value);
        }
        /// <summary> Identifies the <see cref = "Controller.Vector" /> dependency property. </summary>
        public static readonly DependencyProperty VectorProperty = DependencyProperty.Register(nameof(Vector), typeof(Vector2), typeof(Controller), new PropertyMetadata(Vector2.Zero, (sender, e) =>
        {
            Controller control = (Controller)sender;

            if (e.NewValue is Vector2 value)
            {
                control.VectorChanged?.Invoke(control, value); // Delegate
            }
        }));


        /// <summary> Gets or sets vector of <see cref = "Controller.PrimaryButton" />. </summary>
        public Vector2 VectorCore
        {
            get => (Vector2)base.GetValue(VectorCoreProperty);
            set => base.SetValue(VectorCoreProperty, value);
        }
        /// <summary> Identifies the <see cref = "Controller.VectorCore" /> dependency property. </summary>
        public static readonly DependencyProperty VectorCoreProperty = DependencyProperty.Register(nameof(VectorCore), typeof(Vector2), typeof(Controller), new PropertyMetadata(Vector2.Zero));


        #endregion

        readonly ControllerVelocity VectorVelocity = new ControllerVelocity();
        readonly ControllerVelocity MoveVelocity = new ControllerVelocity();
        public bool IsGamepad { get; private set; }
        bool IsGamepadB;

        //@Construct
        public Controller()
        {
            this.InitializeComponent();
            base.Unloaded += (s, e) =>
            {
                Window.Current.CoreWindow.KeyUp -= this.CoreWindow_KeyUp;
                Window.Current.CoreWindow.KeyDown -= this.CoreWindow_KeyDown;
                Windows.Gaming.Input.Gamepad.GamepadRemoved -= this.Gamepad_GamepadRemoved;
                Windows.Gaming.Input.Gamepad.GamepadAdded -= this.Gamepad_GamepadAdded;
            };
            base.Loaded += (s, e) =>
            {
                this.CheckGamepad();
                Window.Current.CoreWindow.KeyUp += this.CoreWindow_KeyUp;
                Window.Current.CoreWindow.KeyDown += this.CoreWindow_KeyDown;
                Windows.Gaming.Input.Gamepad.GamepadRemoved += this.Gamepad_GamepadRemoved;
                Windows.Gaming.Input.Gamepad.GamepadAdded += this.Gamepad_GamepadAdded;
            };

            this.Control.ManipulationStarted += (s, e) =>
            {

            };
            this.Control.ManipulationDelta += (s, e) =>
            {
                Vector2 vector = new Vector2((float)e.Position.X / 100 - 1, (float)e.Position.Y / 100 - 1);
                float length = vector.Length();

                if (length <= 0.5)
                {
                    this.Vector = Vector2.Zero;
                    this.VectorCore = vector;
                }
                else if (length >= 1)
                    this.Vector = this.VectorCore = vector / length;
                else
                    this.Vector = this.VectorCore = vector;
            };
            this.Control.ManipulationCompleted += (s, e) =>
            {
                this.Vector = this.VectorCore = Vector2.Zero;
            };
        }

        #region Gamepad


        public void CheckGamepad()
        {
            this.IsGamepad = Windows.Gaming.Input.Gamepad.Gamepads.Count != 0;
            base.Visibility = this.IsGamepad ? Visibility.Collapsed : Visibility.Visible;
        }

        private async void Gamepad_GamepadRemoved(object sender, Windows.Gaming.Input.Gamepad e)
        {
            if (this.IsGamepad == false) return;
            this.IsGamepad = false;
            await base.Dispatcher.RunIdleAsync((a) =>
            {
                base.Visibility = Visibility.Visible;
            });
        }
        private async void Gamepad_GamepadAdded(object sender, Windows.Gaming.Input.Gamepad e)
        {
            if (this.IsGamepad == true) return;
            this.IsGamepad = true;
            await base.Dispatcher.RunIdleAsync((a) =>
            {
                base.Visibility = Visibility.Collapsed;
            });
        }

        public Vector2 GetGamepadVector()
        {
            if (this.IsGamepad)
            {
                foreach (Windows.Gaming.Input.Gamepad item in Windows.Gaming.Input.Gamepad.Gamepads)
                {
                    Windows.Gaming.Input.GamepadReading read = item.GetCurrentReading();
                    {
                        bool isGamepadB = read.Buttons == Windows.Gaming.Input.GamepadButtons.B;
                        if (this.IsGamepadB != isGamepadB)
                        {
                            this.IsGamepadB = isGamepadB;
                            this.GamepadBChanged?.Invoke(this, isGamepadB); // Delegate
                        }
                    }
                    return new Vector2((float)read.LeftThumbstickX, -(float)read.LeftThumbstickY);
                }
                this.Gamepad_GamepadRemoved(null, null);
            }

            return Vector2.Zero;
        }


        #endregion

        #region Key


        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.A:
                    this.VectorVelocity.IsLeft = false;
                    break;
                case VirtualKey.W:
                    this.VectorVelocity.IsTop = false;
                    break;
                case VirtualKey.D:
                    this.VectorVelocity.IsRight = false;
                    break;
                case VirtualKey.S:
                    this.VectorVelocity.IsBottom = false;
                    break;

                case VirtualKey.Left:
                    this.MoveVelocity.IsLeft = false;
                    break;
                case VirtualKey.Up:
                    this.MoveVelocity.IsTop = false;
                    break;
                case VirtualKey.Right:
                    this.MoveVelocity.IsRight = false;
                    break;
                case VirtualKey.Down:
                    this.MoveVelocity.IsBottom = false;
                    break;

                default:
                    if (this.Key == args.VirtualKey)
                    {
                        this.Key = VirtualKey.None;
                    }
                    break;
            }

            this.Vector = this.VectorCore = this.VectorVelocity.GetVector();
            this.Moved?.Invoke(this, this.MoveVelocity.GetVector()); // Delegate
        }
        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.A:
                    this.VectorVelocity.IsLeft = true;
                    this.VectorVelocity.IsLeftFrist = true;
                    break;
                case VirtualKey.W:
                    this.VectorVelocity.IsTop = true;
                    this.VectorVelocity.IsTopFrist = true;
                    break;
                case VirtualKey.D:
                    this.VectorVelocity.IsRight = true;
                    this.VectorVelocity.IsLeftFrist = false;
                    break;
                case VirtualKey.S:
                    this.VectorVelocity.IsBottom = true;
                    this.VectorVelocity.IsTopFrist = false;
                    break;

                case VirtualKey.Left:
                    this.MoveVelocity.IsLeft = true;
                    this.MoveVelocity.IsLeftFrist = true;
                    break;
                case VirtualKey.Up:
                    this.MoveVelocity.IsTop = true;
                    this.MoveVelocity.IsTopFrist = true;
                    break;
                case VirtualKey.Right:
                    this.MoveVelocity.IsRight = true;
                    this.MoveVelocity.IsLeftFrist = false;
                    break;
                case VirtualKey.Down:
                    this.MoveVelocity.IsBottom = true;
                    this.MoveVelocity.IsTopFrist = false;
                    break;

                default:
                    this.Key = args.VirtualKey;
                    break;
            }

            this.Vector = this.VectorCore = this.VectorVelocity.GetVector();
            this.Moved?.Invoke(this, this.MoveVelocity.GetVector()); // Delegate
        }


        #endregion

    }
}