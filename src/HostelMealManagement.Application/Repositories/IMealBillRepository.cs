using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostelMealManagement.Application.Repositories;

public interface IMealBillRepository: IBaseService<MealBill>
{
    Task<bool> GenerateMealBillAsync(long mealCycleId, long createdBy);
    Task<List<MealBill>> GetMealBillsWithMemberAsync(long mealCycleId, long? memberId);
}

public class MealBillRepository : BaseService<MealBill>, IMealBillRepository
{
    public MealBillRepository(ApplicationDbContext context) : base(context)
    {
    }
    public async Task<bool> GenerateMealBillAsync(long mealCycleId, long createdBy)
    {
        try
        {
            var sql = @"
      DECLARE
      @MealCycleId BIGINT = @MealCycleIdParam,
      @StartDate DATETIMEOFFSET,
      @EndDate DATETIMEOFFSET,
      @TotalBazarsAmount DECIMAL(18,2),
      @TotalMeals INT,
      @TotalGuestMeals INT,
      @GrandTotalMeals INT,
      @MealRate DECIMAL(18,2),
      @CreatedBy BIGINT = @CreatedByParam,
	  @CurrentBill DECIMAL(18,2),
	  @GasBill DECIMAL(18,2),
	  @ServantBill  DECIMAL(18,2),
	  @TotalMembers INT,
	  @TotalMealMembers INT,
	  @PerMemberCurrentBill DECIMAL(18,2),
	  @PerMemberGasBill DECIMAL(18,2),
	  @PerMemberServantBill DECIMAL(18,2)

  SELECT 
      @StartDate = StartDate,
      @EndDate   = EndDate
  FROM MealCycle
  WHERE Id = @MealCycleId;
    SELECT @TotalMealMembers = COUNT(*)
FROM Member
WHERE IsDelete = 0 and MealStatus = 1
  
  SELECT @TotalMembers = COUNT(*)
FROM Member
WHERE IsDelete = 0;

  SELECT 
    @CurrentBill = ISNULL(SUM(CurrentBill),0),
    @GasBill = ISNULL(SUM(GasBill),0),
    @ServantBill = ISNULL(SUM(ServantBill),0)
FROM UtilityBill
WHERE [Date] BETWEEN @StartDate AND @EndDate


SET @PerMemberCurrentBill = 
    CASE 
        WHEN @TotalMembers > 0 
        THEN @CurrentBill / @TotalMembers 
        ELSE 0 
    END;

	SET @PerMemberGasBill = 
    CASE 
        WHEN @TotalMembers > 0 
        THEN @GasBill / @TotalMealMembers 
        ELSE 0 
    END;
	SET @PerMemberServantBill = 
    CASE 
        WHEN @TotalMembers > 0 
        THEN @ServantBill / @TotalMealMembers 
        ELSE 0 
    END;
  
  SELECT 
      @TotalBazarsAmount = ISNULL(SUM(BazarAmount),0)
  FROM MealBazars
  WHERE BazarDate BETWEEN @StartDate AND @EndDate;
  
  SELECT
      @TotalMeals = ISNULL(SUM(
          CASE WHEN IsBreakfast=1 THEN 1 ELSE 0 END +
          CASE WHEN IsLunch=1 THEN 1 ELSE 0 END +
          CASE WHEN IsDinner=1 THEN 1 ELSE 0 END
      ),0),
      @TotalGuestMeals = ISNULL(SUM(
          ISNULL(GuestBreakfastQty,0) +
          ISNULL(GuestLunchQty,0) +
          ISNULL(GuestDinnerQty,0)
      ),0)
  FROM MealAttendances
  WHERE MealDate BETWEEN @StartDate AND @EndDate
    AND IsDelete = 0;
  
  SET @GrandTotalMeals = @TotalMeals + @TotalGuestMeals;
  SET @MealRate = CASE 
      WHEN @GrandTotalMeals > 0 
      THEN @TotalBazarsAmount / @GrandTotalMeals 
      ELSE 0 
  END;
  DELETE FROM MealBill
  WHERE MealCycleId = @MealCycleId;   
  
  ;WITH MemberMeals AS
  (
      SELECT
          MA.MemberId,
          TotalMemberMeal = SUM(
              CASE WHEN MA.IsBreakfast=1 THEN 1 ELSE 0 END +
              CASE WHEN MA.IsLunch=1 THEN 1 ELSE 0 END +
              CASE WHEN MA.IsDinner=1 THEN 1 ELSE 0 END
          ),
          TotalGuestMeal = SUM(
              ISNULL(MA.GuestBreakfastQty,0) +
              ISNULL(MA.GuestLunchQty,0) +
              ISNULL(MA.GuestDinnerQty,0)
          )
      FROM MealAttendances MA
      WHERE MA.MealDate BETWEEN @StartDate AND @EndDate
        AND MA.IsDelete = 0
      GROUP BY MA.MemberId
  )
  
  INSERT INTO MealBill
  (
      MemberId, TotalBazar, TotalMemberMeal, TotalMeal, TotalGuestMeal, 
      MealRate, MealAmount, HouseBill, UtilityBill, OtherBill, TotalPayable, 
      MealCycleId, CreatedDate, CreatedBy, IsDelete,CurrentBill,GasBill,ServantBill
  )
  SELECT
      MM.MemberId,
      @TotalBazarsAmount,
      MM.TotalMemberMeal,
      MM.TotalMemberMeal + MM.TotalGuestMeal,
      MM.TotalGuestMeal,
      @MealRate,
      (MM.TotalMemberMeal + MM.TotalGuestMeal) * @MealRate,
      ISNULL(M.HouseBill,0),
      ISNULL(M.UtilityBill,0),
      ISNULL(M.OtherBill,0),
      (MM.TotalMemberMeal + MM.TotalGuestMeal) * @MealRate + ISNULL(M.HouseBill,0) + ISNULL(M.UtilityBill,0) + ISNULL(M.OtherBill,0)+@PerMemberCurrentBill+@PerMemberGasBill+@PerMemberServantBill,
      @MealCycleId,
      SYSDATETIMEOFFSET(),
      @CreatedBy,
      0,
	  @PerMemberCurrentBill,
	  @PerMemberGasBill,
	  @PerMemberServantBill

  FROM MemberMeals MM
  INNER JOIN Member M ON M.Id = MM.MemberId
  WHERE M.MealStatus = 1 AND M.IsDelete = 0

  UNION ALL

-- 🔹 Non-Meal Members
SELECT
    M.Id,
    0,0,0,0,0,0,
    ISNULL(M.HouseBill,0),
    ISNULL(M.UtilityBill,0),
    ISNULL(M.OtherBill,0),
    ISNULL(M.HouseBill,0) + ISNULL(M.UtilityBill,0) + ISNULL(M.OtherBill,0)+@PerMemberCurrentBill,
    @MealCycleId,
    SYSDATETIMEOFFSET(),
    @CreatedBy,
    0,
	@PerMemberCurrentBill,
	 0,
	 0
FROM Member M
WHERE M.MealStatus = 0 AND M.IsDelete = 0";

            int affectedRows = await _context.Database.ExecuteSqlRawAsync(
                sql,
                new SqlParameter("@MealCycleIdParam", mealCycleId),
                new SqlParameter("@CreatedByParam", createdBy)
            );

            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task<List<MealBill>> GetMealBillsWithMemberAsync(long mealCycleId, long? memberId)
    {
        return await _context.Set<MealBill>()
        .Include(mb => mb.Member)
        .Include(mb => mb.MealCycle)
        .Where(mb =>
            mb.MealCycleId == mealCycleId &&
            !mb.IsDelete &&
            (memberId == 0 || mb.MemberId == memberId))
        .ToListAsync();
    }

}
