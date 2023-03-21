using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;

namespace WpfApp1
{
    internal class GridViewHelper
    {
        public static ObservableCollection<ColumnViewModelBase> GetDynamicColumns(DependencyObject obj)
        {
            return (ObservableCollection<ColumnViewModelBase>)obj.GetValue(DynamicColumnsProperty);
        }

        public static void SetDynamicColumns(DependencyObject obj, ObservableCollection<ColumnViewModelBase> value)
        {
            obj.SetValue(DynamicColumnsProperty, value);
        }

        // Using a DependencyProperty as the backing store for DynamicColumns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DynamicColumnsProperty =
            DependencyProperty.RegisterAttached("DynamicColumns", typeof(ObservableCollection<ColumnViewModelBase>), typeof(GridViewHelper), new PropertyMetadata(null, DynamicColumnsChanged));

        private static void DynamicColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not GridView gridView) { return; }
            if (e.NewValue is not ObservableCollection<ColumnViewModelBase> columnVms) { return; }

            gridView.Columns.Clear();
            foreach (var columnVm in columnVms)
            {
                gridView.Columns.Add(ToGridViewColumn(columnVm));
            }
            columnVms.CollectionChanged += (_, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        var added = (ColumnViewModelBase)e.NewItems![0]!;
                        gridView.Columns.Add(ToGridViewColumn(added));
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        var removed = e.OldStartingIndex;
                        gridView.Columns.Remove(gridView.Columns[removed]);
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        var replace = e.NewStartingIndex;
                        gridView.Columns[replace] = ToGridViewColumn((ColumnViewModelBase)e.NewItems![0]!);
                        break;
                    case NotifyCollectionChangedAction.Move:
                        gridView.Columns.Move(e.OldStartingIndex, e.NewStartingIndex);
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        gridView.Columns.Clear();
                        break;
                }
            };
        }

        private static GridViewColumn ToGridViewColumn(ColumnViewModelBase columnVm)
        {
            return new GridViewColumn
            {
                Header = columnVm,
                CellTemplate =
                    columnVm.CellTemplateResourceKey is not null ?
                    (DataTemplate)App.Current.Resources[columnVm.CellTemplateResourceKey] :
                    columnVm.DisplayMenber is not null ?
                    CreateContentControlTemplate(columnVm.DisplayMenber) :
                    null,
            };
        }

        private static DataTemplate CreateContentControlTemplate(string contentName)
        {
            var dataTemplateString =
                @$"<DataTemplate><ContentControl Content=""{{Binding {contentName}}}""/></DataTemplate>";

            ParserContext parserContext = new ParserContext();
            parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            DataTemplate template = (DataTemplate)XamlReader.Parse(dataTemplateString, parserContext);
            return template;
        }
    }
}
