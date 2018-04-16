using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using EcommerceSynchronizer.Utilities;
using Hangfire;

namespace EcommerceSynchronizer.Controllers
{
    [Produces("application/json")]
    public class SynchronizerStatusController : Controller
    {
        private ApplicationState state;

        public SynchronizerStatusController(ApplicationState state)
        {
            this.state = state;
        }

        // GET api/SynchronizerStatus
        [Route("api/synchronizer")]
        [HttpGet]
        public string GetSynchronizerStatus()
        {
            return state.MyProperty.ToString();
        }

        [Route("api/synchronizer/start")]
        [HttpPost]
        public string PostStart()
        {
            RecurringJob.AddOrUpdate(() => Test(),Cron.Minutely);
            state.IsSynchronizerRunning = true;
            return "started";
        }



        public void Test()
        {
            state.MyProperty += 10;
        }

        [Route("api/synchronizer/stop")]
        [HttpPost]
        public string PostStop()
        {
            state.IsSynchronizerRunning = false;
            return "stopped";
        }
    }
}