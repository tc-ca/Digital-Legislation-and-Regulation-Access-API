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
        public static async void Update()
        {

            context = new AppDbContext(Program.services.GetRequiredService<DbContextOptions<AppDbContext>>());
            DateTime minDate = await context.Act.MinAsync(a => a.currentToDate);

            //Checking to see if any regulations are out of date
            if(DateTime.Compare(DateTime.Now, minDate) >= 0)
            {
                Initialize();
            }
            
        }

        private static async void Initialize()
        {
            string xml = "";
            int totalFailures = 0;

            for (int tries = 0; xml.Equals(""); tries++)
            {
                try
                {
                    xml = await httpGet("https://laws-lois.justice.gc.ca/eng/XML/Legis.xml");
                }
                catch
                {
                    totalFailures++;
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
                        totalFailures++;
                        if (totalFailures > 500)//If the service seems to be broken, give up for the day and try again at the next scheduled attempt
                            return;
                        await Task.Delay(1000);
                        if (tries > 3)
                        {
                            newDetails = context.RegDetails.Find(reg.Attribute("id").Value);//Find will return null if there is no match
                        }
                    }
                }
                if (newDetails != null)
                    regDetails.Add(newDetails);
            }

            context.Reg.RemoveRange(context.Reg);
            context.Act.RemoveRange(context.Act);
            context.RegDetails.RemoveRange(context.RegDetails);
            context.ActDetails.RemoveRange(context.ActDetails);

            await context.SaveChangesAsync();

            int loops = 0;
            foreach (Reg reg in regs)
            {
                context.Reg.Add(reg);
                if (loops % 100 == 0)
                {
                    await SaveChanges();
                }
            }

            loops = 0;
            foreach (Act act in acts)
            {
                context.Act.Add(act);
                if (loops % 100 == 0)
                {
                    await SaveChanges();
                }

            }

            loops = 0;
            foreach (ActDetails actDetail in actDetails)
            {
                context.ActDetails.Add(actDetail);
                if (loops % 25 == 0)
                {
                    await SaveChanges();
                }

            }

            loops = 0;
            foreach (ActDetails actDetail in actDetails)
            {
                context.ActDetails.Add(actDetail);
                if (loops % 25 == 0)
                {
                    await SaveChanges();
                }
            }


        }

        private static async Task<bool> SaveChanges()
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                try
                {
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException)
                {
                    return false;
                }
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