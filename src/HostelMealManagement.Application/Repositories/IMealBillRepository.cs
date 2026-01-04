using HostelMealManagement.Application.Repositories.Base;
using HostelMealManagement.Core.Entities;
using HostelMealManagement.Infrastructure.DatabaseContext;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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
            var sql = @"DECLARE
      @MealCycleId BIGINT = @MealCycleIdParam,
      @StartDate DATETIMEOFFSET,
      @EndDate DATETIMEOFFSET,
      @TotalBazarsAmount DECIMAL(18,2),
      @TotalMeals INT,
      @TotalGuestMeals INT,
      @GrandTotalMeals INT,
      @MealRate DECIMAL(18,2),
      @CreatedBy BIGINT = @MealCycleIdParam,
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
  ---update data
  DELETE FROM MealBill
  WHERE MealCycleId = @MealCycleId;   

  --temp table
  
 IF OBJECT_ID('tempdb..#Members') IS NOT NULL DROP TABLE #Members;

 --insert data temp table

SELECT 
    ROW_NUMBER() OVER (ORDER BY M.Id) AS RowNum,
    M.Id AS MemberId,
    M.MealStatus,
    ISNULL(M.HouseBill,0) AS HouseBill,
    ISNULL(M.UtilityBill,0) AS UtilityBill,
    ISNULL(M.OtherBill,0) AS OtherBill
INTO #Members
FROM Member M
WHERE M.IsDelete = 0;

DECLARE 
    @i INT = 1,
    @MaxRow INT,
    @MemberId BIGINT,
    @MealStatus BIT,
    @HouseBill  DECIMAL(18,2),
    @UtilityBill  DECIMAL(18,2),
    @OtherBill  DECIMAL(18,2),
    @TotalMemberMeal INT,
    @TotalGuestMeal INT,
	@TotalPaymentAmount  DECIMAL(18,2),
	@PaidStatus VARCHAR(20),
    @TotalPayable DECIMAL(18,2),
	@NetPayable DECIMAL(18,2);

SELECT @MaxRow = MAX(RowNum) FROM #Members;

WHILE @i <= @MaxRow
BEGIN
    SELECT 
        @MemberId = MemberId,
        @MealStatus = MealStatus,
        @HouseBill = HouseBill,
        @UtilityBill = UtilityBill,
        @OtherBill = OtherBill
		
    FROM #Members
    WHERE RowNum = @i;

    SET @TotalMemberMeal = 0;
    SET @TotalGuestMeal = 0;


	SELECT @TotalPaymentAmount=ISNULL(SUM(PaymentAmount),0) FROM NormalPayments WHERE MemberId=@MemberId AND MealCycleId=@MealCycleId 

    IF @MealStatus = 1
    BEGIN
        SELECT
            @TotalMemberMeal = SUM(
                CASE WHEN IsBreakfast = 1 THEN 1 ELSE 0 END +
                CASE WHEN IsLunch = 1 THEN 1 ELSE 0 END +
                CASE WHEN IsDinner = 1 THEN 1 ELSE 0 END
            ),
            @TotalGuestMeal = SUM(
                ISNULL(GuestBreakfastQty,0) +
                ISNULL(GuestLunchQty,0) +
                ISNULL(GuestDinnerQty,0)
            )
        FROM MealAttendances
        WHERE MemberId = @MemberId
          AND MealDate BETWEEN @StartDate AND @EndDate
          AND IsDelete = 0;

		  SET @TotalPayable =
(
    (ISNULL(@TotalMemberMeal,0) + ISNULL(@TotalGuestMeal,0)) * @MealRate
    + ISNULL(@HouseBill,0)
    + ISNULL(@UtilityBill,0)
    + ISNULL(@OtherBill,0)
    + ISNULL(@PerMemberCurrentBill,0)
    + ISNULL(@PerMemberGasBill,0)
    + ISNULL(@PerMemberServantBill,0)
);


SET @NetPayable =
CASE 
    WHEN @TotalPayable - ISNULL(@TotalPaymentAmount,0) < 0 THEN 0
    ELSE @TotalPayable - ISNULL(@TotalPaymentAmount,0)
