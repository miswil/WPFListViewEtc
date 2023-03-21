using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Windows.Input;

namespace WpfApp1
{
    internal class UnlimitedSelectionItemViewModel : ObservableObject
    {
        public object? Value { get; }

        private bool isSelected;
        public bool IsSelected
        {
            get => this.isSelected;
            set
            {
                this.SetProperty(ref this.isSelected, value);
                this.Selected?.Invoke(this, EventArgs.Empty);
            }
        }

        public ICommand SelectedCommand { get; }

        public event EventHandler? Selected;

        public UnlimitedSelectionItemViewModel(object? value, ICommand filterCommand)
        {
            this.Value = value;
            this.SelectedCommand = filterCommand;
        }
    }
}
