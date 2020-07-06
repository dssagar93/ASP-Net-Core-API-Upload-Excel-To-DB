using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ExcelToDatabase.Services
{
    public interface IBrandChannelService
    {
        public void UploadExcel(IFormFile formFile);
        public DataTable GetBrandChannel(string query);
    }
}
