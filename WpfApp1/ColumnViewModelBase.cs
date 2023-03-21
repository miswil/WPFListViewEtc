using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Windows.Input;

namespace WpfApp1
{
    internal abstract class ColumnViewModelBase : ObservableObject
    {
        public virtual string? DisplayMenber { get; } = null;
        public virtual object? CellTemplateResourceKey { get; } = null;
        public abstract bool IsFiltering { get; }

        private bool isSorting;
        public bool IsSorting 
        { 
            get => this.isSorting;
            private set => this.SetProperty(ref this.isSorting, value); 
        }
        private ListSortDirection? sortDirection; 
        public ListSortDirection? SortDirection
        {
            get => this.sortDirection;
            private set => this.SetProperty(ref this.sortDirection, value);
        }

        private bool grouping;
        public bool IsGrouping
        {
            get => this.grouping;
            set => this.SetProperty(ref this.grouping, value);
        }

        public ICommand FilterCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand GroupCommand { get; }
        public ICommand ResetFilterAndGroupCommand { get; }

        public event EventHandler? FilterRequested;
        public event EventHandler<ListSortDirection?>? SortRequested;
        public event EventHandler? GroupingRequested;

        public ColumnViewModelBase()
        {
            this.SortCommand = new RelayCommand<ListSortDirection?>(this.SortCommandExecute);
            this.FilterCommand = new RelayCommand(this.FilterCommandExecute);
            this.GroupCommand = new RelayCommand(this.GroupCommandExecute);
            this.ResetFilterAndGroupCommand = new RelayCommand(this.ResetFilterCommandExecute);
        }

        public bool Filter(object itemVm)
        {
            return this.FilterOverride(itemVm);
        }
        protected abstract bool FilterOverride(object itemVm);

        public SortDescription Sort(ListSortDirection direction)
        {
            this.IsSorting = true;
            this.SortDirection = direction;
            return this.SortOverride(direction);
        }
        protected abstract SortDescription SortOverride(ListSortDirection direction);

        public void ResetSort()
        {
            this.IsSorting = false;
            this.SortDirection = null;
        }

        public GroupDescription Group()
        {
            return this.GroupOverride();
        }

        public abstract GroupDescription GroupOverride();

        public void ResetGroup()
        {
            this.IsGrouping = false;
        }

        private void FilterCommandExecute()
        {
            this.FilterRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SortCommandExecute(ListSortDirection? sortDirection)
        {
            this.SortRequested?.Invoke(this, sortDirection);
        }

        private void GroupCommandExecute()
        {
            this.GroupingRequested?.Invoke(this, EventArgs.Empty); ;
        }

        private void ResetFilterCommandExecute()
        {
            var isThisFiltering = this.IsFiltering;
            var isThisGoruping = this.IsGrouping;
            this.ResetFilterAndGroupCommandExecuteOverride();
            if (isThisFiltering)
            {
                this.FilterRequested?.Invoke(this, EventArgs.Empty);
            }
            if (isThisGoruping)
            {
                this.GroupingRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void ResetFilterAndGroupCommandExecuteOverride() { }
    }
}
