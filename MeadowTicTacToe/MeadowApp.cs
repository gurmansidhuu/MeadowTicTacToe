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
using System.Threading;
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

            button1 = new PushButton(Device, Device.Pins.D04);
            button1.Clicked += ButtonClicked;

            button2 = new PushButton(Device, Device.Pins.D03);
            button2.Clicked += ButtonClicked;

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

            if (game.position < 0)
            {
                game.position = 0;
            }
            else if (game.position > 8)
            {
                game.position = 8;
            }

        }

        void ButtonClicked(object sender, EventArgs e)
        {

            //Menu & EndGamestate just accept the input
            if (game.gamestate == 1)
            {
                game.gamestate++;

                if(game.turn == 1)
                {
                    joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick2.StopUpdating();
                }
                else
                {
                    joystick2.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick1.StopUpdating();
                }

                return;
            }

            if (game.gamestate == 3)
            {
                if (game.position <= 0)
                {
                    game.gamestate = 2;
                }
                else
                {
                    game.gamestate = 1;
                }
                
            }

            if (game.gamestate == 2)
            {
                //Check if valid for Gamestate (Game)
                if (game.turn == 0 && game.CheckMove())
                {
                    game.turn = 1;

                    joystick2.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick1.StopUpdating();
                }
                else if (game.turn == 1 && game.CheckMove())
                {
                    game.turn = 0;

                    joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                    joystick2.StopUpdating();
                }
            }

            if(game.gamestate == 3)
            {
                joystick1.StartUpdating(TimeSpan.FromMilliseconds(20));
                joystick2.StopUpdating();
            }
        }
    }
}