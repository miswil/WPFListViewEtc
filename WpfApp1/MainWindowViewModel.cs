using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace WpfApp1
{
    internal class MainWindowViewModel : ObservableObject
    {
        private ColumnViewModelBase currentSortColumn;
        private ColumnViewModelBase? currentGroupColumn;

        public ObservableCollection<PersonViewModel> Persons { get; } = new();
        public ObservableCollection<ColumnViewModelBase> Columns { get; } = new();

        public ICommand ShowNameCommand { get; }
        public ICommand ShowFuriganaCommand { get; }
        public ICommand ShowAgeCommand { get; }
        public ICommand ShowLastLoginCommand { get; }
        public ICommand ShowBloodTypeCommand { get; }
        public ICommand ShowBirthplaceCommand { get; }

        public MainWindowViewModel()
        {
            this.InitData();

            this.ShowNameCommand = new RelayCommand<bool?>(this.ShowNameCommandExecute);
            this.ShowFuriganaCommand = new RelayCommand<bool?>(this.ShowFuriganaCommandExecute);
            this.ShowAgeCommand = new RelayCommand<bool?>(this.ShowAgeCommandExecute);
            this.ShowLastLoginCommand = new RelayCommand<bool?>(this.ShowLastLoginCommandExecute);
            this.ShowBloodTypeCommand = new RelayCommand<bool?>(this.ShowBloodTypeCommandExecute);
            this.ShowBirthplaceCommand = new RelayCommand<bool?>(this.ShowBirthplaceCommandExecute);

            this.ShowNameCommandExecute(true);
            this.ShowFuriganaCommandExecute(true);
            this.ShowAgeCommandExecute(true);
            this.ShowLastLoginCommandExecute(true);
            this.ShowBloodTypeCommandExecute(true);
            this.ShowBirthplaceCommandExecute(true);
            this.currentSortColumn = this.Columns[0];
            this.SortItems(this.Columns[0], ListSortDirection.Ascending);
        }

        private void ShowNameCommandExecute(bool? showName)
        {
            if (showName is bool b && b)
            {
                var vm = new StringColumnViewModel(nameof(PersonViewModel.Name), "名前");
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = this.Columns.First(c => c is StringColumnViewModel scvm && scvm.DisplayMenber == nameof(PersonViewModel.Name));
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                this.Columns.Remove(vm);
            }
        }

        private void ShowFuriganaCommandExecute(bool? showFurigana)
        {
            if (showFurigana is bool b && b)
            {
                var vm = new StringColumnViewModel(nameof(PersonViewModel.Furigana), "ふりがな");
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = this.Columns.First(c => c is StringColumnViewModel scvm && scvm.DisplayMenber == nameof(PersonViewModel.Furigana));
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                this.Columns.Remove(vm);
            }
        }

        private void ShowAgeCommandExecute(bool? showAge)
        {
            if (showAge is bool b && b)
            {
                var vm = new AgeColumnViewModel(this.Persons);
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                vm.GroupingRequested += this.GroupItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = (AgeColumnViewModel)this.Columns.First(c => c is AgeColumnViewModel);
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                vm.GroupingRequested -= this.GroupItems;
                this.Columns.Remove(vm);
                vm.Dispose();
            }
        }

        private void ShowLastLoginCommandExecute(bool? showLastLogin)
        {
            if (showLastLogin is bool b && b)
            {
                var vm = new DateTimeColumnViewModel(nameof(PersonViewModel.LastLogin), this.Persons);
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                vm.GroupingRequested += this.GroupItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = (DateTimeColumnViewModel)this.Columns.First(c => c is DateTimeColumnViewModel dcvm && dcvm.DisplayMenber == nameof(PersonViewModel.LastLogin));
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                vm.GroupingRequested -= this.GroupItems;
                this.Columns.Remove(vm);
                vm.Dispose();
            }
        }

        private void ShowBloodTypeCommandExecute(bool? showBloodType)
        {
            if (showBloodType is bool b && b)
            {
                var vm = new BloodTypeColumnViewModel();
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                vm.GroupingRequested += this.GroupItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = this.Columns.First(c => c is BloodTypeColumnViewModel);
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                vm.GroupingRequested -= this.GroupItems;
                this.Columns.Remove(vm);
            }
        }

        private void ShowBirthplaceCommandExecute(bool? showBirthplace)
        {
            if (showBirthplace is bool b && b)
            {
                var vm = new UnlimitedSelectionColumnViewModel(nameof(PersonViewModel.Birthplace), this.Persons);
                vm.FilterRequested += this.FilterItems;
                vm.SortRequested += this.SortItems;
                vm.GroupingRequested += this.GroupItems;
                this.Columns.Add(vm);
            }
            else
            {
                var vm = (UnlimitedSelectionColumnViewModel)this.Columns.First(c => c is UnlimitedSelectionColumnViewModel);
                vm.FilterRequested -= this.FilterItems;
                vm.SortRequested -= this.SortItems;
                vm.GroupingRequested -= this.GroupItems;
                this.Columns.Remove(vm);
                vm.Dispose();
            }
        }

        private void FilterItems(object? sender, EventArgs e)
        {
            var view = CollectionViewSource.GetDefaultView(this.Persons);
            view.Filter = item =>
            {
                if (item is not PersonViewModel vm) { return false; }
                var allFilterPassed = true;
                foreach (var column in this.Columns)
                {
                    var isPassed = column.Filter(vm);
                    allFilterPassed = allFilterPassed && isPassed;
                }
                return allFilterPassed;
            };
        }

        private void SortItems(object? sender, ListSortDirection? requestedDirection)
        {
            if (sender is not ColumnViewModelBase column) { return; }
            var view = CollectionViewSource.GetDefaultView(this.Persons);
            view.SortDescriptions.Clear();
            ListSortDirection nextDirection;
            if (column == this.currentSortColumn && requestedDirection is null)
            {
                nextDirection = column.SortDirection switch
                {
                    ListSortDirection.Ascending => ListSortDirection.Descending,
                    ListSortDirection.Descending => ListSortDirection.Ascending,
                    _ => throw new ArgumentException(nameof(requestedDirection))
                };
            }
            else 
            {
                nextDirection = requestedDirection ?? ListSortDirection.Ascending;
            }
            this.currentSortColumn.ResetSort();
            view.SortDescriptions.Add(column.Sort(nextDirection));
            this.currentSortColumn = column;
        }

        private void GroupItems(object? sender, EventArgs e)
        {
            if (sender is not ColumnViewModelBase column) { return; }
            var view = CollectionViewSource.GetDefaultView(this.Persons);
            view.GroupDescriptions.Clear();
            if (column.IsGrouping)
             {
                if (this.currentGroupColumn is not null)
                {
                    this.currentGroupColumn.ResetGroup();
                }
                view.GroupDescriptions.Add(column.Group());
                this.currentGroupColumn = column;
            }
            else
            {
                this.currentGroupColumn = null;
            }
        }

        private void InitData()
        {
            var today = DateTime.Today;
            var r = new Random(); this.Persons.Add(new PersonViewModel(new Person("山本 美津子", "やまもと みつこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "福岡県")));
            this.Persons.Add(new PersonViewModel(new Person("渡部 敬子", "わたなべ けいこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "神奈川県")));
            this.Persons.Add(new PersonViewModel(new Person("宮田 修平", "みやた しゅうへい", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "福岡県")));
            this.Persons.Add(new PersonViewModel(new Person("近藤 大輝", "こんどう たいき", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("岡村 泰司", "おかむら やすし", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.AB, "大阪府")));
            this.Persons.Add(new PersonViewModel(new Person("山本 伸", "やまもと しん", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "千葉県")));
            this.Persons.Add(new PersonViewModel(new Person("山 和雄", "やま かずお", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("金子 健一", "かねこ けんいち", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "茨城県")));
            this.Persons.Add(new PersonViewModel(new Person("持田 みずき", "もちだ みずき", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.AB, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("鶴 あゆ", "つる あゆ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "埼玉県")));
            this.Persons.Add(new PersonViewModel(new Person("久保 秀敏", "くぼ ひでとし", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("亀田 康夫", "かめだ やすお", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("谷口 伸子", "たにぐち のぶこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("藤野 健太郎", "ふじの けんたろう", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "埼玉県")));
            this.Persons.Add(new PersonViewModel(new Person("橋本 康史", "はしもと やすし", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("一ノ瀬 政貴", "いちのせ まさたか", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.AB, "大阪府")));
            this.Persons.Add(new PersonViewModel(new Person("本多 翔子", "ほんだ しょうこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "三重県")));
            this.Persons.Add(new PersonViewModel(new Person("鎌田 佳祐", "かまた けいすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "千葉県")));
            this.Persons.Add(new PersonViewModel(new Person("岡田 龍之介", "おかだ りゅうのすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "北海道")));
            this.Persons.Add(new PersonViewModel(new Person("中嶋 大輔", "なかじま だいすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("山口 幸治", "やまぐち こうじ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("山本 信太郎", "やまもと しんたろう", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("中村 敏和", "なかむら としかず", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "北海道")));
            this.Persons.Add(new PersonViewModel(new Person("真鍋 唯", "まなべ ゆい", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("福島 信介", "ふくしま しんすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("中村 圭史", "なかむら よしふみ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "宮城県")));
            this.Persons.Add(new PersonViewModel(new Person("山口 亮佑", "やまぐち りょうすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "北海道")));
            this.Persons.Add(new PersonViewModel(new Person("藤山 みゆき", "ふじやま みゆき", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("笹野 誠", "ささの まこと", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("川俣 智", "かわまた とも", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "大阪府")));
            this.Persons.Add(new PersonViewModel(new Person("中塚 陽一", "なかつか よういち", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.AB, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("小幡 卓史", "おばた たかし", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "埼玉県")));
            this.Persons.Add(new PersonViewModel(new Person("松本 ケンジ", "まつもと けんじ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("新谷 幸一郎", "しんたに こういちろう", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("小野寺 健一", "おのでら けんいち", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "熊本県")));
            this.Persons.Add(new PersonViewModel(new Person("山本 良二", "やまもと りょうじ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "北海道")));
            this.Persons.Add(new PersonViewModel(new Person("川村 誠志", "かわむら せいじ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "大阪府")));
            this.Persons.Add(new PersonViewModel(new Person("大原 香那", "おおはら かな", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "千葉県")));
            this.Persons.Add(new PersonViewModel(new Person("尾形 望", "おがた のぞみ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("川添 淳子", "かわぞえ じゅんこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("園田 邦彦", "そのだ くにひこ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.AB, "宮城県")));
            this.Persons.Add(new PersonViewModel(new Person("金成 ひとみ", "かなり ひとみ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "北海道")));
            this.Persons.Add(new PersonViewModel(new Person("渡邊 龍", "わたなべ りゅう", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "愛知県")));
            this.Persons.Add(new PersonViewModel(new Person("小島 紗矢", "こじま さや", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "神奈川県")));
            this.Persons.Add(new PersonViewModel(new Person("内藤 修", "ないとう おさむ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("坂本 雪絵", "さかもと ゆきえ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "神奈川県")));
            this.Persons.Add(new PersonViewModel(new Person("小坂 大輔", "こさか だいすけ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.A, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("山本 亮平", "やまもと りょうへい", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "大阪府")));
            this.Persons.Add(new PersonViewModel(new Person("古庄 穣", "ふるしょう ゆたか", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.B, "東京都")));
            this.Persons.Add(new PersonViewModel(new Person("山本 人美", "やまもと ひとみ", r.Next(0, 80), today.AddDays(-r.Next(0, 90)), BloodType.O, "宮崎県")));

        }
    }
}
