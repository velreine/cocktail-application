namespace CocktailConsoleApplication
{
    public abstract class AbstractIngredient : AbstractEntity , IIngredient
    {
        public string Name { get; set; }
        
    }
}