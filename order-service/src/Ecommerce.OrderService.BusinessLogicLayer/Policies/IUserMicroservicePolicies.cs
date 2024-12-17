using Polly;

namespace Ecommerce.OrderService.BusinessLogicLayer.Policies;


public interface IUserMicroservicePolicies
{
    IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy();
}