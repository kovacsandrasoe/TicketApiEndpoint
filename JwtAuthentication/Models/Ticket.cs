using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JwtAuthentication.Models
{
    public class Ticket
    {
        [Key]
        public string Uid { get; set; }

        [StringLength(100)]
        public string EventName { get; set; }

        public DateTime EventDate { get; set; }

        public int Price { get; set; }

        [StringLength(100)]
        public string ContentType { get; set; }

        public byte[] PictureData { get; set; }

        public IdentityUser Seller { get; set; }
    }
}
