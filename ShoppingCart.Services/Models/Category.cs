namespace ShoppingCart.Service.Models
{
    public class Category
    {
        public string Title { get; }
        public Category ParentCategory { get; set; }

        public Category(string title)
        {
            Title = title;
        }

        public Category(string title, Category parentCategory) : this(title)
        {
            ParentCategory = parentCategory;
        }
    }
}