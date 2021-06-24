using System;
using System.Collections.Generic;

namespace WebTemplate.Models.Vessel.Vessel
{
    public class Index : BaseModel
    {
        public short ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal? GrossTon { get; set; }
        public decimal? NetTon { get; set; }
        public decimal? HP { get; set; }
        public List<ProcessLayer.Entities.Vessel> Vessels { get; set; } = new List<ProcessLayer.Entities.Vessel>();
    }
}