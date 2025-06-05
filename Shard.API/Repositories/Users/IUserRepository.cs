using Shard.API.Model.Buildings;
using Shard.API.Model.Units;
using Shard.API.Model.Users;

namespace Shard.API.Repositories.Users;

public interface IUserRepository
{
    List<User> FindAllUsers();
    User FindUserById(string userId);
    User FindUserByUnitId(string unitId);
    User FindUserByBuildingId(string buildingId);
    void SaveUser(User user);
    List<Building> FindAllBuildingsByUserId(string userId);
    Building FindBuildingByUserIdAndBuildingId(string userId, string buildingId);
    void SaveBuildingToUser(User user, Building building);
    void DeleteBuildingFromUser(User user, Building building);
    List<Unit> FindAllUnitsByUserId(string userId);
    Unit FindUnitByUserIdAndUnitId(string userId, string unitId);
    void SaveUnitToUser(User user, Unit unit);
    void DeleteUnitFromUser(User user, Unit unit);
}