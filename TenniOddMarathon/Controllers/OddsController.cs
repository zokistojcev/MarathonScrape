using OddMarathon.Services.BusinessLogic.Odds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace TenniOddMarathon.Controllers
{
    public class OddsController : Controller
    {
        private IOddsService _oddService;
        public OddsController(IOddsService oddsService)
        {
            _oddService = oddsService;
        }
        // GET: Odds
        public async Task<ActionResult> Index()
        {
            var model = await _oddService.GetAll();
            return View();
        }
    }
}