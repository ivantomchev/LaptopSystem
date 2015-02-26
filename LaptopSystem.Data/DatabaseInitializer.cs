namespace LaptopSystem.Data
{
    using LaptopSystem.Model;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class DatabaseInitializer : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public DatabaseInitializer()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            try
            {
                Random rnd = new Random();

                Manufacturer sampleManufacturer = new Manufacturer { Name = "Lenovo" };

                ApplicationUser user = new ApplicationUser { UserName = "pesho", Email = "pesho@abv.bg" };

                var laptops = new List<Laptop>();

                for (int i = 0; i < 10; i++)
                {
                    Laptop laptop = new Laptop();
                    laptop.HardDiskSize = rnd.Next(10, 1000);
                    laptop.ImageStringUrl = "http://laptop.bg/system/images/18267/thumb/IdeaPad_B590.png?1358425188.jpg";
                    laptop.Manufactorer = sampleManufacturer;
                    laptop.Model = "ideaPad";
                    laptop.MonitorSize = 15.4;
                    laptop.Price = rnd.Next(600, 3000);
                    laptop.RamMemorySize = rnd.Next(1, 16);
                    laptop.Weight = 3;

                    var votesCount = rnd.Next(0, 10);

                    for (int j = 0; j < votesCount; j++)
                    {
                        laptop.Votes.Add(new Vote { Laptop = laptop, VotedBy = user });
                    }

                    for (int j = 0; j < votesCount; j++)
                    {
                        laptop.Comments.Add(new Comment { Content = "Mnogo qk laptop brat.", Author = user });
                    }

                    laptops.Add(laptop);
                }
                laptops.ForEach(l => context.Laptops.AddOrUpdate(p => p.Model, l));

                context.SaveChanges();
            }
            catch (Exception)
            {
            }
            finally
            {
                base.Seed(context);
            }
        }


    }
}
