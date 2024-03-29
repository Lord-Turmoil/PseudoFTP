﻿// Copyright (C) 2018 - 2024 Tony's Studio. All rights reserved.

using AutoMapper;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;
using Profile = PseudoFTP.Model.Database.Profile;

namespace PseudoFTP.Api;

public class AutoMapperProfile : MapperConfigurationExpression
{
    public AutoMapperProfile()
    {
        CreateMap<Profile, ProfileDto>();
        CreateMap<ProfileDto, Profile>();

        CreateMap<TransferHistory, TransferHistoryDto>();
        CreateMap<Profile, ProfileDto>();
    }
}