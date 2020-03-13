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
                            details = "placeholder"
                        });
                    /*try
                    {
                        await context.SaveChangesAsync();
                        Console.WriteLine("Saved the reg " + reg.Attribute("id").Value);
                    }
                    catch (DbUpdateException)
                    {
                        Console.WriteLine("Exception while saving reg " + reg.Attribute("id").Value);
                    }*/
                }

                try
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine("Regs were saved successfully");
                }
                catch (DbUpdateException)
                {
                    Console.WriteLine("There was an exception thrown when saving changes to Regs");
                    success = false;
                }

                if (success || !success)
                {
                    int i = 0;
                    foreach (XElement act in acts.Elements("Act"))
                    {
                        string fullDetailsJSON = "";

                        /*try
                        {
                            var fullDetailsXML = XElement.Parse(httpGet(act.Element("LinkToXML").Value));

                            XmlDocument doc = new XmlDocument();
                            doc.LoadXml(fullDetailsXML.ToString());
                            fullDetailsJSON = JsonConvert.SerializeXmlNode(doc);
                        }
                        catch { }*/
                        Act newAct = new Act
                        {
                            uniqueId = act.Element("UniqueId").Value,
                            officialNum = act.Element("OfficialNumber").Value,
                            lang = act.Element("Language").Value,
                            title = act.Element("Title").Value,
                            currentToDate = act.Element("CurrentToDate").Value,
                            details = fullDetailsJSON,
                            actRegs = new List<ActReg>()
                        };

                        try
                        {
                            foreach (XElement childReg in act.Element("RegsMadeUnderAct").Descendants("Reg"))
                            {
                                var reg = context.Reg.Find(childReg.Attribute("idRef").Value);
                                if (reg != null)
                                {
                                    newAct.actRegs.Add(new ActReg
                                    {
                                        act = newAct,
                                        reg = reg
                                    });
                                }

                            }
                        }
                        catch { }

                        context.Act.Add(newAct);

                        if (i % 50 == 0)
                        {
                            try
                            {
                                await context.SaveChangesAsync();
                                Console.WriteLine("A batch of Acts was saved successfully");
                            }
                            catch (DbUpdateException)
                            {
                                Console.WriteLine("There was an exception thrown when saving changes to a batch of Acts");
                            }
                        }
                        i++;
                    }
                }

                try
                {
                    await context.SaveChangesAsync();
                    Console.WriteLine("Acts were saved successfully");
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

}