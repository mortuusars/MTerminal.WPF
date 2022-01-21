using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MTerminal.WPF;

internal class Output : IOutput
{
    public void Write(string value)
    {
        throw new NotImplementedException();
    }

    public void Write(string value, Color color)
    {
        throw new NotImplementedException();
    }
}
