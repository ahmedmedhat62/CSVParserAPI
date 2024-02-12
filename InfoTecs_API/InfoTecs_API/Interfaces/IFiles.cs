using CsvHelper;
using InfoTecs_API.Data;
using InfoTecs_API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace InfoTecs_API.Interfaces
{
    public interface IFiles
    {
        Task<CSV_File> ReadAndSaveCSVAsync<T>(Stream file, string fileName);
        Task<List<value>> GetValuesByFileNameAsync(string fileName);
        Task<List<Result>> GetResultsAsync(string fileName, int? averageMin, int? averageMax, double? indicatorMin, double? indicatorMax,DateTime? from, DateTime? to) ;
    }
}


namespace InfoTecs_API.Interfaces
{
    public class CSVService : IFiles
    {
        private readonly DataContext _context;

        public CSVService(DataContext context)
        {
            _context = context;
        }

        public async Task<CSV_File> ReadAndSaveCSVAsync<T>(Stream file, string fileName)
        {
            try
            {
                bool isValidFile = true; // Flag to track file validity
                bool isValidRecords = true; // Flag to track record validity

                // Check if the file already exists in the database
                var existingFile = await _context.files.FirstOrDefaultAsync(f => f.file_name == fileName);

                if (existingFile == null)
                {
                    // File doesn't exist, create a new file entry
                    existingFile = new CSV_File
                    {
                        file_name = fileName,
                        file_id = Guid.NewGuid()
                    };
                    _context.files.Add(existingFile);
                }
                else
                {
                    // File exists, delete existing values and results associated with the file
                    var existingValues = await _context.values.Where(v => v.File_Id == existingFile.file_id).ToListAsync();
                    _context.values.RemoveRange(existingValues);

                    var existingResults = await _context.results.Where(r => r.file_id == existingFile.file_id).ToListAsync();
                    _context.results.RemoveRange(existingResults);
                }

                // Read and save the CSV file content along with its name to the database
                using (var reader = new StreamReader(file))

                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {

                    csv.Read();
                    csv.ReadHeader();
                    var records = csv.GetRecords<value>().ToList();
                        var recordCount = records.Count;

                        // Apply constraint on the number of records
                        if (recordCount < 1 || recordCount > 10000)
                        {
                            // Handle case where the number of records is out of range
                            isValidFile = false;
                        }
                        else
                        {
                            // Process and validate each record
                            foreach (var record in records)
                            {
                                // Parse the date and time string and set the DateAndTime property
                                record.DateAndTime1 = DateTime.ParseExact(record.DateAndTime, "yyyy-MM-dd_HH-mm-ss", CultureInfo.InvariantCulture)
                                                        .ToUniversalTime(); // Convert to UTC

                                // Set the file ID for the record
                                record.File_Id = existingFile.file_id;

                                // Validate the record after parsing the date and time
                                if (!ValidateRecord(record))
                                {
                                    // Log the validation failure
                                    Console.WriteLine($"Invalid record found in file '{fileName}': {record}");

                                    // Set flag to indicate invalid records
                                    isValidRecords = false;
                                }
                                else
                                {
                                    // Add the record to the database
                                    _context.values.Add(record);
                                }
                            }
                        }

                        if (isValidFile && isValidRecords)
                        {
                            // Calculate result
                            var result = CalculateResult(records, existingFile.file_id);

                            // Save changes to the database
                            await _context.SaveChangesAsync();

                            // Return the existing or newly created file entity
                            return existingFile;
                        }
                        else
                        {
                            // Handle case where file or records are not valid
                            return null;
                        }
                    }
                
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"An error occurred while processing the file '{fileName}': {ex.Message}");
                // You can throw the exception or return null here based on your requirement
                throw;
            }
        }

