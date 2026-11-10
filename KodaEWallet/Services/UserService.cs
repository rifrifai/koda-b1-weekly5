using KodaEWallet.DTOs;
using KodaEWallet.Exceptions;
using KodaEWallet.Interfaces;
using KodaEWallet.Models;

namespace KodaEWallet.Services;

public class UserService(IUserRepository userRepo) : IUserService
{
    public async Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct)
    {
        string email = request.Email.Trim();
        if (await userRepo.EmailExistsAsync(email, null, ct))
        {
            throw new ConflictException($"Email {email} sudah digunakan");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim()
        };

        User created = await userRepo.CreateWithWalletAsync(user, ct);
        return ToResponse(created);
    }

    public async Task<List<UserResponse>> GetAllAsync(CancellationToken ct)
    {
        var users = await userRepo.GetAllAsync(ct);
        return users.Select(ToResponse).ToList();
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken ct)
    {
        User user = await userRepo.GetByIdAsync(id, ct) ?? throw new NotFoundException($"User dengan id {id} tidak ditemukan");
        return ToResponse(user);
    }

    public async Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct)
    {
        _ = await userRepo.GetByIdAsync(id, ct)
            ?? throw new NotFoundException($"User dengan id {id} tidak ditemukan");

        string email = request.Email.Trim();
        if (await userRepo.EmailExistsAsync(email, id, ct))
        {
            throw new ConflictException($"Email {email} sudah digunakan");
        }

        var user = new User
        {
            Id = id,
            Name = request.Name.Trim(),
            Email = email,
            Phone = request.Phone.Trim()
        };
        await userRepo.UpdateAsync(user, ct);
        return ToResponse(user);
    }
    private static UserResponse ToResponse(User user) => new UserResponse
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Phone = user.Phone
    };
}