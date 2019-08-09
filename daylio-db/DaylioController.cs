using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace rgkaizen.daylio
{
    [Route("[controller]/[action]")]
    public class DaylioController : Controller
    {
        private readonly IDaylioRepository _daylioRepository;

        public DaylioController(IDaylioRepository daylioRepository)
        {
            _daylioRepository = daylioRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> process()
        {
            await _daylioRepository.truncateTables();
            await _daylioRepository.populateRawData();
            await _daylioRepository.convertRawData();
            return Json("Ok");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> activites()
        {
            return Json(_daylioRepository.getActivites());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> entries()
        {
            return Json(_daylioRepository.getEntries());
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> activityCount()
        {
            return Json(_daylioRepository.getActivityCounts());
        }
    }
}
