using MTerminal.WPF;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;

namespace MTerminalWPFDemo;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Set max number of lines:
        Terminal.BufferCapacity = 400;

        // Changing window style:
        Terminal.Style.WindowTitle = "Demo Terminal";
        // Changing the default color of the text:
        Terminal.Style.Foreground = Color.FromRgb(31, 194, 85);

        // Set Background to custom color:
        //Terminal.Style.Background = new RadialGradientBrush(Color.FromRgb(22, 40, 37), Color.FromRgb(10, 20, 16)) { Center = new Point(0.5, 0.5), RadiusX=0.8, RadiusY=1.2 };

        // Set Background to image:
        BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Images/terminal_bg.png", UriKind.Absolute));
        var imageBrush = new ImageBrush(img);
        imageBrush.Stretch = Stretch.UniformToFill;
        Terminal.Style.Background = imageBrush;

        // Set custom font
        Terminal.Style.FontFamily = new FontFamily(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Fonts/"), "./#Classic Console Neue");

        // Adding commands:
        Terminal.Commands.Add(new TerminalCommand("exit")
        {
            Description = "Exits the app",
            Action = (_) => Shutdown()
        });

        Terminal.Commands.Add(new TerminalCommand("read", "Reads user input", (_) => Task.Run(Read)));

        // Show the window:
        Terminal.Show();

        // Check terminal state:
        bool isOpen = Terminal.IsOpen;
        bool isVisible = Terminal.IsVisible;

        // Writing:
        Terminal.WriteLine("Writing to a Terminal.", Colors.Fuchsia);

        // Redirect console output to the terminal:
        Console.SetOut(Terminal.Out);
        Console.WriteLine("Console output was redirected to the Terminal!");
        //Console.Clear();  - IOException. Some System.Console methods fail when output is redirected.
    }

    public async void Read()
    {
        Terminal.WriteLine("Enter the character:", Colors.LightBlue);
        char c = await Terminal.Read();
        Terminal.WriteLine($"You entered: '{c}'");

        Terminal.WriteLine("Enter the line:", Colors.CadetBlue);
        string line = await Terminal.ReadLine();
        Terminal.WriteLine($"You entered: \"{line}\"");

        Terminal.WriteLine("Press any key:", Colors.LightGreen);
        var (key, modifiers) = await Terminal.ReadKey();
        string pressedModifiers = modifiers != ModifierKeys.None ? modifiers.ToString() + " + " : "";
        Terminal.Write("You pressed: ");
        Terminal.WriteLine($"{pressedModifiers}{key}", Colors.Gold);
    }
}