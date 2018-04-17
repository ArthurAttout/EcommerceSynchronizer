using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using EcommerceSynchronizer.Synchronizers;
using EcommerceSynchronizer.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authorization;

namespace EcommerceSynchronizer.Controllers
{
    [Produces("application/json")]
    public class SynchronizerStatusController : Controller
    {
        private readonly ApplicationState _state;
        private readonly Synchronizer _synchronizer;

        public SynchronizerStatusController(ApplicationState state, Synchronizer synchronizer)
        {
            this._state = state;
            this._synchronizer = synchronizer;
        }

        // GET api/SynchronizerStatus
        [Route("api/synchronizer")]
        [HttpGet]
        public string GetSynchronizerStatus()
        {
            return _state.IsSynchronizerRunning.ToString();
        }

        [Route("api/synchronizer/start")]
        [HttpPost]
        public string PostStart()
        {
            var jobid = Guid.NewGuid().ToString();
            RecurringJob.AddOrUpdate(jobid,() => _synchronizer.UpdateFromTimeout(),Cron.Minutely);

            _state.SynchronizerJobID = jobid;
            _state.IsSynchronizerRunning = true;

            return "started";
        }


        [Route("api/synchronizer/stop")]
        [HttpPost]
        public string PostStop()
        {
            RecurringJob.RemoveIfExists(_state.SynchronizerJobID);
            _state.IsSynchronizerRunning = false;

            return "stopped";
        }

        [Route("api/synchronizer/forceUpdate")]
        [HttpPost]
        public string PostForceUpdate()
        {

            _synchronizer.UpdateFromTimeout();
            return "updated";
        }

        [Route("api/synchronizer/sale")]
        [HttpPost]
        public string PostSaleTrigger()
        {

            _synchronizer.UpdateFromTimeout();
            return "updated";
        }
    }
}