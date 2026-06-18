using ElevatorSim.Application.Controllers;
using ElevatorSim.Application.Dispatchers;
using ElevatorSim.Application.Elevators;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSim.Infrastructure.DependencyInjection;

/// <summary>
/// A static class that provides extension methods for registering elevator simulation services with the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the elevator simulation services with the dependency injection container, including elevators, dispatch strategy, and controller,
    /// based on the provided simulation configuration.
    /// </summary>
    /// <param name="services">The dependency injection container.</param>
    /// <param name="config">The simulation configuration.</param>
    /// <returns>The updated dependency injection container.</returns>
    public static IServiceCollection AddElevatorSimulation(
        this IServiceCollection services,
        SimulationConfig config)
    {
        const string LogFilePath = "logs/elevator-sim.log";
        services.AddSingleton(config);

        services.AddSingleton<IDispatchStrategy, NearestAvailableDispatchStrategy>();

        RegisterPassengerElevators(services, config);
        RegisterFreightElevators(services, config);
        RegisterHighSpeedElevators(services, config);

        services.AddSingleton<IElevatorController, ElevatorController>();
        services.AddSingleton<ILogViewer>(_ => new FileLogViewer(LogFilePath));
        return services;
    }

    private static void RegisterPassengerElevators(
        IServiceCollection services, SimulationConfig config)
    {
        for (int i = 0; i < config.NumberOfElevators; i++)
        {
            services.AddSingleton<IElevator>(
                _ => new PassengerElevator(
                    capacity: config.ElevatorCapacity,
                    startFloor: 1));
        }
    }

    private static void RegisterFreightElevators(
        IServiceCollection services, SimulationConfig config)
    {
        for (int i = 0; i < config.NumberOfFreightElevators; i++)
        {
            services.AddSingleton<IElevator>(
                _ => new FreightElevator(startFloor: 1));
        }
    }

    private static void RegisterHighSpeedElevators(
        IServiceCollection services, SimulationConfig config)
    {
        for (int i = 0; i < config.NumberOfHighSpeedElevators; i++)
        {
            services.AddSingleton<IElevator>(
                _ => new HighSpeedElevator(startFloor: 1));
        }
    }
}