        private Result CalculateResult(List<value> values, Guid fileId)
        {
            // Perform calculation of the result based on the values
            int allTime = values.Max(v => v.IntegerTimeValue) - values.Min(v => v.IntegerTimeValue);
            DateTime minimumDate = values.Min(v => v.DateAndTime1);
            int averageTime = (int)values.Average(v => v.IntegerTimeValue);
            double averageIndicator = values.Average(v => v.FloatingPointIndicator);
            double medianIndicator = values.OrderBy(v => v.FloatingPointIndicator).Skip(values.Count / 2).First().FloatingPointIndicator;
            double maxIndicator = values.Max(v => v.FloatingPointIndicator);
            double minIndicator = values.Min(v => v.FloatingPointIndicator);
            int rowNumbers = values.Count;

            // Create a new Result object with the calculated values
            var result = new Result
            {
                // Set the properties of the result object
                file_id = fileId,
                All_Time = allTime,
                Minimum_Date = minimumDate,
                Average_Time = averageTime,
                Average_Indicator = averageIndicator,
                Median_Indicator = medianIndicator,
                Maximum_Indicator = maxIndicator,
                Minimum_Indicator = minIndicator,
                Row_Numbers = rowNumbers
            };

            // Add the result to the database
            _context.results.Add(result);

            return result;
        }
        private bool ValidateRecord(value record)
        {
            // Validate Date
            if (record.DateAndTime1 > DateTime.UtcNow || record.DateAndTime1 < new DateTime(2000, 1, 1))
            {
                return false; // Date is out of range
            }

            // Validate Time
            if (record.IntegerTimeValue < 0)
            {
                return false; // Time is less than 0
            }

            // Validate Indicator
            if (record.FloatingPointIndicator < 0)
            {
                return false; // Indicator is less than 0
            }

           

            return true; // Record is valid
        }


        public async Task<List<value>> GetValuesByFileNameAsync(string fileName)
        {
            // Get the file ID using the file name
            var fileId = await _context.files
                .Where(f => f.file_name == fileName)
                .Select(f => f.file_id)
                .FirstOrDefaultAsync();

            // If file ID is found, retrieve values associated with it
            if (fileId != null)
            {
                return await _context.values
                    .Where(v => v.File_Id == fileId)
                    .ToListAsync();
            }
            else
            {
                // Handle case where file with given name does not exist
                return null;
            }
        }
        public async Task<List<Result>> GetResultsAsync(string fileName, int? averageMin, int? averageMax, double? indicatorMin, double? indicatorMax, DateTime? from, DateTime? to)
        {
            IQueryable<Result> query = _context.results;
            if (!string.IsNullOrEmpty(fileName))
            {
                // Find the file ID by its name
                var fileId = await _context.files
                    .Where(f => f.file_name == fileName)
                    .Select(f => f.file_id)
                    .FirstOrDefaultAsync();

                // If the file ID is not found, return an empty list
                if (fileId == null)
                {
                    return new List<Result>();
                }

                // Filter results by the file ID
                query = query.Where(r => r.file_id == fileId);
            }
            // Filter by average time range
            if (averageMin != null)
            {
                query = query.Where(r => r.Average_Time >= averageMin);
            }

            if (averageMax != null)
            {
                query = query.Where(r => r.Average_Time <= averageMax);
            }

            // Filter by indicator range
            if (indicatorMin != null)
            {
                query = query.Where(r => r.Average_Indicator >= indicatorMin);
            }

            if (indicatorMax != null)
            {
                query = query.Where(r => r.Average_Indicator <= indicatorMax);
            }

            // Convert 'from' and 'to' to UTC
            DateTime? fromUtc = from?.ToUniversalTime();
            DateTime? toUtc = to?.ToUniversalTime();

            // Filter by date range
            if (fromUtc != null && toUtc != null)
            {
                query = query.Where(r => r.Minimum_Date >= fromUtc && r.Minimum_Date <= toUtc);
            }

            // Execute the query and return the results
            return await query.ToListAsync();
        }

    }
}