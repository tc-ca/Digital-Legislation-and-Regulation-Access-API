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
using System.Xml;
using Newtonsoft.Json;
using System.Threading;

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

                int i = 1;
                int successes = 0;
                int failures = 0;
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
                            currentToDate = reg.Element("CurrentToDate").Value
                        });
                    string fullDetailsJSON = getJsonFromXmlOnWeb(reg.Element("LinkToXML").Value);
                    
                    if (fullDetailsJSON != "")
                    {
                        context.RegDetails.Add(new RegDetails
                        {
                            id = reg.Attribute("id").Value,
                            fullDetails = fullDetailsJSON
                        });
                    }

                    if (i % 25 == 0)
                    {
                        try
                        {
                            await context.SaveChangesAsync();
                            successes++;
                            Console.WriteLine("A batch of Regs was saved successfully. There have been " + successes + " successes and " + failures + " failures.");
                        }
                        catch (DbUpdateException)
                        {
                            failures++;
                            Console.WriteLine("There was an exception thrown when saving changes to a batch of Regs");
                        }
                    }
                    i++;
                }

                try
                {
                    await context.SaveChangesAsync();
                    successes++;
                    Console.WriteLine("Regs were saved successfully. There have been " + successes + " successes and " + failures + " failures.");
                }
                catch (DbUpdateException)
                {
                    failures++;
                    Console.WriteLine("There was an exception thrown when saving changes to the last batch of Regs. There have been " + successes + " successes and " + failures + " failures.");
                }

                i = 1;
                successes = 0;
                failures = 0;
                foreach (XElement act in acts.Elements("Act"))
                {
                    if (context.Act.Find(act.Element("UniqueId").Value,act.Element("Language").Value) != null)
                        continue;

                    string fullDetailsJSON = getJsonFromXmlOnWeb(act.Element("LinkToXML").Value);

                    if(fullDetailsJSON != "")
                    {
                        context.ActDetails.Add(new ActDetails
                        {
                            uniqueId = act.Element("UniqueId").Value,
                            lang = act.Element("Language").Value,
                            fullDetails = fullDetailsJSON
                        });
                    }

                    Act newAct = new Act
                    {
                        uniqueId = act.Element("UniqueId").Value,
                        officialNum = act.Element("OfficialNumber").Value,
                        lang = act.Element("Language").Value,
                        title = act.Element("Title").Value,
                        currentToDate = act.Element("CurrentToDate").Value,
                        regs = new List<ActReg>()
                    };

                    try
                    {
                        foreach (XElement childReg in act.Element("RegsMadeUnderAct").Descendants("Reg"))
                        {
                            var reg = context.Reg.Find(childReg.Attribute("idRef").Value);
                            if (reg != null)
                            {
                                newAct.regs.Add(new ActReg
                                {
                                    reg = reg
                                });
                            }

                        }
                    }
                    catch { }

                    context.Act.Add(newAct);

                    //if (i % 15 == 0)
                    if(0==0)
                    {
                        try
                        {
                            await context.SaveChangesAsync();
                            successes++;
                            Console.WriteLine("A batch of Acts was saved successfully. There have been " + successes + " successes and " + failures + " failures.");
                        }
                        catch (DbUpdateException)
                        {
                            failures++;
                            Console.WriteLine("There was an exception thrown when saving changes to a batch of Acts. There have been " + successes + " successes and " + failures + " failures.");
                        }
                    }
                    i++;
                }

                try
                {
                    await context.SaveChangesAsync();
                    successes++;
                    Console.WriteLine("Acts were saved successfully. There have been " + successes + " successes and " + failures + " failures.");
                }
                catch (DbUpdateException)
                {
                    failures++;
                    Console.WriteLine("There was an exception thrown when saving changes to the last batch of Acts. There have been " + successes + " successes and " + failures + " failures.");
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

        private static string getJsonFromXmlOnWeb(string url)
        {
            try
            {
                var fullDetailsXML = XElement.Parse(httpGet(url));

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fullDetailsXML.ToString());
                return JsonConvert.SerializeXmlNode(doc).Replace(@"\","");
            }
            catch
            {
                return "";
            }
        }
    }

}