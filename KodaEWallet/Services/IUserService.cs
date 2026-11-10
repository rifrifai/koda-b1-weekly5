using KodaEWallet.DTOs;

namespace KodaEWallet.Services;
public interface IUserService
{
    Task<List<UserResponse>> GetAllAsync(CancellationToken ct);
    Task<UserResponse> GetByIdAsync(int id, CancellationToken ct);
    Task<UserResponse> CreateAsync(CreateUserRequest request, CancellationToken ct);
    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, CancellationToken ct);
}
