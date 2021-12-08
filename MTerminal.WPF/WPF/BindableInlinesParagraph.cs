using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Specialized;

namespace MTerminal.WPF.WPF;

public class BindableInlinesParagraph : Paragraph
{
    public ObservableCollection<Inline> InlinesCollection
    {
        get { return (ObservableCollection<Inline>)GetValue(InlinesCollectionProperty); }
        set { SetValue(InlinesCollectionProperty, value); }
    }

    public static readonly DependencyProperty InlinesCollectionProperty =
        DependencyProperty.Register(nameof(InlinesCollection), typeof(ObservableCollection<Inline>), typeof(BindableInlinesParagraph), new PropertyMetadata(null, OnPropertyChanged));

    private static SynchronizationContext _syncContext;

    static BindableInlinesParagraph()
    {
        _syncContext = SynchronizationContext.Current ?? throw new NullReferenceException("SynchronizationContext was null when creating BindableInlinesParagraph.");
    }

    private static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var paragraph = sender as BindableInlinesParagraph;

        if (e.OldValue != null)
        {
            var coll = (INotifyCollectionChanged)e.OldValue;
            // Unsubscribe from CollectionChanged on the old collection of the DP-instance (!)
            coll.CollectionChanged -= paragraph.InlineCollectionChanged;
        }

        if (e.NewValue != null)
        {
            var coll = (ObservableCollection<Inline>)e.NewValue;
            // Subscribe to CollectionChanged on the new collection of the DP-instance (!)
            coll.CollectionChanged += paragraph.InlineCollectionChanged;
        }
    }

    private void InlineCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (sender is null)
            throw new NullReferenceException("Inline Collection cannot be null here.");

        if (e.NewItems is null) return;

        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            Inlines.AddRange(e.NewItems);
        }
        else if (e.Action is NotifyCollectionChangedAction.Remove
            or NotifyCollectionChangedAction.Reset
            or NotifyCollectionChangedAction.Move
            or NotifyCollectionChangedAction.Replace)
        {
            Inlines.Clear();
            Inlines.AddRange((ObservableCollection<Inline>)sender);
        }
    }
}