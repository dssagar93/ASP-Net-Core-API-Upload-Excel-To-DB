using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ExcelDataReader;
using ExcelToDatabase.CustomExceptions;
using ExcelToDatabase.Models;
using ExcelToDatabase.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace ExcelToDatabase.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExcelOpsController : ControllerBase
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(ExcelOpsController));

        public ILogger _logger;
        private IBrandChannelService _service;

        public ExcelOpsController(ILogger<ExcelOpsController> logger,IBrandChannelService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost("/api/UploadExcelFile"), DisableRequestSizeLimit]
        public async Task<IActionResult> UploadExcel([FromForm(Name = "file")]IFormFile formFile)
        {

            try
            {
                _service.UploadExcel(formFile);
                return Ok(new { results = "File Uploaded Successfully To Database" });
            }
            catch (ValidationException ve)
            {
                return Ok(new { results = ve.Message.ToString() });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
         
        }


        [HttpPost("/api/BrandChannel")]
        public async Task<IActionResult> BrandChannelData([FromBody]string query)
        {

            try
            {
                DataTable dt = _service.GetBrandChannel(query);
                string jsonDT = JsonConvert.SerializeObject(dt, Formatting.Indented);

                return Ok(new { jsonData = jsonDT });

            }
            catch (ValidationException ve)
            {
                return Ok(new { results = ve.Message.ToString() });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }


    }
}