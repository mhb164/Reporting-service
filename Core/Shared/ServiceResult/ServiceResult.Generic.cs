﻿namespace Tizpusoft;

public class ServiceResult<T>
{
    public Guid? TrackingId { get; private set; }
    public T? Value { get; private set; }
    public string? Message { get; private set; }
    public ServiceResultCode Code { get; private set; }

    public ServiceResult()
    {
        Code = ServiceResultCode.NotImplemented;
    }

    public ServiceResult<T> Success(T value)
    {
        Value = value;
        Code = ServiceResultCode.Success;
        return this;
    }

    public ServiceResult<T> InternalError(Guid trackingId, string message)
    {
        TrackingId = trackingId;
        Value = default;
        Code = ServiceResultCode.InternalError;
        Message = message;

        return this;
    }

    private ServiceResult<T> Failed(ServiceResultCode code, string message)
    {
        Value = default;
        Code = code;
        Message = message;

        return this;
    }

    public ServiceResult<T> BadRequest(string message) => Failed(ServiceResultCode.BadRequest, message);
    public ServiceResult<T> Unauthorized(string message) => Failed(ServiceResultCode.Unauthorized, message);
    public ServiceResult<T> Forbidden(string message) => Failed(ServiceResultCode.Forbidden, message);
    public ServiceResult<T> NotFound(string message) => Failed(ServiceResultCode.NotFound, message);
    public ServiceResult<T> Conflict(string message) => Failed(ServiceResultCode.Conflict, message);
    public ServiceResult<T> NotImplemented(string message) => Failed(ServiceResultCode.NotImplemented, message);
    public ServiceResult<T> ServiceUnavailable(string message) => Failed(ServiceResultCode.ServiceUnavailable, message);

}
