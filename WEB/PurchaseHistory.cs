//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WEB
{
    using System;
    using System.Collections.Generic;
    
    public partial class PurchaseHistory
    {
        public int ID { get; set; }
        public Nullable<int> UserID { get; set; }
        public string Code { get; set; }
        public string Bank { get; set; }
        public string BankCode { get; set; }
        public Nullable<decimal> Value { get; set; }
        public Nullable<bool> Status { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
    }
}
