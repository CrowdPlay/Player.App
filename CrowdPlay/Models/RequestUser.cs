using System;

namespace CrowdPlay.Models
{
    public class RequestUser
    {
        public string Mood { get; set; }
        public int Room { get; set; }
        public string TwitterHandle { get; set; }
    }
}