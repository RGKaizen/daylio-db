using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rgkaizen.daylio.Models
{
    [Table("raws")]
    public class DaylioRawModel
    {
        [Key]
        public int key { get; set; }
        public int year { get; set; }
        public string date { get; set; }
        public string weekday { get; set; }
        public string time { get; set; }
        public string mood { get; set; }
        public string activities { get; set; }
        public string note { get; set; }
    }

    public class DaylioCsvModel
    {
        public int year { get; set; }
        public string date { get; set; }
        public string weekday { get; set; }
        public string time { get; set; }
        public string mood { get; set; }
        public string activities { get; set; }
        public string note { get; set; }
    }

    [Table("entries")]
    public class DaylioEntryModel
    {
        [Key]
        public int Id { get; set; }
        public DateTime date { get; set; }
        public string mood { get; set; }
        public string note { get; set; }
        public string Guid { get; set; }
    }

    [Table("activities")]

    public class DaylioActivityModel
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public bool isPrivate { get; set; }
        public string Guid { get; set; }
    }

    [Table("activity_entry_refs")]
    public class DaylioActivityEntryRefModel
    {
        [Key]
        public int Id { get; set; }
        public int entry_id { get; set; }
        public int activity_id { get; set; }
    }

    public class DaylioPayload
    {
        public List<DaylioEntryModel> Entries { get; set; }

        public List<DaylioActivityModel> Activites { get; set; }


    }
}
