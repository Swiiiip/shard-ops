using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Model.Users;

namespace Shard.RayanCedric.API.Gateway;

public interface IShardGateway
{
    Task<string> ExecuteJump(string serverName, User user, Unit unit);
}