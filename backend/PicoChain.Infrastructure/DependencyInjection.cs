using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PicoChain.Infrastructure.Persistence;
using PicoChain.Application.Shipments;
using PicoChain.Infrastructure.Shipments;
using PicoChain.Application.Approvals;
using PicoChain.Infrastructure.Approvals;
using PicoChain.Application.Events;
using PicoChain.Infrastructure.Events;
using PicoChain.Infrastructure.Messaging;
using PicoChain.Application.Agents;
using PicoChain.Infrastructure.Agents;

namespace PicoChain.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<PicoChainDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PicoChain")));
        services.AddScoped<IShipmentQueryService, ShipmentQueryService>();
        services.AddScoped<IShipmentEventService, ShipmentEventService>();
        services.AddScoped<IAgentTaskService, AgentTaskService>();
        services.AddScoped<IAgentTaskQueryService, AgentTaskQueryService>();
        services.AddScoped<IApprovalService, ApprovalService>();
        services.AddScoped<IOutboxWriter, OutboxWriter>();
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.SectionName));
        services.AddSingleton<IRabbitMqPublisher, RabbitMqPublisher>();
        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}
