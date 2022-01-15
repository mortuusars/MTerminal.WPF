using MTerminal.WPF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MTerminalWPFDemo;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Terminal.Commands.Add(new TerminalCommand("crash")
        {
            Description = "[message] | Tries to crash the app",
            Action = (p) => throw new Exception(p)
        });

        Terminal.Commands.Add(new TerminalCommand("exit")
        {
            Description = "Shutdowns the app",
            Action = (p) => Shutdown()
        });

        Terminal.ShowWindow();
        Terminal.WriteLine("Writing to a Terminal", Colors.Aqua);
        Terminal.WriteLine("Writing to a Terminal", Colors.IndianRed);

        MTerminal.WPF.Windows.TerminalWindow window = new MTerminal.WPF.Windows.TerminalWindow();
        window.Show();
    }
}