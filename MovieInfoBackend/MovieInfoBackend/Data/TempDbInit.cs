using MovieInfoBackend.Models;
using System;
using System.Linq;

namespace MovieInfoBackend.Data
{
    public static class TempDbInit
    {
        public static void Initialize(MovieInfoContext context)
        {
            context.Database.EnsureCreated();

            // Look for any test models.
            if (context.TestModels.Any())
            {
                return;   // DB has been seeded
            }

            var testModels = new TestModel[]
            {
            new TestModel{FirstMidName="Carson",LastName="Alexander",EnrollmentDate=DateTime.Parse("2005-09-01")}
            };
            foreach (TestModel testModel in testModels)
            {
                context.TestModels.Add(testModel);
            }
            context.SaveChanges();
        }
    }
}