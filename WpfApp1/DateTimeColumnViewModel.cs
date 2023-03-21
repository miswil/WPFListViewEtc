using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1
{
    internal class DateTimeColumnViewModel : ColumnViewModelBase, IDisposable
    {
        private string propertyName;
        private INotifyCollectionChanged? items;
        private ICollection<DateTime> filterRange;

        public override string? DisplayMenber => this.propertyName;

        private bool range;
        public bool Range
        {
            get => this.range;
            set
            {
                if (this.SetProperty(ref this.range, value) && value)
                {
                    this.Today = false;
                    this.Yesterday = false;
                    this.ThisWeek = false;
                    this.LastWeek = false;
                    this.ThisMonth = false;
                    this.LastMonth = false;
                    this.MorePast = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool today;
        public bool Today
        {
            get => this.today;
            set
            {
                if (this.SetProperty(ref this.today, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool yesterday;
        public bool Yesterday
        {
            get => this.yesterday;
            set
            {
                if (this.SetProperty(ref this.yesterday, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool thisWeek;
        public bool ThisWeek
        {
            get => this.thisWeek;
            set
            {
                if (this.SetProperty(ref this.thisWeek, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool lastWeek;
        public bool LastWeek
        {
            get => this.lastWeek;
            set
            {
                if (this.SetProperty(ref this.lastWeek, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool thisMonth;
        public bool ThisMonth
        {
            get => this.thisMonth;
            set
            {
                if (this.SetProperty(ref this.thisMonth, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool lastMonth;
        public bool LastMonth
        {
            get => this.lastMonth;
            set
            {
                if (this.SetProperty(ref this.lastMonth, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool morePast;
        public bool MorePast
        {
            get => this.morePast;
            set
            {
                if (this.SetProperty(ref this.morePast, value) && value)
                {
                    this.Range = false;
                }
                this.OnPropertyChanged(nameof(IsFiltering));
            }
        }
        private bool todayExist;
        public bool TodayExist
        {
            get => this.todayExist;
            set => this.SetProperty(ref this.todayExist, value);
        }
        private bool yesterdayExist;
        public bool YesterdayExist
        {
            get => this.yesterdayExist;
            set => this.SetProperty(ref this.yesterdayExist, value);
        }
        private bool thisWeekExist;
        public bool ThisWeekExist
        {
            get => this.thisWeekExist;
            set => this.SetProperty(ref this.thisWeekExist, value);
        }
        private bool lastWeekExist;
        public bool LastWeekExist
        {
            get => this.lastWeekExist;
            set => this.SetProperty(ref this.lastWeekExist, value);
        }
        private bool thisMonthExist;
        public bool ThisMonthExist
        {
            get => this.thisMonthExist;
            set => this.SetProperty(ref this.thisMonthExist, value);
        }
        private bool lastMonthExist;
        public bool LastMonthExist
        {
            get => this.lastMonthExist;
            set => this.SetProperty(ref this.lastMonthExist, value);
        }
        private bool morePastExist;
        public bool MorePastExist
        {
            get => this.morePastExist;
            set => this.SetProperty(ref this.morePastExist, value);
        }


        public override bool IsFiltering =>
            this.Range ||
            this.Today || this.Yesterday ||
            this.ThisWeek || this.LastWeek ||
            this.ThisMonth || this.LastMonth ||
            this.MorePast;

        public ICommand SelectFilterRangeComand { get; }

        public DateTimeColumnViewModel(string propertyName, IEnumerable items)
        {
            this.propertyName = propertyName;
            this.filterRange = new List<DateTime> { DateTime.Today };
            this.SelectFilterRangeComand = new RelayCommand<SelectedDatesCollection>(this.SelectFilterRangeComandExecute);

            ShowFilters(items);
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

            var value = itemVm.GetType().GetProperty(this.propertyName)?.GetValue(itemVm);
            if (value is not DateTime dt) { throw new NotImplementedException(); }
            var category = dt.CategorizeDateTime();
            return 
                (this.Range && this.filterRange!.Min().Date <= dt &&  dt < this.filterRange.Max().Date.AddDays(1)) ||
                (!this.Range && (
                    (this.Today && category == DateTimeCategory.Today) ||
                    (this.Yesterday && category == DateTimeCategory.Yesterday) ||
                    (this.ThisWeek && category == DateTimeCategory.ThisWeek) ||
                    (this.LastWeek && category == DateTimeCategory.LastWeek) ||
                    (this.ThisMonth && category == DateTimeCategory.ThisMonth) ||
                    (this.LastMonth && category == DateTimeCategory.LastMonth) ||
                    (this.MorePast && category == DateTimeCategory.MorePast)
                    ));
        }

        private void SelectFilterRangeComandExecute(SelectedDatesCollection? filterRange)
        {
            if (filterRange is null) { return; }
            this.filterRange = filterRange;
            this.Range = true;
            this.FilterCommand.Execute(null);
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(this.propertyName, direction);
        }

        public override GroupDescription GroupOverride()
        {
            return new DateTimeGroupDescription(this.propertyName);
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.Range = false;
            this.Today = false;
            this.Yesterday = false;
            this.ThisWeek = false;
            this.LastWeek = false;
            this.ThisMonth = false;
            this.LastMonth = false;
            this.MorePast = false;
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
                    ShowFilters((IEnumerable)sender);
                    break;
            }
        }

        void ShowFilters(IEnumerable items)
        {
            foreach (var item in items.Cast<object>())
            {
                var value = item.GetType().GetProperty(this.propertyName)?.GetValue(item);
                if (value is not DateTime dt) { throw new NotImplementedException(); }
                var category = dt.CategorizeDateTime();
                switch (dt.CategorizeDateTime())
                {
                    case DateTimeCategory.Today:
                        this.TodayExist = true;
                        break;
                    case DateTimeCategory.Yesterday:
                        this.YesterdayExist = true;
                        break;
                    case DateTimeCategory.ThisWeek:
                        this.ThisWeekExist = true;
                        break;
                    case DateTimeCategory.LastWeek:
                        this.lastWeekExist = true;
                        break;
                    case DateTimeCategory.ThisMonth:
                        this.ThisMonthExist = true;
                        break;
                    case DateTimeCategory.LastMonth:
                        this.LastMonthExist = true;
                        break;
                    case DateTimeCategory.MorePast:
                        this.MorePastExist = true;
                        break;
                }
            }
        }

        public class DateTimeGroupDescription : GroupDescription
        {
            private readonly string propertyName;

            public DateTimeGroupDescription(string propertyName)
            {
                this.CustomSort = new DateTimeComparer();
                this.propertyName = propertyName;
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                var value = item.GetType().GetProperty(this.propertyName)?.GetValue(item);
                if (value is not DateTime dt) { throw new NotImplementedException(); }
                var category = dt.CategorizeDateTime();
                var title = category switch
                {
                    DateTimeCategory.Today => "今日",
                    DateTimeCategory.Yesterday => "昨日",
                    DateTimeCategory.ThisWeek => "今週（今日・昨日を除く）",
                    DateTimeCategory.LastWeek => "先週",
                    DateTimeCategory.ThisMonth => "今月（今週・先週を除く）",
                    DateTimeCategory.LastMonth => "先月",
                    DateTimeCategory.MorePast => "かなり前",
                    _ => throw new NotImplementedException(),
                };
                return new GroupHeaderViewModel(category, title);
            }

            private class DateTimeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((DateTimeCategory)((GroupHeaderViewModel)gx.Name).Value).CompareTo((DateTimeCategory)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}
