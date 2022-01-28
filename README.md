<div align="center">
[![.NET Version 6.0](https://img.shields.io/badge/.NET-6.0-green)](https://www.nuget.org/packages/MTerminal.WPF/)
</div>


<div align="center">
  <h2>MTerminal provides a "console" for the WPF apps.</h2>
  
  Terminal is a simple WPF window that can display text and has an input box for commands.
  Can be used as more user-friendly Command-Line Interface for simple apps or as a convenient debug tool for your WPF app.
</div>

### Features:
  
- Similar to the `System.Console`
- Can be used as target output of a `Console`
- Supports colored text
- Autocomplete for the commands
- Customizable style of the window
- Has two built-in commands:
  - **help**: shows information about available commands
  - **clear**: clears the screen

## Basic usage:
```csharp
// Show the Terminal window:
Terminal.Show();

//Write a colored message to the Terminal:
Terminal.WriteLine("Hello World!", Colors.LightBlue);
```

## **Commands overview:**

Basic working command consists of two elements:
- Command name/ID (this is what typed to invoke it)
- `Action<string[]>` - code that will be executed when command is invoked.

Command also has some additional properties but they are not essential.

### **Arguments**

When command is invoked - all text that was typed after a command id will be passed to the command as an argument.
Args work as the regular app args do:
 - It's a array of strings separated by spaces
 - Text encased in quotes counts as 1 argument

Adding a command:
```csharp
Terminal.Commands.Add(new TerminalCommand("exit", "Exits the app", (args) => App.Shutdown()));
```

## **Read functionality:**

Terminal supports **Read**, **ReadLine** and **ReadKey**.
They work as expected: 
- Read returns next char that is typed in by user;
- ReadLine waits for the __return__ key, after which it returns text that was inputed;
- ReadKey returns a key that was pressed;

But - they are implemented as Tasks, which means to use it properly they should be awaited.

❗**Warning:**  if called from the UI thread with waiting (such as GetAwaiter().GetResult()) they will hang the app in a deadlock, as the UI thread is waiting and cannot process user input.
