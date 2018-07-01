using Newtonsoft.Json;
using RBS.Helpers;
using RBS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RBS.Models;

namespace RBS.Controllers
{
    public class ApiController : Controller
    {
        


        private async Task<bool> checkUploadPermissions(string accesstoken){
            //return true;
            return await NightscoutPermissions.CheckUploadPermissions(accesstoken);
        }


        private ActionResult Error(string msg){
            return Json(
                new
                {
                    Error = true,
                    Message = msg
                },
                JsonRequestBehavior.AllowGet);
        } 
        private ActionResult Success<T>(T result, string command){
            return Json(
                new
                {
                    Error = false,
                    Command = command,
                    Result = result
                },
                JsonRequestBehavior.AllowGet);
        } 
       
        public async Task<ActionResult> CreateRequestAsync(string accesstoken, string contents ){

            //var permissions= await NightscoutPermissions.CheckUploadPermissions(accesstoken);
            //var permissions = await NightscoutPermissions.CheckProcessPermissions(accesstoken);
            if (! await this.checkUploadPermissions(accesstoken))
            {
                return this.Error("CreateRequestAsync Denied");
            }
           


            if(string.IsNullOrWhiteSpace(contents)) {
                return this.Error("CreateRequestAsync Denied: invalid parameter contents");
            }

           

            var g = Guid.NewGuid().ToString();
            var reading = new RBS_Blob
            {
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                contents = contents,
                uuid = g,
                purpose = "RBS"


            };

            //return this.Error("synthax ok");
            try{
                await MongoConnection.AsyncInsertReading(reading);

            } catch(System.TimeoutException) {
                return Error("Timeout, database down?");
            }

            return Success<RBS_Blob>(reading, "CreateRequestAsync");

            //var content = $"accesstoken: {accesstoken}, b64contents: {b64contents}, guid: {g}";
            //return Content("CreateRequestAsync IS NOT IMPLEMENTED YET:" + content);    
        }




        public async Task<ActionResult> FetchContents(string accesstoken){
            
            if (!await this.checkUploadPermissions(accesstoken))
            {
                return this.Error("FetchContents Denied");
            }
            List<RBS_Blob> readings;
            try
            {
                readings = await MongoConnection.GetPendingReadingsForProcessing();
            }
            catch (Exception ex)
            {
                return Error("FetchContents Failed: " + ex.Message);
            }

            var result = "";
           
            foreach (var item in readings)
            {
                result += $"{item.ModifiedOn}|{item.contents}\r\n";
            }
            return Content(result, "text/plain");
            //var content = $"processing_accesstoken: {processing_accesstoken}";
            //return Content(System.Reflection.MethodBase.GetCurrentMethod().Name + " IS NOT IMPLEMENTED YET:" + content);
        }

       
        public ActionResult Index()
        {
            var content = @"
{ available_methods: ""
/api/CreateRequestAsync -> Object{uuid:String|null, error:false|true, msg:String}
 params:   -contents
  -accesstoken

""} 

";
            return Content(content, "application/json");
            //the minutes parameter is often 1440 (24 hours), telling you how long back you should do the search
            //In nightscout context it is mostly redundant as the maxCount will search as long back as needed.
            //we ignore that parameter
            //Logger.LogInfo("Accesing Glucose Index");


           
        }
    }
}
