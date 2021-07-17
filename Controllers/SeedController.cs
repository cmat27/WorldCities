using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OfficeOpenXml;
using WorldCities.Data;
using WorldCities.Data.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WorldCities.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SeedController(ApplicationDbContext context, IWebHostEnvironment env) {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import() {
            // this will prevent any non-development env to acces/run this task
            if (!_env.IsDevelopment()) {
                throw new SecurityException("not allowed");
            }

            var path = Path.Combine(_env.ContentRootPath, "Data/Source/worldcities.xlsx"  );

            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);
            //access the first worksheet
           
            var worksheet = excelPackage.Workbook.Worksheets.First();
            //define the amount of rows to process
            var nEndRow = worksheet.Dimension.End.Row;
            //initialize the record counter
            var numberOfCountriesAdded = 0;
            var numberOfCitiesAdded = 0;
            //create a look up directory
            // containing all the countries allready existing
            //into the DB it will be empty on the firstrun
            var countriesByName = _context.Countries
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            //iterate through all the rows skipping the first one
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];
                var countryName = row[nRow, 5].GetValue<string>();
                var isao2 = row[nRow, 6].GetValue<string>();
                var isao3 = row[nRow, 7].GetValue<string>();
                // condition to skeep if country already in DB
                if (countriesByName.ContainsKey(countryName)) {
                    continue;
                }
                //create teh country entity and fill with xlxs data
                var country = new Country
                {
                    Name = countryName,
                    ISO2 = isao2,
                    ISO3 = isao3
                };
                // add new country to the db context
                await _context.Countries.AddAsync(country);
                // store country in look up to retrieve its id later
                countriesByName.Add(countryName, country);
                numberOfCountriesAdded++;

            }

            if (numberOfCountriesAdded > 0) {
                await _context.SaveChangesAsync();
            }
            //create a look up dictionary with all the cities already existing
            // into the db
            var cities = _context.Cities
               .AsNoTracking()
               .ToDictionary(x => (
                    Name: x.Name,
                    Lat: x.Lat,
                    Lon: x.Lon,
                    CountrId: x.CountryId
                ));
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[nRow, 1, nRow, worksheet.Dimension.End.Column];

                var name = row[nRow, 1].GetValue<string>();
                var nameAscii = row[nRow, 2].GetValue<string>();
                var lat = row[nRow, 3].GetValue<decimal>();
                var lon = row[nRow, 4].GetValue<decimal>();
                var countryName = row[nRow, 5].GetValue<string>();

                // retrieve country name by country id
                var countryId = countriesByName[countryName].Id;
                //skipp city if already exist in db
                if (cities.ContainsKey((
                     Name: name,
                     Lat: lat,
                     Lon: lon,
                     CountrId: countryId
                     ))) 
                    continue;
                //create City entity and fill with datea from xlxs file
                var city = new City
                {
                    Name = name,
                    Name_ASCII = nameAscii,
                    Lat = lat,
                    Lon = lon,
                    CountryId = countryId
                };
                // add city to the db
                _context.Cities.Add(city);
                numberOfCitiesAdded++;
            }
            if (numberOfCitiesAdded > 0) {
                await _context.SaveChangesAsync();
            }

            return new JsonResult(new
            {
                Citie = numberOfCitiesAdded,
                Countries= numberOfCountriesAdded

            });


        }
    }
}
