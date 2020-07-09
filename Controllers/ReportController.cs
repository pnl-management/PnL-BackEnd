using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PnLReporter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
        private static readonly string AppName = "PnLReporter";
        private static readonly string SpreadsheetId = "1CreuKDEYZEWqGChVJO31S8PlhbtdqhMiJBHvUhEZJaY";
        private static readonly string SheetName = "Sheet1";
        private static SheetsService service;

        [HttpGet("/gg-sheet")]
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
            var range = $"{SheetName}!A7:C7";
            var valueRange = new ValueRange();

            var objLst = new List<object>() { "Quý 7 / 2020", "9383", "829" };
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
        }
    }
}