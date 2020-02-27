using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using LegsandRegsCS.Models;

namespace LegsandRegsCS.Data
{
    public class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                Console.WriteLine("reached flag 1");
                if (!context.Act.Any())
                {
                    Console.WriteLine("reached flag 2");
                    context.Act.AddRange(
                        new Act
                        {
                            uniqueId = "A-1",
                            officialNum = "A-1",
                            lang = "eng",
                            title = "Access to Information Act",
                            currentToDate = "2020-01-16"
                        },
                        new Act
                        {
                            uniqueId = "A-1",
                            officialNum = "A-1",
                            lang = "fra",
                            title = "Loi sur l’accès à l’information",
                            currentToDate = "2020-01-16"
                        }
                    );
                    Console.WriteLine("reached flag 3");
                }
                if (!context.Reg.Any())
                {
                    context.Reg.AddRange(
                        new Reg
                        {
                            id = "638933E",
                            otherLangId = "627530F",
                            uniqueId = "SI-83-108",
                            title = "Designating the Minister of Justice and the President of the Treasury Board as Ministers for Purposes of Certain Sections of the Act",
                            lang = "eng",
                            currentToDate = "2020-01-16",
                            parentActId = "A-1"
                        },
                        new Reg
                        {
                            id = "627530F",
                            otherLangId = "638933E",
                            uniqueId = "TR-83-108",
                            title = "Désignation du ministre de la Justice et du président du conseil du Trésor comme ministres chargés de l’application de certains articles de la Loi",
                            lang = "fra",
                            currentToDate = "2020-01-16",
                            parentActId = "A-1"
                        }
                    );
                }

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("There was an exception thrown when saving changes");
                }
            }
        }
    }
}
