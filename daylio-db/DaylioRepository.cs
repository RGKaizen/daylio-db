using CsvHelper;
using Microsoft.EntityFrameworkCore;
using rgkaizen.daylio.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace rgkaizen.daylio
{
    public interface IDaylioRepository
    {
        Task populateRawData();

        Task convertRawData();

        Task truncateTables();

        List<DaylioActivityModel> getActivites();

        List<DaylioEntryModel> getEntries();

        List<DaylioMonthlyActivityDto> getActivityCounts();
    }

    public class DaylioRepository : IDaylioRepository
    {
        protected readonly DaylioDBContext _dbContext;

        public DaylioRepository(DaylioDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<DaylioActivityModel> getActivites()
        {
            return _dbContext.Activities.ToList();
        }

        public List<DaylioEntryModel> getEntries()
        {
            return _dbContext.Entries.ToList();
        }

        public List<DaylioMonthlyActivityDto> getActivityCounts()
        {
            var results = new List<DaylioMonthlyActivityDto>(); 
            var sql = @"select YEAR(e.date) as Year, MONTH(e.date) as Month, a.name, count(aer.id) as count from entries e 
                        join activity_entry_refs aer on aer.entry_id = e.id
                        join activities a on aer.activity_id = a.id
                        group by YEAR(e.date), MONTH(e.date), aer.activity_id";
            return RawSqlHack.ExecSQL<DaylioMonthlyActivityDto>(sql, _dbContext);
        }

        #region ParseCSV
        public async Task populateRawData()
        {
            using (var reader = File.OpenText("daylio_export.csv"))
            {
                var csv = new CsvReader(reader);
                csv.Configuration.MissingFieldFound = null;
                var daylioImport = csv.GetRecords<DaylioCsvModel>().Select(x => x.Map());
                _dbContext.Raws.AddRange(daylioImport);
                _dbContext.SaveChanges();

                Console.WriteLine($"Got {_dbContext.Raws.Count()} raws");

            }
        }

        public async Task convertRawData()
        {
            var entriesList = new List<DaylioEntryModel>();
            var activitiesList = new List<DaylioActivityModel>();
            var refList = new List<(string entryGuid, string activityGuid)>();

            foreach (var raw in _dbContext.Raws.ToList())
            {
                var entry = CreateNewEntry(raw);
                entriesList.Add(entry);
                if (!string.IsNullOrWhiteSpace(raw.activities))
                {
                    ParseActivities(raw, entry.Guid, activitiesList, refList);
                }
            }
            _dbContext.Entries.AddRange(entriesList);
            _dbContext.Activities.AddRange(activitiesList);
            _dbContext.SaveChanges();

            var refModelList = BuildRefModelList(entriesList, activitiesList, refList);

            _dbContext.ActivityEntryRefs.AddRange(refModelList);
            _dbContext.SaveChanges();

            Console.WriteLine($"Got {_dbContext.Entries.Count()} entries");
            Console.WriteLine($"Got {_dbContext.Activities.Count()} activites");
            Console.WriteLine($"Got {_dbContext.ActivityEntryRefs.Count()} entry activity refs");
        }

        public async Task truncateTables()
        {
            _dbContext.RemoveRange(_dbContext.Raws);
            _dbContext.RemoveRange(_dbContext.Entries);
            _dbContext.RemoveRange(_dbContext.Activities);
            _dbContext.RemoveRange(_dbContext.ActivityEntryRefs);
            _dbContext.SaveChanges();
        }

        private DaylioEntryModel CreateNewEntry(DaylioRawModel raw)
        {
            return new DaylioEntryModel()
            {
                Guid = Guid.NewGuid().ToString(),
                date = DateTime.Parse($"{raw.year}, {raw.date}, {raw.time}"),
                mood = raw.mood,
                note = raw.note,    
            };
        }

        private void ParseActivities(
            DaylioRawModel raw,
            string entryGuid,
            List<DaylioActivityModel> activitiesList,
            List<(string entryGuid, string activityGuid)> refList)
        {
            // Split activities and find existing records
            var activites = raw.activities.Contains("|")
                ? raw.activities.Split("|")
                : new string[] { raw.activities };
            
            activites.ForEach(activity =>
            {
                activity = activity.Trim();
                var activityRecord = activitiesList.Find(x => x.name.Equals(activity));
                if (activityRecord == null)
                {
                    var newActivityRecord = new DaylioActivityModel()
                    {
                        name = activity,
                        Guid = Guid.NewGuid().ToString(),
                        isPrivate = false
                    };
                    activitiesList.Add(newActivityRecord);
                    activityRecord = newActivityRecord;
                }

                refList.Add((entryGuid, activityRecord.Guid));
            });
        }

        private List<DaylioActivityEntryRefModel> BuildRefModelList(
            List<DaylioEntryModel> entries,
            List<DaylioActivityModel> activities,
            List<(string entryGuid, string activityGuid)> refList)
        {
            return refList.Select(reference =>
            {              
                var s = entries.Find(entry => entry.Guid.Equals(reference.entryGuid)).Id;
                var q = activities.Find(activity => activity.Guid.Equals(reference.activityGuid)).Id;
                return new DaylioActivityEntryRefModel
                {
                    entry_id = entries.Find(entry => entry.Guid.Equals(reference.entryGuid)).Id,
                    activity_id = activities.Find(activity => activity.Guid.Equals(reference.activityGuid)).Id
                };
            }).ToList();
        }

        #endregion
    }
}
