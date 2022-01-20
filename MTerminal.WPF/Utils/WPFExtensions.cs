using System.Windows;
using System.Windows.Media;

namespace MTerminal.WPF;

internal static class WPFExtensions
{
    public static T? GetDescendantByType<T>(this Visual element) where T : class
    {
        if (element == null)
            return default;

        if (element.GetType() == typeof(T))
            return element as T;

        T? foundElement = null;

        if (element is FrameworkElement)
            ((FrameworkElement)element).ApplyTemplate();

        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
        {
            Visual? visual = VisualTreeHelper.GetChild(element, i) as Visual;
            foundElement = visual?.GetDescendantByType<T>();
            if (foundElement != null)
                break;
        }

        return foundElement;
    }
}
