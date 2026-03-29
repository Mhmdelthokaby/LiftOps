namespace Collins_BackEnd.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? UserEmail { get; }
}
