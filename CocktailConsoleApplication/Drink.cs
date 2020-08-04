using System.Collections.Generic;
using System.Text;

namespace CocktailConsoleApplication
{
    public class Drink : AbstractEntity, INameable
    {
        public string Name { get; set; }
        
        public ICollection<DrinkIngredient> Ingredients { get; set; } = new List<DrinkIngredient>();
        
        public Drink()
        {
            //this.Ingredients = new List<DrinkIngredient>();
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append($"Drink: \"{this.Name}\" Ingredients: ");

            foreach (var ingredient in this.Ingredients)
            {
                sb.Append(ingredient.ToString() + ",");
            }
            
            
            return sb.ToString();
        }
    }
}