//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class WordUrlAssociation
    {
        public int AssociationId { get; set; }
    
        public virtual Url Url { get; set; }
        public virtual Word Word { get; set; }
    }
}
