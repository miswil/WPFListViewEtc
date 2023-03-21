using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    internal class StringViewModel : ObservableObject
    {
        private readonly string originalString;

        public StringViewModel(string originalString)
        {
            this.originalString = originalString;
            this.previousFilteredText = string.Empty;
            this.filteredText = string.Empty;
            this.followingFilteredText = originalString;
        }

        public string Value => this.originalString;

        private string previousFilteredText;
        public string PreviousFilteredText
        {
            get => this.previousFilteredText;
            set => this.SetProperty(ref this.previousFilteredText, value);
        }

        private string filteredText;
        public string FilteredText
        {
            get => this.filteredText;
            set => this.SetProperty(ref this.filteredText, value);
        }

        private string followingFilteredText;
        public string FollowingFilteredText
        {
            get => this.followingFilteredText;
            set => this.SetProperty(ref this.followingFilteredText, value);
        }

        public bool Filter(string filterText)
        {
            var index = this.originalString.IndexOf(filterText);
            if (index == -1)
            {
                this.PreviousFilteredText = this.originalString;
                this.FilteredText = string.Empty;
                this.FollowingFilteredText = string.Empty;
                return false;
            }
            else
            {
                this.PreviousFilteredText = this.originalString[..index];
                this.FilteredText = this.originalString.Substring(index, filterText.Length);
                this.FollowingFilteredText = this.originalString[(index + filterText.Length)..];
                return true;
            }
        }
    }
}
