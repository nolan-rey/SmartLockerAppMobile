using SmartLockerApp.DTOs;
using SmartLockerApp.Models;

namespace SmartLockerApp.Services;

/// <summary>
/// Service de mapping entre les DTOs de l'API et les modèles de l'application
/// Conversion bidirectionnelle simple et claire
/// </summary>
public static class ApiMappingService
{
    #region User Mapping

    /// <summary>
    /// Convertit un UserDto en User
    /// </summary>
    public static User ToModel(this UserDto dto)
    {
        return new User
        {
            Id = dto.id,
            Name = dto.name,
            Email = dto.email,
            Role = dto.role,
            CreatedAt = dto.created_at,
            UpdatedAt = dto.updated_at
        };
    }

    /// <summary>
    /// Convertit un User en UserDto
    /// </summary>
    public static UserDto ToDto(this User model)
    {
        return new UserDto
        {
            id = model.Id,
            name = model.Name,
            email = model.Email,
            role = model.Role,
            created_at = model.CreatedAt,
            updated_at = model.UpdatedAt
        };
    }

    #endregion

    #region Locker Mapping

    /// <summary>
    /// Convertit un LockerDto en Locker
    /// </summary>
    public static Locker ToModel(this LockerDto dto)
    {
        return new Locker
        {
            Id = dto.id,
            Name = dto.name,
            Status = dto.status,
            CreatedAt = dto.created_at,
            UpdatedAt = dto.updated_at,
            LastOpenedAt = dto.last_opened_at
        };
    }

    /// <summary>
    /// Convertit un Locker en LockerDto
    /// </summary>
    public static LockerDto ToDto(this Locker model)
    {
        return new LockerDto
        {
            id = model.Id,
            name = model.Name,
            status = model.Status,
            created_at = model.CreatedAt,
            updated_at = model.UpdatedAt,
            last_opened_at = model.LastOpenedAt
        };
    }

    #endregion

    #region Session Mapping

    /// <summary>
    /// Convertit un SessionDto en LockerSession
    /// </summary>
    public static LockerSession ToModel(this SessionDto dto)
    {
        return new LockerSession
        {
            Id = dto.id,
            UserId = dto.user_id,
            LockerId = dto.locker_id,
            Status = dto.status,
            StartedAt = dto.started_at,
            EndedAt = dto.ended_at,
            PlannedEndAt = dto.planned_end_at,
            AmountDue = dto.amount_due,
            Currency = dto.currency,
            PaymentStatus = dto.payment_status,
            CreatedAt = dto.created_at,
            UpdatedAt = dto.updated_at
        };
    }

    /// <summary>
    /// Convertit un LockerSession en SessionDto
    /// </summary>
    public static SessionDto ToDto(this LockerSession model)
    {
        return new SessionDto
        {
            id = model.Id,
            user_id = model.UserId,
            locker_id = model.LockerId,
            status = model.Status,
            started_at = model.StartedAt,
            ended_at = model.EndedAt,
            planned_end_at = model.PlannedEndAt,
            amount_due = model.AmountDue,
            currency = model.Currency,
            payment_status = model.PaymentStatus,
            created_at = model.CreatedAt,
            updated_at = model.UpdatedAt
        };
    }

    #endregion

    #region Collection Mapping

    /// <summary>
    /// Convertit une liste de UserDto en liste de User
    /// </summary>
    public static List<User> ToModels(this List<UserDto> dtos)
    {
        return dtos.Select(dto => dto.ToModel()).ToList();
    }

    /// <summary>
    /// Convertit une liste de LockerDto en liste de Locker
    /// </summary>
    public static List<Locker> ToModels(this List<LockerDto> dtos)
    {
        return dtos.Select(dto => dto.ToModel()).ToList();
    }

    /// <summary>
    /// Convertit une liste de SessionDto en liste de LockerSession
    /// </summary>
    public static List<LockerSession> ToModels(this List<SessionDto> dtos)
    {
        return dtos.Select(dto => dto.ToModel()).ToList();
    }

    #endregion
}
