using System;

namespace WpfApp1
{
    internal record Person(string Name, string Furigana, int Age, DateTime LastLogin, BloodType BloodType, string Birthplace) 
    {
    }
}
