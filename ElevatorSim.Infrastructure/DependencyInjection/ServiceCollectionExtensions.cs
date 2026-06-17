using ElevatorSim.Application.Controllers;
using ElevatorSim.Application.Dispatchers;
using ElevatorSim.Application.Elevators;
using ElevatorSim.Domain.Interfaces;
using ElevatorSim.Infrastructure.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace ElevatorSim.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddElevatorSimulation(
        this IServiceCollection services,
        SimulationConfig config)
    {
        services.AddSingleton(config);

        services.AddSingleton<IDispatchStrategy, NearestAvailableDispatchStrategy>();

        RegisterPassengerElevators(services, config);
        RegisterFreightElevators(services, config);
        RegisterHighSpeedElevators(services, config);

        services.AddSingleton<IElevatorController, ElevatorController>();
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
