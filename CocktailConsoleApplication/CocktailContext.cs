using Microsoft.EntityFrameworkCore;

namespace CocktailConsoleApplication
{
    public class CocktailContext : DbContext
    {
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Liquid> Liquids { get; set; }
        public DbSet<Accessory> Accessories { get; set; }
        public DbSet<AbstractIngredient> Ingredients { get; set; }


        public CocktailContext() : base()
        {
        }

        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=localhost;database=cocktaildb;user=sa;password=!supercomplex12345");
            //optionsBuilder.UseMySQL("server=localhost;database=cocktaildb;user=sa;password=12345");

            //base.OnConfiguring(optionsBuilder);
        }
    }
}