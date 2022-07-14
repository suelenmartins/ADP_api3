using System;
using System.Net.Http;

namespace ADP_api.Models
{
    public class Instruction
    {
        public string Id { get; set; }
        public string Operation { get; set; }
        public float Left { get; set; }
        public float Right { get; set; }
        public float Result { get; set; }

    }

}
