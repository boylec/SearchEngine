﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SearchEngine.Data
{
    using System;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class SearchEngineEntities : DbContext
    {
        private DbConnection connection;

        public SearchEngineEntities()
            : base("name=SearchEngineEntities")
        {
        }

        public SearchEngineEntities(DbConnection connection)
        {
            this.connection = connection;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Url> Urls { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<WordUrlAssociation> WordUrlAssociations { get; set; }
    }
}