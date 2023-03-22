using System;
using System.ComponentModel;

namespace WpfApp1
{
    internal class StringColumnViewModel : ColumnViewModelBase
    {
        private string propertyName;

        public override string? DisplayMenber => this.propertyName;
        public string HeaderText { get; }
        private string filterText = string.Empty;
        public string FilterText 
        {
            get => this.filterText;
            set
            {
                this.SetProperty(ref this.filterText, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        public override bool IsFiltering => !string.IsNullOrEmpty(this.FilterText);

        public StringColumnViewModel(string propertyName, string headerText)
        {
            this.propertyName = propertyName;
            this.HeaderText = headerText;
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription($"{this.propertyName}.{nameof(StringViewModel.Value)}", direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            return
                (itemVm.GetType().GetProperty(this.propertyName)!.GetValue(itemVm) as StringViewModel)?.Filter(this.FilterText)
                ?? false;
        }

        public override GroupDescription GroupOverride()
        {
            throw new NotImplementedException();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.FilterText = string.Empty;
        }
    }
}
