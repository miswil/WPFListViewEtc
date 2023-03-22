using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace WpfApp1
{
    internal sealed class AgeColumnViewModel : ColumnViewModelBase, IDisposable
    {
        public override object CellTemplateResourceKey => "AgeCell";
        private ObservableCollection<PersonViewModel> persons;

        private bool underTen;
        public bool UnderTen
        {
            get => this.underTen;
            set
            {
                this.SetProperty(ref this.underTen, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool teenAgers;
        public bool TeenAgers
        {
            get => this.teenAgers;
            set
            {
                this.SetProperty(ref this.teenAgers, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool twenties;
        public bool Twenties
        {
            get => this.twenties;
            set
            {
                this.SetProperty(ref this.twenties, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool thirties;
        public bool Thirties
        {
            get => this.thirties;
            set
            {
                this.SetProperty(ref this.thirties, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool fourties;
        public bool Fourties
        {
            get => this.fourties;
            set
            {
                this.SetProperty(ref this.fourties, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool filties;
        public bool Fifties
        {
            get => this.filties;
            set
            {
                this.SetProperty(ref this.filties, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool sixties;
        public bool Sixties
        {
            get => this.sixties;
            set
            {
                this.SetProperty(ref this.sixties, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool overSeventies;
        public bool OverSeventies
        {
            get => this.overSeventies;
            set
            {
                this.SetProperty(ref this.overSeventies, value);
                this.OnPropertyChanged(nameof(this.IsFiltering));
            }
        }
        private bool underTenExist;
        public bool UnderTenExist
        {
            get => this.underTenExist;
            set => this.SetProperty(ref this.underTenExist, value);
        }
        private bool teenAgersExist;
        public bool TeenAgersExist
        {
            get => this.teenAgersExist;
            set => this.SetProperty(ref this.teenAgersExist, value);
        }
        private bool twentiesExist;
        public bool TwentiesExist
        {
            get => this.twentiesExist;
            set => this.SetProperty(ref this.twentiesExist, value);
        }
        private bool thirtiesExist;
        public bool ThirtiesExist
        {
            get => this.thirtiesExist;
            set => this.SetProperty(ref this.thirtiesExist, value);
        }
        private bool fourtiesExist;
        public bool FourtiesExist
        {
            get => this.fourtiesExist;
            set => this.SetProperty(ref this.fourtiesExist, value);
        }
        private bool filtiesExist;
        public bool FiftiesExist
        {
            get => this.filtiesExist;
            set => this.SetProperty(ref this.filtiesExist, value);
        }
        private bool sixtiesExist;
        public bool SixtiesExist
        {
            get => this.sixtiesExist;
            set => this.SetProperty(ref this.sixtiesExist, value);
        }
        private bool overSeventiesExist;
        public bool OverSeventiesExist
        {
            get => this.overSeventiesExist;
            set => this.SetProperty(ref this.overSeventiesExist, value);
        }

        public override bool IsFiltering =>
            this.UnderTen || this.TeenAgers ||
            this.Twenties || this.Thirties ||
            this.Fourties || this.Fifties ||
            this.Sixties || this.OverSeventies;

        public AgeColumnViewModel(ObservableCollection<PersonViewModel> persons)
        {
            this.ShowFilters(persons);
            persons.CollectionChanged += this.Persons_CollectionChanged;
            this.persons = persons;
        }

        public void Dispose()
        {
            this.persons.CollectionChanged -= this.Persons_CollectionChanged;
        }

        protected override SortDescription SortOverride(ListSortDirection direction)
        {
            return new SortDescription(nameof(PersonViewModel.Age), direction);
        }

        protected override bool FilterOverride(object itemVm)
        {
            if (itemVm is not PersonViewModel pvm) { return false; }
            if (!this.IsFiltering) { return true; }
            var category = pvm.Age.CategorizeAge();
            return
                (this.UnderTen && category == AgeCategory.UnderTen) ||
                (this.TeenAgers && category == AgeCategory.TeenAgers) ||
                (this.Twenties && category == AgeCategory.Twenties) ||
                (this.Thirties && category == AgeCategory.Thirties) ||
                (this.Fourties && category == AgeCategory.Fourties) ||
                (this.Fifties && category == AgeCategory.Fifties) ||
                (this.Sixties && category == AgeCategory.Sixties) ||
                (this.overSeventies && category == AgeCategory.OverSeventies);
        }

        public override GroupDescription GroupOverride()
        {
            return new AgeGroupDescription();
        }

        protected override void ResetFilterAndGroupCommandExecuteOverride()
        {
            this.UnderTen = false;
            this.TeenAgers = false;
            this.Twenties = false;
            this.Thirties = false;
            this.Fourties = false;
            this.Fifties = false;
            this.Sixties = false;
            this.OverSeventies = false;
            this.IsGrouping = false;
        }

        private void Persons_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    this.ShowFilters((ObservableCollection<PersonViewModel>)sender);
                    break;
            }
        }

        void ShowFilters(ObservableCollection<PersonViewModel> persons)
        {
            foreach (var person in persons)
            {
                switch (person.Age.CategorizeAge())
                {
                    case AgeCategory.UnderTen:
                        this.UnderTenExist = true;
                        break;
                    case AgeCategory.TeenAgers:
                        this.TeenAgersExist = true;
                        break;
                    case AgeCategory.Twenties:
                        this.TwentiesExist = true;
                        break;
                    case AgeCategory.Thirties:
                        this.ThirtiesExist = true;
                        break;
                    case AgeCategory.Fourties:
                        this.FourtiesExist = true;
                        break;
                    case AgeCategory.Fifties:
                        this.FiftiesExist = true;
                        break;
                    case AgeCategory.Sixties:
                        this.SixtiesExist = true;
                        break;
                    case AgeCategory.OverSeventies:
                        this.OverSeventiesExist = true;
                        break;
                }
            }
        }

        public class AgeGroupDescription : GroupDescription
        {
            public AgeGroupDescription()
            {
                this.CustomSort = new AgeComparer();
            }

            public override object GroupNameFromItem(object item, int level, CultureInfo culture)
            {
                if (item is not PersonViewModel pvm) { throw new ArgumentException(nameof(item)); }
                var category = pvm.Age.CategorizeAge();
                var title = category switch
                {
                    AgeCategory.UnderTen => "10才未満",
                    AgeCategory.TeenAgers => "10歳台",
                    AgeCategory.Twenties => "20歳台",
                    AgeCategory.Thirties => "30歳台",
                    AgeCategory.Fourties => "40歳台",
                    AgeCategory.Fifties => "50歳台",
                    AgeCategory.Sixties => "60歳台",
                    AgeCategory.OverSeventies => "70才以上",
                };
                return new GroupHeaderViewModel(category, title);
            }

            private class AgeComparer : IComparer
            {
                public int Compare(object? x, object? y)
                {
                    return (x, y) switch
                    {
                        (CollectionViewGroup gx, CollectionViewGroup gy) =>
                            ((AgeCategory)((GroupHeaderViewModel)gx.Name).Value).CompareTo((AgeCategory)((GroupHeaderViewModel)gy.Name).Value),
                        _ => throw new ArgumentException(),
                    };
                }
            }
        }
    }
}
