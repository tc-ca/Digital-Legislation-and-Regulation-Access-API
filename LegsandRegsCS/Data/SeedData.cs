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
        public static AppDbContext context;
        private static bool dbBuildInProgress = false;

        public static async void Update(bool force = false)
        {
            if (force)
            {
                Program.telemetry.TrackEvent("FORCE_UPDATE_STARTED");
                Start(true);
            }
            else {
                try
                {
                    DateTime minDate = await context.Act.MinAsync(a => a.currentToDate);

                    //Checking to see if any regulations are out of date
                    if (DateTime.Compare(DateTime.Now, minDate) >= 0)
                    {
                        Program.telemetry.TrackTrace("The DB is out of date. It will be updated.");
                        Program.telemetry.TrackEvent("UPDATE_STARTED");
                        Start();
                    }
                }
                catch
                {
                    Program.telemetry.TrackTrace("The DB is empty. It will be populated.");
                    Program.telemetry.TrackEvent("DB_INITIALIZATION");
                    Start();
                }
            }
        }

        private static async void Start(bool force = false)
        {
            try
            {
                await ExtractAndLoad(force);
                if(dbBuildInProgress == false)
                {
                    Program.downForMaintenance = false;//If some of the data does not make it into the DB, the API stays down for maintenance
                    Program.telemetry.TrackTrace("App taken out of maintenance mode");
                }
                    
            }
            catch
            {
                Program.telemetry.TrackTrace("An unexpected error occured during the update.");
                Program.telemetry.TrackEvent("UPDATE_FAILED");
                if (dbBuildInProgress == false)
                {
                    Program.downForMaintenance = false;
                    Program.telemetry.TrackTrace("App taken out of maintenance mode");
                }
                else
                    Program.telemetry.TrackEvent("CRITICAL_FAILURE");
            }
        }

        private static async Task<bool> ExtractAndLoad(bool force)
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
                    if (tries > 3)
                        await Task.Delay(300000);
                    if (tries > 5)
                    {
                        Program.telemetry.TrackTrace("Retrieval of the primary data source failed. DB update cancelled.");
                        Program.telemetry.TrackEvent("UPDATE_FAILED");
                        return false;
                    }
                        
                }
            }

            XElement actsAndRegs = XElement.Parse(xml);

            var rawActs = actsAndRegs.Element("Acts");
            var rawRegs = actsAndRegs.Element("Regulations");

            var regs = new List<Reg>();
            var acts = new List<Act>();
            var regDetails = new List<RegDetails>();
            var actDetails = new List<ActDetails>();

            if (force == false)
            {
                try//Check to see if the new Acts are actually more up to date than the current ones. Justice Canada does not always post the updated data on time, so there is no point reloading data that is the same as it was before
                {
                    DateTime minDateCurrent = await context.Act.MinAsync(a => a.currentToDate);
                    DateTime maxDateCurrent = await context.Act.MaxAsync(a => a.currentToDate);
                    DateTime minDateNew = rawActs.Elements("Act").ToList().Min(a => DateTime.Parse(a.Element("CurrentToDate").Value));
                    DateTime maxDateNew = rawActs.Elements("Act").ToList().Min(a => DateTime.Parse(a.Element("CurrentToDate").Value));

                    if (DateTime.Compare(minDateCurrent, minDateNew) == 0 || DateTime.Compare(maxDateCurrent, maxDateNew) == 0)
                    {
                        Program.telemetry.TrackTrace("Although some or all data is out of date, it is no more out of date than the primary data source. Update aborted.");
                        return false;
                    }

                }
                catch { }
            }

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
                        {
                            Program.telemetry.TrackTrace("There have been 500 failed attempts to retrieve regulations. DB update cancelled.");
                            Program.telemetry.TrackEvent("UPDATE_FAILED");
                            return false;
                        }
                        
                        await Task.Delay(1000);
                        if (tries > 3)
                        {
                            newDetails = context.RegDetails.Find(reg.Attribute("id").Value);//Find will return null if there is no match
                            Program.telemetry.TrackTrace("A Reg Details had to be retrieved from the old DB due to too many failed retrievals.");
                        }
                    }
                }
                if (newDetails != null)
                    regDetails.Add(newDetails);
            }

            foreach (XElement act in rawActs.Elements("Act"))
            {
                Act newAct =
                    new Act
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
                        var reg = regs.Find(r => r.id == childReg.Attribute("idRef").Value);
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

                acts.Add(newAct);

                string fullDetailsJSON = "";
                ActDetails newDetails = null;

                for (int tries = 0; fullDetailsJSON.Equals(""); tries++)
                {
                    try
                    {
                        fullDetailsJSON = getJsonFromXmlOnWeb(act.Element("LinkToXML").Value);
                        newDetails = new ActDetails
                        {
                            uniqueId = act.Element("UniqueId").Value,
                            lang = act.Element("Language").Value,
                            fullDetails = fullDetailsJSON
                        };
                    }
                    catch
                    {
                        totalFailures++;
                        if (totalFailures > 500)//If the service seems to be broken, give up for the day and try again at the next scheduled attempt
                        {
                            Program.telemetry.TrackTrace("There have been 500 failed attempts to retrieve regulations. DB update cancelled.");
                            Program.telemetry.TrackEvent("UPDATE_FAILED");
                            return false;
                        }
                        await Task.Delay(1000);
                        if (tries > 3)
                        {
                            newDetails = context.ActDetails.Find(act.Element("UniqueId").Value, act.Element("Language").Value);
                            Program.telemetry.TrackTrace("An Act Details had to be retrieved from the old DB due to too many failed retrievals.");
                        }
                    }
                }
                if (newDetails != null)
                    actDetails.Add(newDetails);
            }
            Program.telemetry.TrackTrace("Retrieval of the new data succeeded. Proceeding with DB reset and rebuild.");
            Program.telemetry.TrackEvent("RESET_STARTED");

            await ResetDatabase();
            
            int loops = 0;
            foreach (Reg reg in regs)
            {
                context.Reg.Add(reg);
                if (loops % 100 == 0)
                {
                    await SaveChanges();
                }
                loops++;
            }
            await SaveChanges();

            Program.telemetry.TrackTrace("Regs saved");

            loops = 0;
            foreach (Act act in acts)
            {
                context.Act.Add(act);
                if (loops % 100 == 0)
                {
                    await SaveChanges();
                }
                loops++;
            }
            await SaveChanges();

            Program.telemetry.TrackTrace("Acts saved");

            loops = 0;
            foreach (ActDetails actDetail in actDetails)
            {
                context.ActDetails.Add(actDetail);
                if (loops % 25 == 0)
                {
                    await SaveChanges();
                }
                loops++;
            }
            await SaveChanges();

            Program.telemetry.TrackTrace("Act Details saved");

            loops = 0;
            foreach (RegDetails regDetail in regDetails)
            {
                context.RegDetails.Add(regDetail);
                if (loops % 25 == 0)
                {
                    await SaveChanges();
                }
                loops++;
            }
            await SaveChanges();

            Program.telemetry.TrackTrace("Reg Details saved");

            if (await SaveChanges())
            {
                Program.telemetry.TrackTrace("The update was successful.");
                Program.telemetry.TrackEvent("UPDATE_SUCCESSFUL");
                dbBuildInProgress = false;
                return true;
            }
            else
            {
                Program.telemetry.TrackTrace("Some or all of the data could not be uploaded to the DB. The DB is incomplete and must be rebuilt.");
                Program.telemetry.TrackEvent("UPDATE_FAILED");
                Program.telemetry.TrackEvent("CRITICAL_FAILURE");
                return false;
            }


        }

        private static async Task<bool> ResetDatabase()
        {
            dbBuildInProgress = true;
            Program.downForMaintenance = true;
            context.Database.EnsureDeleted();
            Program.telemetry.TrackTrace("The DB has been deleted");
            context.Database.EnsureCreated();
            Program.telemetry.TrackTrace("The DB structure has been rebuilt");
            await SeedLanguages();
            Program.telemetry.TrackTrace("The languages have been seeded");
            return true;
        }

        public static async Task<bool> SeedLanguages()
        {
            context.Language.Add(new Language
            {
                langCode = "eng",
                englishLabel = "English",
                frenchLabel = "Anglais"
            });
            context.Language.Add(new Language
            {
                langCode = "fra",
                englishLabel = "French",
                frenchLabel = "Français"
            });
            await SaveChanges();
            return true;
        }


        private static async Task<bool> SaveChanges(int depth = 0)
        {
            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException)
            {
                if(depth < 5)
                {
                    return SaveChanges(depth + 1).Result;
                }
                return false;
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
                return JsonConvert.SerializeXmlNode(doc).Replace(@"\", "").Replace("\"@", "\"").Replace("\"#", "\"");
            }
            catch
            {
                throw new Exception("XML Retrieval Failed");
            }
        }
    }

}