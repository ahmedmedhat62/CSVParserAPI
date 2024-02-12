using InfoTecs_API.Interfaces;
using InfoTecs_API.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/EmployeeController")]
public class EmployeeController : ControllerBase
{
    private readonly IFiles _csvService;

    public EmployeeController(IFiles csvService)
    {
        _csvService = csvService;
    }

    [HttpPost("read-employees-csv")]
    public async Task<IActionResult> GetEmployeeCSV([FromForm] IFormFileCollection files)
    {
        // Ensure that at least one file is uploaded
        if (files == null || files.Count == 0)
        {
            return BadRequest("No file uploaded.");
        }

        foreach (var file in files)
        {
            if (file.Length > 0)
            {
                // Extract the name of the uploaded file
                var fileName = file.FileName;

                try
                {
                    // Read and save the CSV file content along with its name to the database
                    var savedFile = await _csvService.ReadAndSaveCSVAsync<value>(file.OpenReadStream(), fileName);

                    // If savedFile is null, it indicates that the validation failed
                    if (savedFile == null)
                    {
                        return BadRequest("Validation failed. Please check the file and try again.");
                    }

                    // Return the saved CSV file information
                    return Ok(savedFile);
                }
                catch (Exception ex)
                {
                    // Log the exception
                    // You can also return a custom error message if needed
                    return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the file.");
                }
            }
        }

        return BadRequest("No file uploaded.");
    }
    [HttpGet("get-values/{fileName}")]
    public async Task<IActionResult> GetValuesByFileName(string fileName)
    {
        // Call the service method to get values by file name
        var values = await _csvService.GetValuesByFileNameAsync(fileName);

        if (values == null)
        {
            return NotFound($"File with name '{fileName}' not found.");
        }

        return Ok(values);
    }
    [HttpGet("get-results")]
    public async Task<IActionResult> GetResults([FromQuery] string fileName = null, [FromQuery] int? averageMin = null, [FromQuery] int? averageMax = null, [FromQuery] double? indicatorMin = null, [FromQuery] double? indicatorMax = null, [FromQuery]DateTime? from = null, [FromQuery]DateTime? to = null)
    {
        try
        {
            // Call the service method to get results based on filtering criteria
            var results = await _csvService.GetResultsAsync(fileName, averageMin, averageMax, indicatorMin, indicatorMax,from,to);

            if (results == null || results.Count == 0)
            {
                return NotFound("No results found matching the provided criteria.");
            }

            return Ok(results);
        }
        catch (Exception ex)
        {
            // Log the exception
            // You can also return a custom error message if needed
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching results.");
        }
    }

}