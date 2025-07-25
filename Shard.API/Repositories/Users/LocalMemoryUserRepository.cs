using Shard.API.Model.Buildings;
using Shard.API.Model.Units;
using Shard.API.Model.Users;

namespace Shard.API.Repositories.Users;

public class LocalMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users;

    public LocalMemoryUserRepository()
    {
        _users = [];
    }
    
    public List<User> FindAllUsers()
    {
        return _users;
    }

    public User FindUserById(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            throw new ArgumentNullException(nameof(userId), "cannot be null or empty.");
        
        var existingUser = _users.Find(user => user.Id == userId);
        return existingUser ?? throw new KeyNotFoundException($"User with id '{userId}' not found.");
    }

    public User FindUserByUnitId(string unitId)
    {
        var existingUser = _users.Find(user => user.Units.Any(unit => unit.Id == unitId));
        
        return existingUser ?? throw new KeyNotFoundException($"No user found for unit with id '{unitId}'.");
    }

    public User FindUserByBuildingId(string buildingId)
    {
        var existingUser = _users.Find(user => user.Buildings.Any(building => building.Id == buildingId));
        
        return existingUser ?? throw new KeyNotFoundException($"No user found for building with id '{buildingId}'.");
    }

    public void SaveUser(User user)
    {
        try
        {
            var existingUser = FindUserById(user.Id);
            UpdateExistingUser(existingUser, user);

        }
        catch (KeyNotFoundException)
        {
            _users.Add(user);
        }
    }

    private void UpdateExistingUser(User existingUser, User updatedUser)
    {
        existingUser.Pseudo = updatedUser.Pseudo;
        existingUser.Units = updatedUser.Units;
        existingUser.Buildings = updatedUser.Buildings;
    }

    public List<Building> FindAllBuildingsByUserId(string userId)
    {
        var existingUser = FindUserById(userId);
        return existingUser.Buildings;
    }

    public Building FindBuildingByUserIdAndBuildingId(string userId, string buildingId)
    {
        var existingUser = FindUserById(userId);
        return existingUser.GetBuilding(buildingId);
    }

    public void SaveBuildingToUser(User user, Building building)
    {
        user.AddBuilding(building);
        SaveUser(user);
    }

    public void DeleteBuildingFromUser(User user, Building building)
    {
        user.RemoveBuilding(building);
        SaveUser(user);
    }

    public List<Unit> FindAllUnitsByUserId(string userId)
    {
        var existingUser = FindUserById(userId);
        return existingUser.Units;
    }

    public Unit FindUnitByUserIdAndUnitId(string userId, string unitId)
    {
        var existingUser = FindUserById(userId);
        return existingUser.GetUnit(unitId);
    }

    public void SaveUnitToUser(User user, Unit unit)
    {
        user.AddUnit(unit);
        SaveUser(user);
    }

    public void DeleteUnitFromUser(User user, Unit unit)
    {
        user.RemoveUnit(unit);
        SaveUser(user);
    }
}
