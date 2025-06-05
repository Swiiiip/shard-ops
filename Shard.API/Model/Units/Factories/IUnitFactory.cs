using Shard.API.Model.Sector;

namespace Shard.API.Model.Units.Factories;
public interface IUnitFactory
{
    Unit CreateUnit(StarSystem starSystem, Planet? planet);
}