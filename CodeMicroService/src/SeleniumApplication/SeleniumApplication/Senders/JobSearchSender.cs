using MassTransit;
using Shared.Dtos.Jobs;

namespace SeleniumApplication.Senders;

public class JobSearchSender
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    public JobSearchSender(ISendEndpointProvider sendEndpointProvider)
    => _sendEndpointProvider = sendEndpointProvider;

    public async Task SendJobSearchMessageAsync(JobSearchDto model)
    {
        var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:jobsearch-queue"));
        await sendEndpoint.Send(model);
    }
}
