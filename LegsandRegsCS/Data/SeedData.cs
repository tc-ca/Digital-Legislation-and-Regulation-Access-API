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
        private static AppDbContext context;
        public static async void update()
        {

            context = new AppDbContext(Program.services.GetRequiredService<DbContextOptions<AppDbContext>>());
            DateTime minDate = context.Act.Min(a => a.currentToDate);

            //Checking to see if any regulations are out of date
            if(DateTime.Compare(DateTime.Now, minDate) >= 0)
            {
                initialize();
            }
            
        }

        private static async void initialize()
        {
            string xml = "";


            for (int tries = 0; xml.Equals(""); tries++)
            {
                try
                {
                    xml = await httpGet("https://laws-lois.justice.gc.ca/eng/XML/Legis.xml");
                }
                catch
                {
                    await Task.Delay(1000);
                    if(tries > 3)
                        await Task.Delay(300000);
                    if (tries > 5)
                        return;
                }
            }

            XElement actsAndRegs = XElement.Parse(xml);

            var rawActs = actsAndRegs.Element("Acts");
            var rawRegs = actsAndRegs.Element("Regulations");

            var regs = new List<Reg>();
            var acts = new List<Act>();
            var regDetails = new List<RegDetails>();
            var actDetails = new List<ActDetails>();


            foreach (XElement reg in rawRegs.Elements("Regulation"))
            {
                regs.Add(
                    new Reg
                    {
                        id = reg.Attribute("id").Value,
                        otherLangId = reg.Attribute("olid").Value,
                        uniqueId = reg.Element("UniqueId").Value,
                        title = reg.Element("Title").Value,
                        lang = reg.Element("Language").Value,
                        currentToDate = DateTime.Parse(reg.Element("CurrentToDate").Value)
                    });

                string fullDetailsJSON = "";
                RegDetails newDetails = null;

                for (int tries = 0; fullDetailsJSON.Equals(""); tries++)
                {
                    try
                    {
                        fullDetailsJSON = getJsonFromXmlOnWeb(reg.Element("LinkToXML").Value);
                        newDetails = new RegDetails
                        {
                            id = reg.Attribute("id").Value,
                            fullDetails = fullDetailsJSON
                        };
                    }
                    catch
                    {
                        await Task.Delay(1000);
                        if (tries > 3)
                        {
                            newDetails = context.RegDetails.Find(reg.Attribute("id").Value);
                        }
                    }
                }
                if (newDetails != null)
                    regDetails.Add(newDetails);
            }

            int i = 1;
            int successes = 0;
            int failures = 0;
            foreach (XElement reg in rawRegs.Elements("Regulation"))
            {
                context.Reg.Add(
                    new Reg
                    {
                        id = reg.Attribute("id").Value,
                        otherLangId = reg.Attribute("olid").Value,
                        uniqueId = reg.Element("UniqueId").Value,
                        title = reg.Element("Title").Value,
                        lang = reg.Element("Language").Value,
                        currentToDate = DateTime.Parse(reg.Element("CurrentToDate").Value)
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
            foreach (XElement act in rawActs.Elements("Act"))
            {

                string fullDetailsJSON = getJsonFromXmlOnWeb(act.Element("LinkToXML").Value);

                if (fullDetailsJSON != "")
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
                    currentToDate = DateTime.Parse(act.Element("CurrentToDate").Value),
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
                if (0 == 0)
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

        private static async Task<string> httpGet(string url)
        {

            try 
            {
                System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw new Exception("HTTP request failed");
            }

        }

        private static string getJsonFromXmlOnWeb(string url)
        {
            try
            {
                var fullDetailsXML = XElement.Parse(httpGet(url).Result);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(fullDetailsXML.ToString());
                return JsonConvert.SerializeXmlNode(doc).Replace(@"\","");
            }
            catch
            {
                throw new Exception("XML Retrieval Failed");
            }
        }
    }

}