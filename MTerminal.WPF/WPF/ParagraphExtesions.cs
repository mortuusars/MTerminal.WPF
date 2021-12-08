using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;

namespace MTerminal.WPF.WPF;

public static class ParagraphExtesions
{

    public static ObservableCollection<Inline> GetBindableInlines(DependencyObject obj)
    {
        return (ObservableCollection<Inline>)obj.GetValue(BindableInlinesProperty);
    }

    public static void SetBindableInlines(DependencyObject obj, ObservableCollection<Inline> value)
    {
        obj.SetValue(BindableInlinesProperty, value);

        //value.CollectionChanged += (o, e) =>
        //{
        //    var paragraph = (Paragraph)obj;
        //    paragraph.Inlines.AddRange(e.NewItems);
        //};
    }

    public static readonly DependencyProperty BindableInlinesProperty =
        DependencyProperty.RegisterAttached("BindableInlines", typeof(ObservableCollection<Inline>), typeof(ParagraphExtesions), new PropertyMetadata(null, OnBindableInlinesChanged));

    private static void OnBindableInlinesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var oldCollection = e.OldValue as ObservableCollection<Inline>;
        var newCollection = e.NewValue as ObservableCollection<Inline>;

        if (oldCollection != null)
        {
            oldCollection.CollectionChanged -= CollectionChanged;
        }
        if (newCollection != null)
        {
            newCollection.CollectionChanged += CollectionChanged;
        }

        ObservableCollection<Inline> inlines = e.NewValue as ObservableCollection<Inline> ?? throw new InvalidCastException("BindableInlines was not a type of ObservableCollection<Inline>");

        var paragraph = d as Paragraph ?? throw new InvalidCastException("Object for bindable inlines must be of type 'Paragraph'");
        paragraph.Inlines.Clear();
        paragraph.Inlines.AddRange((IList<Inline>)e.NewValue);

        inlines.CollectionChanged += (s, e) =>
        {
            var paragraph = d as Paragraph ?? throw new InvalidCastException("Object for bindable inlines must be of type 'Paragraph'");
            paragraph.Inlines.AddRange(e.NewItems);
        };
    }

    private static void CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        throw new NotImplementedException();
    }
}