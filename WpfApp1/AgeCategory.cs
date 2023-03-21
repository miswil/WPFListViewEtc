namespace WpfApp1
{
    internal enum AgeCategory
    {
        UnderTen,
        TeenAgers,
        Twenties,
        Thirties,
        Fourties,
        Fifties,
        Sixties,
        OverSeventies,
    }

    internal static class AgeCategoryExtensions
    {
        public static AgeCategory CategorizeAge(this int age)
        {
            return age switch
            {
                < 10 => AgeCategory.UnderTen,
                >= 10 and < 20 => AgeCategory.TeenAgers,
                >= 20 and < 30 => AgeCategory.Twenties,
                >= 30 and < 40 => AgeCategory.Thirties,
                >= 40 and < 50 => AgeCategory.Fourties,
                >= 50 and < 60 => AgeCategory.Fifties,
                >= 60 and < 70 => AgeCategory.Sixties,
                _ => AgeCategory.OverSeventies,
            };
        }
    }
}
