//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Facturador_CTECAM_3
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FACTURACTECAM_Entities : DbContext
    {
        public FACTURACTECAM_Entities()
            : base("name=FACTURACTECAM_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public DbSet<FACTURA> FACTURAS { get; set; }
        public DbSet<NCF> NCFs { get; set; }
        public DbSet<USER_REGISTER> USER_REGISTER { get; set; }
    }
}
