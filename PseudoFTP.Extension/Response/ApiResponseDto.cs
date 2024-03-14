// Copyright (C) 2018 - 2024 Tony's Studio. All rights reserved.

namespace Tonisoft.AspExtensions.Response;

public class ApiResponseDto
{
    public ApiResponseDto(int status, string? message = null, object? data = null)
    {
        Meta = new ApiResponseMeta(status, message);
        Data = data;
    }

    public ApiResponseMeta Meta { get; set; }
    public object? Data { get; set; }
}

public class ApiResponseDto<TData>
{
    public ApiResponseDto(int status, string? message = null, TData? data = default)
    {
        Meta = new ApiResponseMeta(status, message);
        Data = data;
    }

    public ApiResponseMeta Meta { get; set; }
    public TData? Data { get; set; }
}

public class ApiResponseMeta
{
    public ApiResponseMeta()
    {
        Status = 0;
        Message = null;
    }


    public ApiResponseMeta(int status, string? message = null)
    {
        Status = status;
        Message = message;
    }

    public int Status { get; set; }
    public string? Message { get; set; }
}