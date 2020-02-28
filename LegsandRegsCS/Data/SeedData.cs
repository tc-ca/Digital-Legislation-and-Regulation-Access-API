using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using LegsandRegsCS.Models;
using System.Net;
using System.IO;
using System.Xml.Linq;

namespace LegsandRegsCS.Data
{
    public class SeedData
    {
        public static async void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
            {
                string xml = httpGet("https://laws-lois.justice.gc.ca/eng/XML/Legis.xml");

                XElement actsAndRegs = XElement.Parse(xml);

                var acts = actsAndRegs.Element("Acts");
                var regs = actsAndRegs.Element("Regulations");
                bool success = true;

                foreach (XElement reg in regs.Elements("Regulation"))
                {
                    context.Reg.Add(
                        new Reg
                        {
                            id = reg.Attribute("id").Value,
                            otherLangId = reg.Attribute("olid").Value,
                            uniqueId = reg.Element("UniqueId").Value,
                            title = reg.Element("Title").Value,
                            lang = reg.Element("Language").Value,
                            currentToDate = reg.Element("CurrentToDate").Value,
                            //arentActs = new List<string>()
                        });
                }

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("There was an exception thrown when saving changes to Regs");
                    success = false;
                }

                if (success)
                {
                    foreach (XElement act in acts.Elements("Act"))
                    {
                        List<string> childregs = new List<string>();
                        try
                        {
                            foreach (XElement childReg in act.Element("RegsMadeUnderAct").Descendants("Reg"))
                            {
                                childregs.Add(childReg.Attribute("idRef").Value);
                                Console.WriteLine(childReg.Attribute("idRef").Value);
                            }
                        }
                        catch { }

                        context.Act.Add(
                            new Act
                            {
                                uniqueId = act.Element("UniqueId").Value,
                                officialNum = act.Element("OfficialNumber").Value,
                                lang = act.Element("Language").Value,
                                title = act.Element("Title").Value,
                                currentToDate = act.Element("CurrentToDate").Value,
                                //regsUnderAct = childregs
                            });
                    }
                }

                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("There was an exception thrown when saving changes to Acts");
                }


            }
        }

        private static string httpGet(string url)
        {
            string output = "The request failed.";

            try 
            {
                System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    output = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return output;
        }
    }
    /*var reg =
                                from results in regs.Elements()
                                where results.Attribute("id").Value == childReg.Attribute("idRef").Value
                                select results;
                            foreach (XElement result in reg)
                            {
                                context.Reg.Add(
                                    new Reg
                                    {
                                        id = result.Attribute("id").Value,
                                        otherLangId = result.Attribute("olid").Value,
                                        uniqueId = result.Element("UniqueId").Value,
                                        title = result.Element("Title").Value,
                                        lang = result.Element("Language").Value,
                                        currentToDate = result.Element("CurrentToDate").Value,
                                        parentActId = act.Element("UniqueId").Value
                                    });
                            }
*/
}