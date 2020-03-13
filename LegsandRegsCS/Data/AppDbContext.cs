using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LegsandRegsCS.Models;

namespace LegsandRegsCS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<LegsandRegsCS.Models.Reg> Reg { get; set; }
        public DbSet<LegsandRegsCS.Models.Act> Act { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            //Setting the primary key of the EF table for this object to have a compound primary key made from the uniqueId and lang fields.
            modelbuilder.Entity<Act>().HasKey(c => new { c.uniqueId, c.lang });
            modelbuilder.Entity<ActReg>().HasKey(c => new { c.actUniqueId, c.actLang, c.regId });

            modelbuilder.Entity<ActReg>()
                .HasOne(ActReg => ActReg.act)
                .WithMany(Act => Act.actRegs)
                .HasForeignKey(ActReg => new {ActReg.actUniqueId, ActReg.actLang});
            modelbuilder.Entity<ActReg>()
                .HasOne(ActReg => ActReg.reg)
                .WithMany(Reg => Reg.actRegs)
                .HasForeignKey(ActReg => ActReg.regId);
        }
    }
}
