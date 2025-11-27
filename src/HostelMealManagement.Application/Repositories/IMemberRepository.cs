using AutoMapper;
using HostelMealManagement.Application.CommonModel;
using HostelMealManagement.Application.FileServices;
using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using HostelMealManagement.Infrastructure.Helper.Acls;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static HostelMealManagement.Core.Entities.Auth.IdentityModel;

public interface IMemberRepository : IBaseService<Member>
{
    Task<bool> CreateOrUpdateMemberWithUserAsync(MemberVm vm,CancellationToken cancellationToken);
}

public class MemberRepository : BaseService<Member>, IMemberRepository
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly ApplicationDbContext _context;
    private readonly ISignInHelper _signInHelper;
    private readonly IFileService _fileService;

    public MemberRepository(ApplicationDbContext context, IMapper mapper, UserManager<User> userManager, ISignInHelper signInHelper, IFileService fileService)
        : base(context)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInHelper = signInHelper;
        _fileService = fileService;
    }

    public async Task<bool> CreateOrUpdateMemberWithUserAsync(MemberVm vm, CancellationToken cancellationToken)
    {
        if (vm == null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var isNewMember = vm.Id == 0;
            Member member;

            if (isNewMember)
            {
                member = _mapper.Map<Member>(vm);
                member.CreatedBy = _signInHelper.UserId ?? 0;
                member.CreatedDate = DateTimeOffset.UtcNow;

                if (vm.ImageFile != null)
                {
                    member.Picture = await _fileService.Upload(vm.ImageFile, CommonVariables.ProfileLocation);
                }

                await _context.Set<Member>().AddAsync(member, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // Check if email already exists
                if (await _userManager.FindByEmailAsync(vm.Email) != null)
                    throw new Exception($"Email '{vm.Email}' is already registered.");

                var user = new User
                {
                    Email = vm.Email,
                    UserName = vm.Email,
                    PhoneNumber = vm.PhoneNumber,
                    MemberId = member.Id,
                    Name = vm.Name,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString()
                };

                var createResult = await _userManager.CreateAsync(user, vm.Password);
                if (!createResult.Succeeded)
                    throw new Exception(string.Join(", ", createResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(user, "Member");
            }
            else
            {
                member = await _context.Set<Member>().FirstOrDefaultAsync(m => m.Id == vm.Id, cancellationToken);
                if (member == null) return false;

                _mapper.Map(vm, member);

                if (vm.ImageFile != null)
                {
                    if (!string.IsNullOrEmpty(member.Picture))
                        _fileService.DeleteFile(member.Picture, CommonVariables.ProfileLocation);
                    member.Picture = await _fileService.Upload(vm.ImageFile, CommonVariables.ProfileLocation);
                }

                member.ModifiedBy = _signInHelper.UserId;
                member.ModifiedDate = DateTimeOffset.UtcNow;
                _context.Set<Member>().Update(member);

                if (!string.IsNullOrWhiteSpace(vm.Password))
                {
                    var user = await _context.Set<User>().FirstOrDefaultAsync(u => u.MemberId == member.Id, cancellationToken);
                    if (user != null)
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetResult = await _userManager.ResetPasswordAsync(user, token, vm.Password);
                        if (!resetResult.Succeeded)
                            throw new Exception(string.Join(", ", resetResult.Errors.Select(e => e.Description)));
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            Console.WriteLine(ex.Message);
            return false; // Removed unreachable throw
        }
    }

}
