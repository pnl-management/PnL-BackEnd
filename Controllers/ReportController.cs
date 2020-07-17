using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using PnLReporter.EnumInfo;
using PnLReporter.Models;
using PnLReporter.Service;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportController : ControllerBase
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string AppName = "PnLReporter";
        private static readonly string SpreadsheetId = "1CreuKDEYZEWqGChVJO31S8PlhbtdqhMiJBHvUhEZJaY";
        private static readonly string SheetName = "Sheet1";
        private static SheetsService service;

        private readonly PLSystemContext _context;
        private readonly IReportService _service;

        public ReportController(PLSystemContext context, IDistributedCache cache)
        {
            _context = context;
            _service = new ReportService(context, cache);
        }

        [HttpGet("/api/reports")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult GetReport()
        {
            var user = this.GetCurrentUserInfo();
            int storeSize;
            var listReport = _service.ListDataToGgSheet(user.Brand.Id, out storeSize);

            GoogleCredential credential;

            using (var stream = new FileStream("google-sheet-api-7f7e9f359cf6.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });
            }

            char endColumn = (char) ((storeSize + 65) > 90 ? 90 : (storeSize + 65));

            for (int i = 0; i < listReport.Count(); i++)
            {
                String rangeVal = "A" + (i+1) + ":" + endColumn + (i+1);

                var range = $"{SheetName}!{rangeVal}";
                var valueRange = new ValueRange();
                valueRange.Values = new List<IList<object>> { listReport[i] };

                var request = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
                request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                request.Execute();
            }

            return Ok();
        }

        [HttpGet("/api/reports/periods/{periodId}")]
        [Authorize(Roles = ParticipantsRoleConst.ACCOUNTANT + "," + ParticipantsRoleConst.INVESTOR)]
        public ActionResult GetAllTransactionOfPeriod(int periodId)
        {
            var user = this.GetCurrentUserInfo();
            var result = _service.GetReportOfBrand(user.Brand.Id, periodId);

            if (result == null) return BadRequest("Cannot get report of this brand: " + user.Brand.Id + " of account " + user.Username);

            return Ok(result);
        }

        [HttpGet("/api/reports/periods/{periodId}/stores")]
        [Authorize(Roles = ParticipantsRoleConst.STORE_MANAGER)]
        public ActionResult GetAllTransactionOfStore(int periodId)
        {
            var user = this.GetCurrentUserInfo();
            var result = _service.GetReportOfStore(user.Store.Id, periodId);

            if (result == null) return BadRequest("Cannot get report of this store: " + user.Brand.Id + " of account " + user.Username);

            return Ok(result);
        }

        /*[HttpGet("/gg-sheet")]
        public ActionResult GetList()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("google-sheet-api-7f7e9f359cf6.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });
            }

            return Ok(this.ReadEntries());
        }

        [HttpPost("/gg-sheet")]
        public ActionResult PostList()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("google-sheet-api-7f7e9f359cf6.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });
            }

            return Ok(this.CreateEntries());
        }

        [HttpPut("/gg-sheet")]
        public ActionResult PutList()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("google-sheet-api-7f7e9f359cf6.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });
            }

            return Ok(this.UpdateEntries());
        }

        [HttpDelete("/gg-sheet")]
        public ActionResult DeleteList()
        {
            GoogleCredential credential;

            using (var stream = new FileStream("google-sheet-api-7f7e9f359cf6.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
                service = new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = AppName,
                });
            }

            return Ok(this.ClearEntries());
        }

        private object ReadEntries()
        {
            var range = $"{SheetName}!A2:C5";
            var request = service.Spreadsheets.Values.Get(SpreadsheetId, range);

            var response = request.Execute();
            var values = response.Values;

            return values;
        }

        private object CreateEntries()
        {
            var range = $"{SheetName}!A:C";
            var valueRange = new ValueRange();

            var objectLst = new List<object>() { "Quý 6 / 2020", "20000000", "200000000" };
            valueRange.Values = new List<IList<object>> { objectLst };

            var request = service.Spreadsheets.Values.Append(valueRange, SpreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;

            return request.Execute();
        }

        private object UpdateEntries()
        {
            var range = $"{SheetName}!A8:C8";
            var valueRange = new ValueRange();

            var objLst = new List<object>() { "Quý 7sss / 2020", "9383", "829" };
            valueRange.Values = new List<IList<object>> { objLst };

            var request = service.Spreadsheets.Values.Update(valueRange, SpreadsheetId, range);
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;

            return request.Execute();
        }

        private object ClearEntries()
        {
            var range = $"{SheetName}!A9:C";
            var valueRange = new ClearValuesRequest();

            var request = service.Spreadsheets.Values.Clear(valueRange, SpreadsheetId, range);
            return request.Execute();
        }*/

        private UserModel GetCurrentUserInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            string role = identity.FindFirst(ClaimTypes.Role).Value;
            string participantIdVal = identity.FindFirst(ClaimTypes.NameIdentifier).Value;

            long participantsId;

            long.TryParse(participantIdVal, out participantsId);
            IParticipantService participantService = new ParticipantService(_context);

            return participantService.FindByUserId(participantsId);
        }
    }
}