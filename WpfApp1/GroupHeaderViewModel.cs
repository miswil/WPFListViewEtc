using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    internal class GroupHeaderViewModel : ObservableObject
    {
        public GroupHeaderViewModel(object value, string title)
        {
            this.Value = value;
            this.Title = title;
        }

        public object Value { get; }
        public string Title { get; }

        public override bool Equals(object? obj)
        {
            return obj is GroupHeaderViewModel ghvm &&
                this.Value.Equals(ghvm.Value);
        }
        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
