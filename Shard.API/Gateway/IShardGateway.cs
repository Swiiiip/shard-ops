using Shard.API.Model.Units;
using Shard.API.Model.Users;

namespace Shard.API.Gateway;

public interface IShardGateway
{
    Task<string> ExecuteJump(string serverName, User user, Unit unit);
}