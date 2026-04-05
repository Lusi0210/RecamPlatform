using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Remp.Remp.DataAccess;
using Remp.Remp.Models.DTOs;
using Remp.Remp.Models.Entities;
using Remp.Remp.Models.Interfaces.Services;

namespace Remp.Remp.Service;

public class AuthService : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;
    private readonly RempDbContext _dbContext;

    public AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration, RempDbContext dbContext)
    {
        _userManager = userManager;
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
    {
        IdentityUser? user = await _userManager.FindByEmailAsync(loginRequestDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        bool isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? "Agent";

        string token = GenerateJwtToken(user, role);

        LoginResponseDto loginResponseDto = new LoginResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = role,
            Token = token
        };

        if (role == "Admin")
        {
            PhotographyCompany? company = await _dbContext.PhotographyCompanies
                .FirstOrDefaultAsync(p => p.Id == user.Id);
            if (company != null)
            {
                loginResponseDto.PhotographyCompanyName = company.PhotographyCompanyName;
            }
        }
        else if (role == "Agent")
        {
            Agent? agent = await _dbContext.Agents
                .FirstOrDefaultAsync(a => a.Id == user.Id);
            if (agent != null)
            {
                loginResponseDto.AgentFirstName = agent.AgentFirstName;
                loginResponseDto.AgentLastName = agent.AgentLastName;
                loginResponseDto.AvatarUrl = agent.AvatarUrl;
                loginResponseDto.CompanyName = agent.CompanyName;
            }
        }

        return loginResponseDto;
    }

    private string GenerateJwtToken(IdentityUser user, string role)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;

        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role)
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationInMinutes"])),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<RegisterResponseDto> RegisterAgentAsync(RegisterRequestDto registerRequestDto)
    {
        IdentityUser? existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        IdentityUser newUser = new IdentityUser
        {
            UserName = registerRequestDto.Email,
            Email = registerRequestDto.Email
        };

        IdentityResult result = await _userManager.CreateAsync(newUser, registerRequestDto.Password);
        if (!result.Succeeded)
        {
            string errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Registration failed: {errors}");
        }

        await _userManager.AddToRoleAsync(newUser, "Agent");

        Agent agent = new Agent
        {
            Id = newUser.Id,
            AgentFirstName = registerRequestDto.AgentFirstName,
            AgentLastName = registerRequestDto.AgentLastName,
            AvatarUrl = "",
            CompanyName = ""
        };

        await _dbContext.Agents.AddAsync(agent);
        await _dbContext.SaveChangesAsync();

        RegisterResponseDto responseDto = new RegisterResponseDto
        {
            UserId = newUser.Id,
            Email = newUser.Email!,
            Role = "Agent",
            AgentFirstName = agent.AgentFirstName,
            AgentLastName = agent.AgentLastName
        };

        return responseDto;
    }

    public async Task<PaginatedResponseDto<UserResponseDto>> GetAllUsersAsync(PaginationRequestDto paginationRequest)
    {
        // Get all users with Agent role
        IList<IdentityUser> agentUsers = await _userManager.GetUsersInRoleAsync("Agent");

        int totalCount = agentUsers.Count;
        int totalPages = (int)Math.Ceiling(totalCount / (double)paginationRequest.PageSize);

        List<IdentityUser> pagedUsers = agentUsers
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToList();

        List<UserResponseDto> userDtos = new List<UserResponseDto>();

        foreach (IdentityUser user in pagedUsers)
        {
            Agent? agent = await _dbContext.Agents
                .FirstOrDefaultAsync(a => a.Id == user.Id);

            UserResponseDto userDto = new UserResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                Role = "Agent",
                AgentFirstName = agent?.AgentFirstName,
                AgentLastName = agent?.AgentLastName,
                AvatarUrl = agent?.AvatarUrl,
                CompanyName = agent?.CompanyName
            };

            userDtos.Add(userDto);
        }

        PaginatedResponseDto<UserResponseDto> response = new PaginatedResponseDto<UserResponseDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = paginationRequest.PageNumber,
            PageSize = paginationRequest.PageSize,
            TotalPages = totalPages
        };

        return response;
    }

    public async Task<CurrentUserResponseDto> GetCurrentUserAsync(string userId)
    {
        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("User not found.");
        }

        IList<string> roles = await _userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? "Agent";

        CurrentUserResponseDto responseDto = new CurrentUserResponseDto
        {
            UserId = user.Id,
            Email = user.Email!,
            Role = role,
            AssignedListingCaseIds = new List<int>()
        };

        if (role == "Admin")
        {
            PhotographyCompany? company = await _dbContext.PhotographyCompanies
                .FirstOrDefaultAsync(p => p.Id == user.Id);
            if (company != null)
            {
                responseDto.PhotographyCompanyName = company.PhotographyCompanyName;
            }

            responseDto.AssignedListingCaseIds = await _dbContext.ListingCases
                .Where(l => l.UserId == user.Id && !l.IsDeleted)
                .Select(l => l.Id)
                .ToListAsync();
        }
        else if (role == "Agent")
        {
            Agent? agent = await _dbContext.Agents
                .FirstOrDefaultAsync(a => a.Id == user.Id);
            if (agent != null)
            {
                responseDto.AgentFirstName = agent.AgentFirstName;
                responseDto.AgentLastName = agent.AgentLastName;
                responseDto.AvatarUrl = agent.AvatarUrl;
                responseDto.CompanyName = agent.CompanyName;
            }

            responseDto.AssignedListingCaseIds = await _dbContext.AgentListingCases
                .Where(al => al.AgentId == user.Id)
                .Select(al => al.ListingCaseId)
                .ToListAsync();
        }

        return responseDto;
    }


}
