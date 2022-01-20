using MTerminal.WPF;
using System;
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

        // Changing window style:
        Terminal.Style.WindowTitle = "Demo Terminal";
        // Changing the default color of the text:
        //Terminal.Style.Foreground = Color.FromRgb(31, 194, 85);

        // Set Background to custom color:
        //Terminal.Style.Background = new RadialGradientBrush(Color.FromRgb(20, 30, 26), Color.FromRgb(12, 20, 17)) { Center = new Point(0.5, 0.5) };

        // Set Background to image:
        //BitmapImage img = new BitmapImage(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Images/terminal_bg.png", UriKind.Absolute));
        //Terminal.Style.Background = new ImageBrush(img);

        // Set custom font
        //Terminal.Style.FontFamily = new FontFamily(new Uri("pack://application:,,,/MTerminalWPFDemo;component/Fonts/"), "./#Classic Console Neue");

        // Adding custom commands:
        Terminal.Commands.Add(new TerminalCommand("crash")
        {
            Description = "[message] | Tries to crash the app",
            Action = (p) => throw new Exception(p)
        });

        Terminal.Commands.Add(new TerminalCommand("exit")
        {
            Description = "Shutdowns the app",
            Action = (_) => Shutdown()
        });

        // Redirect console output to the terminal:
        Console.SetOut(Terminal.Out);
        Console.WriteLine("Written from a default console!");


        Terminal.WriteLine("Writing to a Terminal before showing.", Colors.Aqua);
     
        // Show the window:
        Terminal.ShowWindow();
        
        Terminal.WriteLine("Writing to a Terminal after showing.", Colors.Fuchsia);

        //for (int i = 0; i < 20; i++)
        //{
        //    Terminal.WriteLine("test " + i);
        //}

        //Task.Run(PrintLines);

        Terminal.Write("WriteOne");
        Terminal.Write("WriteTwo");
    }

    public async void PrintLines()
    {
        while(true)
        {
            Terminal.Write("a");
            await Task.Delay(200);
        }
    }
}