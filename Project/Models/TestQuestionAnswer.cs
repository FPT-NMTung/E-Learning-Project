//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Project.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TestQuestionAnswer
    {
        public int AnsID { get; set; }
        public string Answer { get; set; }
        public int QuesID { get; set; }
        public bool IsTrue { get; set; }
    
        public virtual TestQuestion TestQuestion { get; set; }
    }
}
