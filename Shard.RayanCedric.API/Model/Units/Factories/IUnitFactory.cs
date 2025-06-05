using Shard.RayanCedric.API.Model.Sector;

namespace Shard.RayanCedric.API.Model.Units.Factories;
public interface IUnitFactory
{
    Unit CreateUnit(StarSystem starSystem, Planet? planet);
}