using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElevatorSimulation(
        this IServiceCollection services,
        SimulationConfig config)
    {
        services.AddSingleton(config);

        services.AddSingleton<IDispatchStrategy, NearestAvailableDispatchStrategy>();

        for (int i = 0; i < config.NumberOfElevators; i++)
        {
            int elevatorIndex = i + 1;
            services.AddSingleton<IElevator>(
                _ => new PassengerElevator(
                    capacity: config.ElevatorCapacity,
                    startFloor: 1));
        }

        services.AddSingleton<IElevatorController, ElevatorController>();
        return services;
    }
}
