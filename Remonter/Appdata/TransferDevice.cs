//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Remonter.Appdata
{
    using System;
    using System.Collections.Generic;
    
    public partial class TransferDevice
    {
        public int id_transfer { get; set; }
        public int id_device { get; set; }
        public string branch_name_removed { get; set; }
        public string branch_name_moved { get; set; }
        public System.DateTime time_removed { get; set; }
    
        public virtual Device Device { get; set; }
    }
}
