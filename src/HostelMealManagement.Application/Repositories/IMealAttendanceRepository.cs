using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Application.ViewModel;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace HostelMealManagement.Application.Repositories;

public interface IMealAttendanceRepository : IBaseService<MealAttendance>
{
    Task<bool> GenerateMealBillAsync(long mealCycleId, long createdBy);
}

public class MealAttendanceRepository : BaseService<MealAttendance>, IMealAttendanceRepository
{
    public MealAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
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
                            @CreatedBy BIGINT = @CreatedByParam;
                        
                        SELECT 
                            @StartDate = StartDate,
                            @EndDate   = EndDate
                        FROM MealCycle
                        WHERE Id = @MealCycleId;
                        
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
                            MealCycleId, CreatedDate, CreatedBy, IsDelete
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
                            (MM.TotalMemberMeal + MM.TotalGuestMeal) * @MealRate + ISNULL(M.HouseBill,0) + ISNULL(M.UtilityBill,0) + ISNULL(M.OtherBill,0),
                            @MealCycleId,
                            SYSDATETIMEOFFSET(),
                            @CreatedBy,
                            0
                        FROM MemberMeals MM
                        INNER JOIN Member M ON M.Id = MM.MemberId";

            int affectedRows = await _context.Database.ExecuteSqlRawAsync(
                sql,
                new SqlParameter("@MealCycleIdParam", mealCycleId),
                new SqlParameter("@CreatedByParam", createdBy)
            );

            return affectedRows > 0;
        }
        catch
        {
            throw;
        }
    }
}


