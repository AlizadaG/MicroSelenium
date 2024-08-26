using MassTransit;
using Shared.Dtos.Jobs;

namespace SeleniumApplication.Consumers;

public class JobSearchConsumer : IConsumer<JobSearchDto>
{
    public Task Consume(ConsumeContext<JobSearchDto> context)
    {
        //throw new NotImplementedException();


        // TODO:FindJob metodu bu alan içerisine taşınacak.
        return Task.CompletedTask;
    }
}
