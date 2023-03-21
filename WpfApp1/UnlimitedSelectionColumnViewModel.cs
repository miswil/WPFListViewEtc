using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace WpfApp1
{
    internal class UnlimitedSelectionColumnViewModel : ColumnViewModelBase, IDisposable
    {
        private INotifyCollectionChanged? items;
        private readonly string propertyName;

        public override string? DisplayMenber => this.propertyName;

        public ObservableCollection<UnlimitedSelectionItemViewModel> Selections { get; } = new();

        public override bool IsFiltering => this.Selections.Any(s => s.IsSelected);

        public UnlimitedSelectionColumnViewModel(string propertyName, IEnumerable items)
        {
            this.propertyName = propertyName;

            MakeFilters(items);
            if (items is INotifyCollectionChanged ncc)
            {
                ncc.CollectionChanged += this.Items_CollectionChanged;
                this.items = ncc;
            }
        }

        public void Dispose()
        {
            if (this.items is not null)
            {
                this.items.CollectionChanged -= this.Items_CollectionChanged;
            }
        }

        protected override bool FilterOverride(object itemVm)
        {
            if (!this.IsFiltering) { return true; }
            var property = itemVm.GetType().GetProperty(this.propertyName);
            if (property is null)
            {
                return false;
            }
            var value = property.GetValue(itemVm);
            return this.Selections.FirstOrDefault(s => object.Equals(s.Value, value))?.IsSelected ?? false;
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(this.propertyName, direction);
        }

        public override GroupDescription GroupOverride()
        {
            return new PropertyGroupDescription(this.propertyName);
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            foreach (var selection in this.Selections)
            {
                selection.IsSelected = false;
            }
            this.IsGrouping = false;
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    MakeFilters((IEnumerable)sender);
                    break;
            }
        }

        void MakeFilters(IEnumerable items)
        {
            foreach (var selection in this.Selections)
            {
                selection.Selected -= this.Selection_Selected;
            }
            this.Selections.Clear();
            foreach (var value in items.Cast<object>()
                .Select(item => {
                    var property = item.GetType().GetProperty(this.propertyName);
                    if (property is null)
                    {
                        throw new InvalidOperationException();
                    }
                    var value = property.GetValue(item);
                    return value;
                }).OrderBy(v => v)
                .Distinct())
            {
                var vm = new UnlimitedSelectionItemViewModel(value, this.FilterCommand);
                this.Selections.Add(vm);
                vm.Selected += this.Selection_Selected;
            }
        }

        private void Selection_Selected(object? sender, EventArgs e)
        {
            this.OnPropertyChanged(nameof(IsFiltering));
        }
    }
}
