using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Hid;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace MeadowTicTacToe
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV2>
    {
        MicroGraphics graphics;

        AnalogJoystick joystick1;
        AnalogJoystick joystick2;

        PushButton button1;
        PushButton button2;

        Game game;

        public override Task Run()
        {
            Console.WriteLine("Run...");

            game = new Game(graphics);

            StartApp();

            return base.Run();
        }

        public override Task Initialize()
        {
            Console.WriteLine("Initialize...");

            //ST7789 Display Setup
            var onboardLed = new RgbPwmLed(
            device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            var config = new SpiClockConfiguration(
                speed: new Frequency(48000, Frequency.UnitType.Kilohertz),
                mode: SpiClockConfiguration.Mode.Mode3);
            var spiBus = Device.CreateSpiBus(
            clock: Device.Pins.SCK,
                copi: Device.Pins.MOSI,
                cipo: Device.Pins.MISO,
                config: config);
            var st7789 = new St7789
            (
            device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.D02,
                dcPin: Device.Pins.D01,
                resetPin: Device.Pins.D00,
                width: 240, height: 240
            );

            graphics = new MicroGraphics(st7789);
            graphics.CurrentFont = new Font12x16();

            //Setup Buttons on AnalogSticks
            button1 = new PushButton(Device, Device.Pins.D04);
            button1.Clicked += ButtonClicked1;


            button2 = new PushButton(Device, Device.Pins.D03);
            button2.Clicked += ButtonClicked2;
            

            //Setup Analog Sticks
            joystick1 = new AnalogJoystick(
                       Device.CreateAnalogInputPort(Device.Pins.A01, 1, TimeSpan.FromMilliseconds(10), new Voltage(3.3)),
                       Device.CreateAnalogInputPort(Device.Pins.A00, 1, TimeSpan.FromMilliseconds(10), new Voltage(3.3)),
                       null);

            _ = joystick1.SetCenterPosition();

            if (joystick1 != null)
            {
                joystick1.Updated += JoystickUpdated;
            }

            joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));

            joystick2 = new AnalogJoystick(
                       Device.CreateAnalogInputPort(Device.Pins.A03, 1, TimeSpan.FromMilliseconds(10), new Voltage(3.3)),
                       Device.CreateAnalogInputPort(Device.Pins.A02, 1, TimeSpan.FromMilliseconds(10), new Voltage(3.3)),
                       null);

            _ = joystick2.SetCenterPosition();

            if (joystick2 != null)
            {
                joystick2.Updated += JoystickUpdated;
            }

            onboardLed.SetColor(Color.Green);

            return base.Initialize();
        }

        private void StartApp()
        {
            game.StartGame();

        }

            //Joystick Function - Only would move once Horizontially and/or Vertically per 500ms
        void JoystickUpdated(object sender, IChangeResult<AnalogJoystickPosition> e)
        {
            //Joystick Input - Left
            if (!game.horzIN)
            {
                if (e.New.Horizontal > 0.8)
                {
                    if (game.position != 0)
                    {
                        game.position--;
                    }
                    game.horzIN = true;
                }

                //Joystick Input - Right
                else if (e.New.Horizontal < -0.8)
                {
                    if (game.position != 8)
                    {
                        game.position++;
                    }
                    game.horzIN = true;
                }
            }

            // Joystick Input - Down
            if (!game.vertIN)
            {
                if (e.New.Vertical > 0.8)
                {
                    if (game.position != 6 || game.position != 7 || game.position != 8)
                    {
                        game.position += 3;
                    }
                    game.vertIN = true;
                }

                // Joystick Input - Up
                else if (e.New.Vertical < -0.8)
                {
                    if (game.position != 0 || game.position != 1 || game.position != 2)
                    {
                        game.position -= 3;
                    }
                    game.vertIN = true;
                }
            }

            //Keep position between 0-8
            if (game.position < 0)
            {
                game.position = 0;
            }
            else if (game.position > 8)
            {
                game.position = 8;
            }

        }

        //Button 1 Function - Includes all cases from all gamestates
        void ButtonClicked1(object sender, EventArgs e)
        {
            //GameOver Inputs
            if (game.gamestate == 3)
            {
                if (game.position <= 0)
                {
                    game.gamestate = 2;

                    if (game.turn == 0)
                    {
                        joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick2.StopUpdating();
                    }
                    else
                    {
                        joystick2.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick1.StopUpdating();
                    }

                }
                else
                {
                    game.gamestate = 1;

                    joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick2.StopUpdating();
                }
            }

            else if (game.turn == 0)
            {
                //Menu accept the input and changes which joystick to get input from
                if (game.gamestate == 1)
                {
                    game.gamestate++;

                    if (game.turn == 0)
                    {
                        joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick2.StopUpdating();
                    }
                    else
                    {
                        joystick2.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick1.StopUpdating();
                    }
                }


                //In Game Inputs
                else if (game.gamestate == 2)
                {
                    //Check if valid input and change active joystick
                    if (game.CheckMove())
                    {
                        game.turn = 1;

                        joystick2.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick1.StopUpdating();
                    }
                }

                //If gamestate changes to gameover joystick 1 activates
                if (game.gamestate == 3)
                {
                    joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick2.StopUpdating();
                }
            }
        }
        

        //Button 2 Function - Only used in game
        void ButtonClicked2(object sender, EventArgs e)
        {
            if (game.turn == 1)
            {
                //In Game Inputs
                if (game.gamestate == 2)
                {
                    //Check if valid input and change active joystick
                    if (game.CheckMove())
                    {
                        game.turn = 0;

                        joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                        joystick2.StopUpdating();
                    }
                }

                //If gamestate changes to gameover joystick 1 activates
                if (game.gamestate == 3)
                {
                    joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick2.StopUpdating();
                }
            }
        }
    }
}