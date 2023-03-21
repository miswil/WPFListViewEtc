using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    internal class BloodTypeColumnViewModel : ColumnViewModelBase
    {
        public override object CellTemplateResourceKey => "BloodTypeCell";
        private bool showAType;
        public bool ShowAType
        {
            get => this.showAType;
            set
            {
                this.SetProperty(ref this.showAType, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool showBType;
        public bool ShowBType
        {
            get => this.showBType;
            set
            {
                this.SetProperty(ref this.showBType, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool showABType;
        public bool ShowABType
        {
            get => this.showABType;
            set
            {
                this.SetProperty(ref this.showABType, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool showOType;
        public bool ShowOType
        {
            get => this.showOType;
            set
            {
                this.SetProperty(ref this.showOType, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }

        public override bool IsFiltering =>
            this.ShowAType || this.ShowBType ||
            this.ShowABType || this.ShowOType;

        protected override bool FilterOverride(object itemVm)
        {
            if (itemVm is not PersonViewModel pvm) { return false; }
            if (!this.IsFiltering) { return true; }
            return (this.ShowAType && pvm.BloodType == BloodType.A) ||
                (this.ShowBType && pvm.BloodType == BloodType.B) ||
                (this.ShowABType && pvm.BloodType == BloodType.AB) ||
                (this.ShowOType && pvm.BloodType == BloodType.O); ;
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(nameof(PersonViewModel.BloodType), direction);
        }

        public override GroupDescription GroupOverride()
        {
            return new BloodTypeGroupDescription();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.ShowAType = false;
            this.ShowBType = false;
            this.ShowABType = false;
            this.ShowOType = false;
            this.IsGrouping = false;
        }

        public class BloodTypeGroupDescription : GroupDescription
        {
            public BloodTypeGroupDescription()
            {
                this.CustomSort = new BloodTypeComparer();
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                if (item is not PersonViewModel pvm) { throw new ArgumentException(nameof(item)); }
                var title = pvm.BloodType switch
                {
                    BloodType.A => "A型",
                    BloodType.B => "B型",
                    BloodType.AB => "AB型",
                    BloodType.O => "O型",
                };
                return new GroupHeaderViewModel(pvm.BloodType, title);
            }

            private class BloodTypeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((BloodType)((GroupHeaderViewModel)gx.Name).Value).CompareTo((BloodType)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}
