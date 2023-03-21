using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace WpfApp1
{
    internal class PersonViewModel : ObservableObject
    {
        private readonly Person person;

        public PersonViewModel(Person person)
        {
            this.person = person;
            this.Name = new StringViewModel(person.Name);
            this.Furigana = new StringViewModel(person.Furigana);
        }

        public StringViewModel Name { get; }

        public StringViewModel Furigana { get; }

        public int Age => this.person.Age;

        public DateTime LastLogin => this.person.LastLogin;

        public BloodType BloodType => this.person.BloodType;

        public string Birthplace => this.person.Birthplace;
    }
}
