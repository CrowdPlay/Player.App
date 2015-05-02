using System;

namespace CrowdPlay.Models
{
    public class User
    {
        public string mood { get; set; }
        public int room { get; set; }
        public Guid id { get; set; }
    }
}