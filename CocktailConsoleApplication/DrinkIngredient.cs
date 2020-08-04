using System;

namespace CocktailConsoleApplication
{
    public class DrinkIngredient : AbstractEntity
    {
        //public int Id { get; private set; }
        
        public AbstractIngredient Ingredient { get; set; }
        
        public MeasurementUnit MeasurementUnit { get; set; }

        public float Amount { get; set; }

        public override string ToString()
        {
            return
                this.MeasurementUnit == MeasurementUnit.None ?
                    $"{this.Ingredient.Name} "
                    :
                $"{this.Amount} {Enum.GetName(typeof(MeasurementUnit), this.MeasurementUnit)} {this.Ingredient.Name} ";
        }
    }
}