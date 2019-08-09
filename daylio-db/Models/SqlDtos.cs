using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace rgkaizen.daylio.Models
{
    public class DaylioMonthlyActivityDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string name { get; set; }
        public int count { get; set; }
    }
}