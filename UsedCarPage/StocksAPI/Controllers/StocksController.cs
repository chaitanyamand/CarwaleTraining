using Microsoft.AspNetCore.Mvc;
using StocksAPI.BAL;
using StocksAPI.BAL.Interfaces;
using StocksAPI.DTOs;

namespace StocksAPI.Controllers
{
    /* Controller for handling stock-related API endpoints */
    [ApiController]
    [Route("api/[controller]")]
    public class StocksController : ControllerBase
    {
        private readonly IStockBAL _stockBAL;
        private readonly ILogger<StocksController> _logger;

        /* Constructor with dependency injection */
        public StocksController(IStockBAL stockBAL, ILogger<StocksController> logger)
        {
            _stockBAL = stockBAL ?? throw new ArgumentNullException(nameof(stockBAL));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /* 
         * GET: api/stocks
         * Search stocks based on provided query filters
         */
        [HttpGet]
        [ProducesResponseType(typeof(StockSearchResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchStocks([FromQuery] StockSearchRequestDTO request)
        {
            try
            {
                _logger.LogInformation("Searching stocks with filters: {@Request}", request);

                var response = await _stockBAL.SearchStocksAsync(request);

                _logger.LogInformation("Successfully retrieved {Count} stocks out of {Total}",
                    response.Stocks.Count, response.TotalCount);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Invalid argument provided for stock search");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching stocks");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An internal server error occurred. Please try again later.");
            }
        }

        /*
         * GET: api/stocks/{id}
         * Retrieve details of a stock by its ID
         */
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StockDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStockById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid stock ID provided: {Id}", id);
                    return BadRequest("Stock ID must be greater than 0");
                }

                _logger.LogInformation("Fetching stock with ID: {Id}", id);

                var stock = await _stockBAL.GetStockByIdAsync(id);

                if (stock == null)
                {
                    _logger.LogWarning("Stock not found with ID: {Id}", id);
                    return NotFound($"Stock with ID {id} not found");
                }

                _logger.LogInformation("Successfully retrieved stock with ID: {Id}", id);
                return Ok(stock);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching stock with ID: {Id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "An internal server error occurred. Please try again later.");
            }
        }

        /*
         * GET: api/stocks/health
         * Health check endpoint to verify service status
         */
        [HttpGet("health")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
        {
            return Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
        }
    }
}
