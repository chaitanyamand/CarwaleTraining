using FinanceService.BAL;
using FinanceService.BAL.Interfaces;
using FinanceService.Protos;
using Grpc.Core;

namespace FinanceService.Controllers
{
    /*
     * gRPC service implementation for the Finance service.
     * Handles RPC calls related to car value-for-money evaluation.
     */
    public class FinanceGrpcService : Finance.FinanceBase
    {
        private readonly IFinanceBAL _financeBusinessLogic;
        private readonly ILogger<FinanceGrpcService> _logger;

        // Constructor injecting business logic and logger
        public FinanceGrpcService(IFinanceBAL financeBusinessLogic, ILogger<FinanceGrpcService> logger)
        {
            _financeBusinessLogic = financeBusinessLogic;
            _logger = logger;
        }

        /*
         * RPC method to determine whether each car is value for money.
         * Validates input and delegates logic to business layer.
         */
        public override async Task<ValueForMoneyResponse> GetIsValueForMoney(ValueForMoneyRequest request, ServerCallContext context)
        {
            try
            {
                _logger.LogInformation("GetIsValueForMoney RPC called");

                // Validate: null request
                if (request == null)
                {
                    _logger.LogWarning("Received null request");
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Request cannot be null"));
                }

                // Validate: empty car ID list
                if (request.CarIds == null || !request.CarIds.Any())
                {
                    _logger.LogWarning("Received empty car IDs list");
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Car IDs list cannot be null or empty"));
                }

                // Validate: invalid (non-positive) car IDs
                var invalidIds = request.CarIds.Where(id => id <= 0).ToList();
                if (invalidIds.Any())
                {
                    _logger.LogWarning($"Received invalid car IDs: {string.Join(", ", invalidIds)}");
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "Car IDs must be positive integers"));
                }

                _logger.LogInformation($"Processing request with {request.CarIds.Count} car IDs");

                // Convert gRPC request to internal data format
                var carIds = request.CarIds.ToList();

                // Invoke business logic layer
                var carStatuses = await _financeBusinessLogic.GetCarValueForMoneyStatusAsync(carIds);

                // Prepare gRPC response
                var response = new ValueForMoneyResponse();

                foreach (var carStatus in carStatuses)
                {
                    response.CarStatuses.Add(new CarStatus
                    {
                        Id = carStatus.Id,
                        IsValueForMoney = carStatus.IsValueForMoney
                    });
                }

                _logger.LogInformation($"Successfully processed {response.CarStatuses.Count} car statuses");
                return response;
            }
            catch (RpcException)
            {
                // Re-throw known gRPC exceptions without modifying them
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred in GetIsValueForMoney");

                // Convert unexpected errors to internal gRPC status
                throw new RpcException(new Status(StatusCode.Internal, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
