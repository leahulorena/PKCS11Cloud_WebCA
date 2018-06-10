using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebCA_LeahuLorena.Models
{
    [Table("Models")]
    public class Model
    {
        [Key]
        public int id { get; set; }
        public string userID { get; set; }
        public string countryShort { get; set; }
        public string country { get; set; }
        public string locality { get; set; }
        public string organization { get; set; }
        public string departament { get; set; }
        public string domain { get; set; }

        [NotMapped]
        public string emailAddress { get; set; }
        public string privatePassword { get; set; }
    }
}