using System.Text.RegularExpressions;
using Shard.RayanCedric.API.Contracts;
using Shard.RayanCedric.API.Model.Buildings;
using Shard.RayanCedric.API.Model.Units;
using Shard.RayanCedric.API.Model.Units.Managers;
using Shard.RayanCedric.API.Model.Users;
using Shard.RayanCedric.API.Repositories.Users;

namespace Shard.RayanCedric.API.Services;

public partial class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly UnitCreationManager _unitCreationManager;

    [GeneratedRegex("^[a-zA-Z0-9-]+$")]
    private static partial Regex AllowedPattern();

    public UserService(IUserRepository userRepository, SectorService sectorService)
    {
        _userRepository = userRepository;
        _unitCreationManager = new UnitCreationManager(sectorService);
    } 
    
    public User CreateUser(string userId, UserContract user)
    {
        ValidateUserId(userId, user.Id);

        try
        {
            var existingUser = GetUser(userId);
            return existingUser;
        }
        catch (KeyNotFoundException)
        {
            var newUser = new User(user.Id, user.Pseudo);
            AddDefaultUnitsToUser(newUser);
            newUser.InitializeDefaultResourcesQuantity();

            Save(newUser);
            return newUser;
        }
    }

    public User UpdateUser(string userId, UserContract user)
    {
        ValidateUserId(userId, user.Id);
        var existingUser = GetUser(user.Id);
        
        if (user.ResourcesQuantity != null) existingUser.ResourcesQuantity = user.ResourcesQuantity;
        return existingUser;
    }

    public User CopyUser(UserContract user)
    {
        var copiedUser = new User(user.Id, user.Pseudo, user.DateOfCreation);
        Save(copiedUser);
        return copiedUser;
    }
    
    public List<User> GetAllUsers()
    {
        return _userRepository.FindAllUsers();
    }
    
    public User GetUser(string userId)
    {
        return _userRepository.FindUserById(userId);
    }
    
    public User GetUserByBuildingId(string buildingId)
    {
       return _userRepository.FindUserByBuildingId(buildingId);
    }

    public User GetUserByUnitId(string unitId)
    {
        return _userRepository.FindUserByUnitId(unitId);
    }
    
    public void Save(User user)
    {
        _userRepository.SaveUser(user);
    }

    public List<Building> GetBuildingsByUserId(string userId)
    {
        return _userRepository.FindAllBuildingsByUserId(userId);
    }

    public async Task<Building> GetBuildingOfUser(string userId, string buildingId)
    {
        var building = _userRepository.FindBuildingByUserIdAndBuildingId(userId, buildingId);
        await building.WaitIfInConstruction(this);

        return building;
    }

    public void AddBuildingToUser(User user, Building building)
    {
        _userRepository.SaveBuildingToUser(user, building);
    }

    public void RemoveBuildingFromUser(User user, Building building)
    {
        _userRepository.DeleteBuildingFromUser(user, building);
    }

    public List<Unit> GetUnitsByUserId(string userId)
    {
        return _userRepository.FindAllUnitsByUserId(userId);
    }

    public async Task<Unit> GetUnitOfUser(string userId, string unitId)
    {
        var unit = _userRepository.FindUnitByUserIdAndUnitId(userId, unitId);
        await unit.WaitIfMoving();

        return unit;
    }

    public void AddUnitToUser(User user, Unit unit)
    {
        _userRepository.SaveUnitToUser(user, unit);
    }

    public void RemoveUnitFromUser(User user, Unit unit)
    {
        _userRepository.DeleteUnitFromUser(user, unit);
    }

    private void ValidateUserId(string urlUserId, string requestUserId)
    {
        if (string.IsNullOrEmpty(urlUserId))
            throw new ArgumentNullException(nameof(urlUserId), "User id in the URL cannot be null or empty.");

        if (string.IsNullOrEmpty(requestUserId))
            throw new ArgumentNullException(nameof(requestUserId), "User id in the request cannot be null or empty.");

        if (!AllowedPattern().IsMatch(urlUserId))
            throw new InvalidOperationException("User id in the URL contains invalid characters.");

        if (!AllowedPattern().IsMatch(requestUserId))
            throw new InvalidOperationException("User id in the request contains invalid characters.");

        if (urlUserId != requestUserId)
            throw new InvalidOperationException($"The user id in the request ({requestUserId}) must match the one in the URL ({urlUserId}).");
    }

    private void AddDefaultUnitsToUser(User user)
    {
        var newUnits = _unitCreationManager.CreateDefaultUnits();
        foreach (var unit in newUnits)
            user.AddUnit(unit);
    }

    public UserContract GetUserContract(User user)
    {
        return new UserContract(
            user.Id,
            user.Pseudo,
            user.DateOfCreation,
            user.ResourcesQuantity);
    }
}