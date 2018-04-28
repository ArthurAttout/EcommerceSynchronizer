using System;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using EcommerceSynchronizer.Model;
using EcommerceSynchronizer.Synchronizers;
using EcommerceSynchronizer.Utilities;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

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
            if(_state.SynchronizerJobID != null)
                RecurringJob.RemoveIfExists(_state.SynchronizerJobID);

            _state.IsSynchronizerRunning = false;

            return "stopped";
        }

        [Route("api/synchronizer/forceUpdate")]
        [HttpPost]
        public string PostForceUpdate()
        {

            BackgroundJob.Enqueue(() => _synchronizer.UpdateFromTimeout());
            return "pending updated";
        }

        [Route("api/synchronizer/sale")]
        [HttpPost]
        public string PostSaleTrigger([FromBody] PostSaleBindingModel model)
        {
            if (model?.Delta == null || model.ItemEcommerceId == null || model.BalanceInCents <= 0)
            {
                Response.StatusCode = 400;
                return "invalid request. Should provide ItemEcommerceId and delta";
            }

            var sale = new Sale()
            {
                QuantitySold = model.Delta,
                Object = new Model.Object()
                {
                    EcommerceID = model.ItemEcommerceId
                },
                BalanceInCents = model.BalanceInCents
            };
            var jobid = BackgroundJob.Enqueue(() => _synchronizer.UpdateFromSale(sale));
            var response = new PostSaleResponseBindingModel()
            {
                Location = jobid,
                Status = "pending"
            };

            Response.StatusCode = 202;
            return JsonConvert.SerializeObject(response);
        }
    }

    public class PostSaleBindingModel
    {
        [JsonProperty("item_ecommerce_id")]
        public string ItemEcommerceId { get; set; }

        [JsonProperty("delta")]
        public int Delta { get; set; }

        [JsonProperty("balance")]
        public int BalanceInCents { get; set; }
    }

    public class PostSaleResponseBindingModel
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }
    }
}