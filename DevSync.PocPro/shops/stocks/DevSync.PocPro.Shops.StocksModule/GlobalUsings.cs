// Global using directives

global using System.Collections.ObjectModel;
global using System.Net;
global using System.Security.Claims;
global using DevSync.PocPro.Shared.Domain.Abstractions;
global using DevSync.PocPro.Shared.Domain.Dtos;
global using DevSync.PocPro.Shared.Domain.Enums;
global using DevSync.PocPro.Shared.Domain.Exceptions;
global using DevSync.PocPro.Shops.Shared.Services;
global using DevSync.PocPro.Shops.StocksModule.Data;
global using DevSync.PocPro.Shops.StocksModule.Domains;
global using DevSync.PocPro.Shops.StocksModule.Domains.Enums;
global using DevSync.PocPro.Shops.StocksModule.Domains.Interfaces;
global using DevSync.PocPro.Shops.StocksModule.Domains.ValueObjects;
global using DevSync.PocPro.Shops.StocksModule.Features.Categories.V1.GetCategory;
global using FastEndpoints;
global using FluentResults;
global using FluentValidation;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;