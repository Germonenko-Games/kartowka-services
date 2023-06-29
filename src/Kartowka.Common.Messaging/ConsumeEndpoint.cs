using Kartowka.Core.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kartowka.Common.Messaging;

internal class ConsumeEndpoint<TModel> : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    private readonly IReceiver<TModel> _receiver;

    private readonly ILogger<ConsumeEndpoint<TModel>> _logger;

    private readonly EndpointOptions<TModel> _options;

    public ConsumeEndpoint(
        IServiceProvider serviceProvider,
        IReceiver<TModel> receiver,
        ILogger<ConsumeEndpoint<TModel>> logger,
        EndpointOptions<TModel> options
    )
    {
        _serviceProvider = serviceProvider;
        _receiver = receiver;
        _logger = logger;
        _options = options;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var semaphore = new SemaphoreSlim(_options.MaxConcurrentMessages);
        var endpointModelType = typeof(TModel);

        while (!stoppingToken.IsCancellationRequested)
        {
            await semaphore.WaitAsync(stoppingToken);
            var message = await _receiver.ReceiveNextAsync(stoppingToken);
            // If null is returned in case of cancellation request.
            if (stoppingToken.IsCancellationRequested || message is null)
            {
                return;
            }

            _logger.LogInformation(
                "Endpoint {Model}: received message: '{Message}'",
                endpointModelType.FullName ?? endpointModelType.Name,
                message.RawBody
            );

            _ = Task.Run(async () =>
            {
                try
                {
                    var scope = _serviceProvider.CreateScope();
                    var consumers = scope.ServiceProvider.GetServices<IConsumer<TModel>>();

                    foreach (var consumer in consumers)
                    {
                        await consumer.ConsumeAsync(message, stoppingToken);
                    }

                    if (!stoppingToken.IsCancellationRequested)
                    {
                        await _receiver.CompleteAsync(message);
                    }
                }
                catch (KartowkaInfrastructureException e)
                {
                    _logger.LogCritical(e,
                        "Endpoint {Model}: critical infrastructure error occurred",
                        endpointModelType.FullName ?? endpointModelType.Name
                    );

                    await _receiver.DeadLetterAsync(message);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e,
                        "Endpoint {Model}: unhandled error occurred",
                        endpointModelType.FullName ?? endpointModelType.Name
                    );

                    await _receiver.DeadLetterAsync(message);
                }
                finally
                {
                    semaphore.Release();
                }
            }, stoppingToken);
        }
    }
}
