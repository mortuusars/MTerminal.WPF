using MTerminal.WPF;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MTerminalWPFDemo;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Terminal.Buffer = 500;       - Window is not created yet
        // Terminal.Show();             - Window is created and shown

        // Terminal.WriteLine("asd");   - Write

        // Terminal.Close();            - Window is closed
        // Terminal.IsOpen              - false

        // Terminal.Write("a");         - nothing happens

        // Terminal.Show();             - Create and show again, display is cleared
        // Terminal.Write("a");         - Write
        // Terminal.Hide();             - Hide, but still able to write



        // Changing window style:
        Terminal.Style.WindowTitle = "Demo Terminal";
        // Changing the default color of the text:
        //Terminal.Style.Foreground = Color.FromRgb(31, 194, 85);

        // Set Background to custom color:
        //Terminal.Style.Background = new RadialGradientBrush(Color.FromRgb(22, 40, 37), Color.FromRgb(10, 20, 16)) { Center = new Point(0.5, 0.5), RadiusX=0.8, RadiusY=1.2 };

        // Set Background to image:
        //BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Images/terminal_bg.png", UriKind.Absolute));
        //var imageBrush = new ImageBrush(img);
        //imageBrush.Stretch = Stretch.UniformToFill;
        //Terminal.Style.Background = imageBrush;

        // Set custom font
        //Terminal.Style.FontFamily = new FontFamily(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Fonts/"), "./#Classic Console Neue");

        // Adding commands:
        Terminal.Commands.Add(new TerminalCommand("crash")
        {
            Description = "[message] | Tries to crash the app",
            Action = (args) => throw new Exception(string.Join(' ', args)),
        }
        .AddAlias("throw")
        .AddAlias("fail"));

        Terminal.Commands.Add(new TerminalCommand("exit")
        {
            Description = "Exits the app",
            Action = (_) => Shutdown()
        });

        Terminal.Commands.Add(new TerminalCommand("write", "", (args) => Terminal.WriteLine(string.Join(' ', args))));
        Terminal.Commands.Add(new TerminalCommand("find", "", (args) => 
        {
            var cmd = Terminal.Commands.Find(string.Join(' ', args));
            Terminal.WriteLine(cmd, Colors.AntiqueWhite);
        }));

        Terminal.Commands.Add(new TerminalCommand("close", "", (_) => Terminal.Close()));
        Terminal.Commands.Add(new TerminalCommand("hide", "", (_) => Terminal.Hide()));
        
        // Show the window:
        Terminal.Show();

        Terminal.WriteLine("Writing to a Terminal.", Colors.Fuchsia);

        // Check terminal state:
        bool isOpen = Terminal.IsOpen;
        bool isVisible = Terminal.IsVisible;

        // Redirect console output to the terminal:
        Console.SetOut(Terminal.Out);
        Console.WriteLine("Written from a default console!");
        //Console.Clear();  - IOException. Some System.Console methods fail when output is redirected.
    }
}