END;



SET @PaidStatus =
CASE
    WHEN @NetPayable = 0 THEN 'Paid'
    WHEN ISNULL(@TotalPaymentAmount,0) > 0 THEN 'Partial'
    ELSE 'Unpaid'
END;




        INSERT INTO MealBill
        (
            MemberId, TotalBazar, TotalMemberMeal, TotalMeal, TotalGuestMeal,
            MealRate, MealAmount,
            HouseBill, UtilityBill, OtherBill,
            TotalPayable,
            MealCycleId, CreatedDate, CreatedBy, IsDelete,
            CurrentBill, GasBill, ServantBill,TotalPaidAmount,NetPayable,PaidStatus
        )
        VALUES
        (
            @MemberId,
            @TotalBazarsAmount,
            ISNULL(@TotalMemberMeal,0),
            ISNULL(@TotalMemberMeal,0) + ISNULL(@TotalGuestMeal,0),
            ISNULL(@TotalGuestMeal,0),
            @MealRate,
            (ISNULL(@TotalMemberMeal,0) + ISNULL(@TotalGuestMeal,0)) * @MealRate,
            @HouseBill,
            @UtilityBill,
            @OtherBill,
            (ISNULL(@TotalMemberMeal,0) + ISNULL(@TotalGuestMeal,0)) * @MealRate
              + @HouseBill + @UtilityBill + @OtherBill
              + @PerMemberCurrentBill + @PerMemberGasBill + @PerMemberServantBill,
            @MealCycleId,
            SYSDATETIMEOFFSET(),
            @CreatedBy,
            0,
            @PerMemberCurrentBill,
            @PerMemberGasBill,
            @PerMemberServantBill,
			@TotalPaymentAmount,
			@NetPayable,
			@PaidStatus

        );
    END
    ELSE
    BEGIN

    	  SET @TotalPayable =
(
    
    ISNULL(@HouseBill,0)
    + ISNULL(@UtilityBill,0)
    + ISNULL(@OtherBill,0)
    + ISNULL(@PerMemberCurrentBill,0)
);


SET @NetPayable =
CASE 
    WHEN @TotalPayable - ISNULL(@TotalPaymentAmount,0) < 0 THEN 0
    ELSE @TotalPayable - ISNULL(@TotalPaymentAmount,0)
END;



SET @PaidStatus =
CASE
    WHEN @NetPayable = 0 THEN 'Paid'
    WHEN ISNULL(@TotalPaymentAmount,0) > 0 THEN 'Partial'
    ELSE 'Unpaid'
END;

        INSERT INTO MealBill
        (
            MemberId, TotalBazar, TotalMemberMeal, TotalMeal, TotalGuestMeal,
            MealRate, MealAmount,
            HouseBill, UtilityBill, OtherBill,
            TotalPayable,
            MealCycleId, CreatedDate, CreatedBy, IsDelete,
            CurrentBill, GasBill, ServantBill,TotalPaidAmount,NetPayable,PaidStatus
        )
        VALUES
        (
            @MemberId,
            0,0,0,0,0,0,
            @HouseBill,
            @UtilityBill,
            @OtherBill,
            @HouseBill + @UtilityBill + @OtherBill + @PerMemberCurrentBill,
            @MealCycleId,
            SYSDATETIMEOFFSET(),
            @CreatedBy,
            0,
            @PerMemberCurrentBill,
            0,
            0,
			@TotalPaymentAmount,
			ISNULL(@NetPayable,0),
			@PaidStatus
        );
    END

    SET @i = @i + 1;
END;
DROP TABLE #Members;

";

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
        try
        {
            var data= await _context.Set<MealBill>()
        .Include(mb => mb.Member)
        .Include(mb => mb.MealCycle)
        .Where(mb =>
            mb.MealCycleId == mealCycleId &&
            !mb.IsDelete &&
            (memberId == 0 || mb.MemberId == memberId))
        .ToListAsync();
            return data;
        }
        catch (Exception ex)
        {

            throw;
        }
        
    }

}
