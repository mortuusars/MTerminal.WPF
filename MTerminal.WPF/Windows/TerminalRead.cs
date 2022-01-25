using System.Windows.Controls;
using System.Windows.Input;

namespace MTerminal.WPF.Windows;

internal class TerminalRead
{
    private bool _charEntered, _lineEntered, _keyPressed;

    private char _char = char.MinValue;
    private string _line = string.Empty;
    private (Key, ModifierKeys) _hotkey = (Key.None, ModifierKeys.None);

    private readonly TextBox _textBox;
    private readonly TerminalWindow _window;

    public TerminalRead(TextBox textBox, TerminalWindow window)
    {
        _textBox = textBox;
        _window = window;
    }

    //TODO: Indicator for read state.

    public async Task<char> Read()
    {
        _textBox.PreviewTextInput += CommandBox_PreviewTextInput;
        _window.IsReadingInput = true;

        while (!_charEntered)
        {
            await Task.Delay(25);
        }

        _window.IsReadingInput = false;
        _textBox.PreviewTextInput -= CommandBox_PreviewTextInput;

        return _char;
    }

    private void CommandBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (e.Text.Length > 0)
        {
            _char = e.Text[0];
            _charEntered = true;
        }
    }

    public async Task<string> ReadLine()
    {
        _textBox.PreviewKeyDown += CommandBox_PreviewKeyDown_Enter;
        _window.IsReadingInput = true;

        while (!_lineEntered)
        {
            await Task.Delay(25);
        }

        _window.IsReadingInput = false;
        _textBox.PreviewKeyDown -= CommandBox_PreviewKeyDown_Enter;

        return _line;
    }

    private void CommandBox_PreviewKeyDown_Enter(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            _line = _textBox.Text;
            _lineEntered = true;
            e.Handled = true;
            _textBox.Text = string.Empty;
        }
    }

    public async Task<(Key key, ModifierKeys modifiers)> ReadKey()
    {
        _textBox.PreviewKeyDown += CommandBox_PreviewKeyDown_Hotkey;
        _window.IsReadingInput = true;

        while (!_keyPressed)
        {
            await Task.Delay(25);
        }

        _window.IsReadingInput = false;
        _textBox.PreviewKeyDown -= CommandBox_PreviewKeyDown_Hotkey;

        return _hotkey;
    }

    private void CommandBox_PreviewKeyDown_Hotkey(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.LeftCtrl &&
                e.Key != Key.RightCtrl &&
                e.Key != Key.LeftAlt &&
                e.Key != Key.RightAlt &&
                e.Key != Key.LeftShift &&
                e.Key != Key.RightShift &&
                e.Key != Key.LWin &&
                e.Key != Key.RWin &&
                e.Key != Key.Clear &&
                e.Key != Key.OemClear &&
                e.Key != Key.Apps)
        {
            _hotkey = (e.Key, Keyboard.Modifiers);
            _keyPressed = true;
        }
    }
}
