using rgkaizen.daylio.Models;
using System;
using System.Collections.Generic;

namespace rgkaizen.daylio
{
    public static class MappingExtensions
    {
        public static DaylioRawModel Map(this DaylioCsvModel model)
        {
            return new DaylioRawModel
            {
                mood = model.mood,
                full_date = model.full_date,
                date = model.date,
                time = model.time,
                weekday = model.weekday,
                activities = model.activities,
                note = model.note
            };
        }

        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }
}
