﻿// Copyright (C) 2018 - 2024 Tony's Studio. All rights reserved.

using Microsoft.AspNetCore.Http;

namespace Tonisoft.AspExtensions.Response;

public class OkResponse : ApiResponse
{
    public OkResponse(object? value) : base(StatusCodes.Status200OK, value)
    {
    }
}

public class BadRequestResponse : ApiResponse
{
    public BadRequestResponse(object? value) : base(StatusCodes.Status400BadRequest, value)
    {
    }
}

public class UnauthorizedResponse : ApiResponse
{
    public UnauthorizedResponse(object? value) : base(StatusCodes.Status401Unauthorized, value)
    {
    }
}

public class ForbiddenResponse : ApiResponse
{
    public ForbiddenResponse(object? value) : base(StatusCodes.Status403Forbidden, value)
    {
    }
}

public class NotFoundResponse : ApiResponse
{
    public NotFoundResponse(object? value) : base(StatusCodes.Status404NotFound, value)
    {
    }
}

public class MethodNotAllowedResponse : ApiResponse
{
    public MethodNotAllowedResponse(object? value) : base(StatusCodes.Status405MethodNotAllowed, value)
    {
    }
}

public class InternalServerErrorResponse : ApiResponse
{
    public InternalServerErrorResponse(object? value) : base(StatusCodes.Status500InternalServerError, value)
    {
    }
}