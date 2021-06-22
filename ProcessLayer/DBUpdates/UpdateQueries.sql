
USE [TPJ]
GO

/****** Object:  Table [kiosk].[Outer Port Request]    Script Date: 30/03/2020 2:41:10 pm ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [kiosk].[Outer Port Request](
             [ID] [bigint] IDENTITY(1,1) NOT NULL,
             [Personnel ID] [bigint] NULL,
             [Start Date] [date] NULL,
             [End Date] [date] NULL,
			 [Location ID] [tinyint] NULL,
             [Purpose] [nvarchar](2000) NULL,
             [Cancelled] [bit] NULL,
             [Cancelled By] [int] NULL,
             [Cancelled On] [datetime] NULL,
             [Cancellation Remarks] [nvarchar](2000) NULL,
             [Created By] [int] NULL,
             [Created On] [datetime] NULL,
             [Modified By] [int] NULL,
             [Modified On] [datetime] NULL,
CONSTRAINT [PK_Outer Port Request] PRIMARY KEY CLUSTERED 
(
             [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [kiosk].[Outer Port Request] ADD  CONSTRAINT [DF_Outer Port Request_Created On]  DEFAULT (getdate()) FOR [Created On]
GO

CREATE PROC kiosk.GetOuterPortRequest(
	@ID BIGINT = NULL
	, @PersonnelID BIGINT = NULL
)
AS
BEGIN
	SELECT *
	FROM kiosk.[Outer Port Request]
	WHERE (@ID IS NULL OR ID = @ID)
		AND (@PersonnelID IS NULL OR [Personnel ID] = @PersonnelID)
	ORDER BY [Created On] DESC
END
GO

CREATE PROC kiosk.CreateOrUpdateOuterPortRequest(
	@ID BIGINT OUT,
	@PersonnelID BIGINT,
	@StartDate DATE,
	@EndDate DATE,
	@Purpose NVARCHAR(200),
	@LocationID TINYINT,
	@LogBy INT
)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO kiosk.[Outer Port Request](
			[Personnel ID]
			, [Start Date]
			, [End Date]
			, Purpose
			, [Location ID]
			, [Created By])
		VALUES(
			@PersonnelID
			, @StartDate
			, @EndDate
			, @Purpose
			, @LocationID
			, @LogBy
		)
	END	
	ELSE
	BEGIN
		UPDATE kiosk.[Outer Port Request]
		SET [Start Date] = @StartDate
			, [End Date] = @EndDate
			, [Purpose] = @Purpose
			, [Location ID] = @LocationID
			, [Modified By] = @LogBy
			, [Modified On] = GETDATE()
		WHERE ID = @ID 
	END
END
GO

CREATE PROC [kiosk].[DeleteOuterPortRequest](@ID BIGINT)
AS
BEGIN
	IF EXISTS(SELECT ID FROM kiosk.[Outer Port Request] WHERE ID = @ID)
	BEGIN
		DELETE FROM kiosk.[Outer Port Request] WHERE ID = @ID
	END
	ELSE
	BEGIN
		RAISERROR('Request not found.', 16, 2)
	END
END
GO

CREATE PROC kiosk.CancelOuterPortRequest(
	@ID BIGINT
	, @CancellationRemarks NVARCHAR(200) = NULL
	, @LogBy INT
)
AS
BEGIN
		UPDATE kiosk.[Outer Port Request]
		SET Cancelled = 1
			, [Cancellation Remarks] = @CancellationRemarks
			, [Cancelled By] = @LogBy
			, [Cancelled On] = GETDATE()
		WHERE ID = @ID

END
GO

CREATE PROC [kiosk].[FilterOuterPortRequest](
	@PageCount INT OUT,
	@Personnel NVARCHAR(MAX) = NULL,
	@LocationID TINYINT = NULL,
	@IsCancelled BIT = 0,
	@StartDate DATE = NULL,
	@EndDate DATE = NULL,
	@PageNumber INT = NULL,
	@GridCount INT = NULL
)
AS
BEGIN
	DECLARE @sql NVARCHAR(3000) = null;

	IF @Personnel = ''
		SET @Personnel = NULL;

	IF @PageNumber IS NULL AND @GridCount IS NULL
	BEGIN
		RAISERROR('Some field is missing', 16, 2);
	END
	ELSE 
	BEGIN
		DECLARE @Sum INT = 0;
		IF @PageNumber > 1
			SET @Sum = (@PageNumber - 1) * @GridCount;
		
		DECLARE @Condition NVARCHAR(MAX);
		
		IF @LocationID IS NOT NULL AND @LocationID != 0
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition =  CONCAT(@Condition, ' [Outer Port Request].[Location ID] = ', @LocationID );
		END

		IF @Personnel IS NOT NULL
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition =  CONCAT(@Condition, ' CONCAT([Last Name], '', '', [First Name], '' '',  [Middle Name]) LIKE ''%', @Personnel, '%''' );
		END
		
		IF @IsCancelled = 1
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition =  CONCAT(@Condition, ' AND Cancelled = 1' );
		END
		

		IF @StartDate IS NOT NULL AND @EndDate IS NOT NULL
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition = CONCAT(@Condition
									, '([Start Date] BETWEEN '''
									, @StartDate
									, ''' AND '''
									, @EndDate
									, ''' OR [End Date] BETWEEN '''
									, @StartDate
									, ''' AND '''
									, @EndDate
									, ''')' );
		END
		ELSE IF @StartDate IS NOT NULL AND @EndDate IS NULL	
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition = CONCAT(@Condition
									, '([Start Date] >= '''
									, @StartDate
									, '''  OR [End Date] <= '''
									, @StartDate
									, ''')' );
		END
		ELSE IF @StartDate IS NULL AND @EndDate IS NOT NULL	
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition = CONCAT(@Condition
									, '([Start Date] <= '''
									, @EndDate
									, '''  OR [End Date] >= '''
									, @EndDate
									, ''')' );
		END
		
		IF @Condition IS NOT NULL
			SET @Condition = CONCAT('WHERE ', @Condition);

		SET @sql = CONCAT('SELECT @PageCount = COUNT([Outer Port Request].ID) 
							FROM kiosk.[Outer Port Request] 
								INNER JOIN hr.Personnel
									ON [Outer Port Request].[Personnel ID] = Personnel.ID
										AND Personnel.Deleted = 0 ', @Condition);

		exec sp_executesql @sql, N'@PageCount INT OUT', @PageCount OUT;

		SET @PageCount = CEILING(CONVERT(FLOAT, @PageCount) / CONVERT(FLOAT, @GridCount))
		
		SET @sql = CONCAT('SELECT [Outer Port Request].*
						FROM kiosk.[Outer Port Request] INNER JOIN hr.Personnel
						ON [Outer Port Request].[Personnel ID] = Personnel.ID
							AND Personnel.Deleted = 0 ', 
						@Condition, ' ORDER BY [Outer Port Request].[Start Date] DESC
						OFFSET ', @Sum,' ROWS
						FETCH NEXT ', @GridCount,' ROWS ONLY');
		EXEC(@sql);
	END
END
GO

CREATE PROC [kiosk].[GetApprovedOuterPortRequest](
	@PersonnelID BIGINT,
	@LocationID TINYINT,
	@StartDate DATE,
	@EndDate DATE
)
AS
BEGIN
	SELECT * 
	FROM kiosk.[Outer Port Request]
	WHERE [Personnel ID] = @PersonnelID
		AND [Location ID] = @LocationID
		AND (([Start Date] BETWEEN @StartDate AND @EndDate)
			OR ([End Date] BETWEEN @StartDate AND @EndDate))
		AND (Cancelled = 0 OR Cancelled IS NULL)
END
GO

ALTER TABLE lookup.Location
ADD [Hazard Rate] DECIMAL(10,2) DEFAULT(0)
GO

ALTER TABLE cnb.[Payroll Details]
ADD [Location ID] TINYINT NULL
	, [OT Rate] DECIMAL(10,2)
GO

ALTER TABLE cnb.[Payroll]
ADD [Hazard Pay] DECIMAL(16,2)
GO


ALTER PROC [cnb].[CreateOrUpdatePayroll](
		@ID BIGINT OUT,
		@PayrollPeriodId BIGINT,
		@PersonnelID BIGINT,
		@TotalDeductions DECIMAL(16,2),
		@Allowance DECIMAL(16,2),
		@Tax DECIMAL(16,2),
		@RegularOTPay DECIMAL(16,2),
		@SundayOTPay DECIMAL(16,2),
		@HolidayOTPay DECIMAL(16,2),
		@NightDifferentialPay DECIMAL(16,2),
		@NOofDays DECIMAL(15,3),
		@DailyRate DECIMAL(15,3),
		@OutstandingVale DECIMAL(16,2),
		@HazardPay DECIMAL(16,2),
		@LogBy INT
)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO cnb.Payroll(
			[Payroll Period Id],
			[Personnel ID],
			[Tax],
			[Total Deductions],
			[Allowance],
			[Regular OT Pay],
			[Sunday OT Pay],
			[Holiday OT Pay],
			[Night Differential Pay],
			[NO of Days],
			[Daily Rate],
			[Outstanding Vale],
			[Hazard Pay],
			[Created By])
		VALUES (
			@PayrollPeriodId,
			@PersonnelID,
			@Tax,
			@TotalDeductions,
			@Allowance,
			@RegularOTPay,
			@SundayOTPay,
			@HolidayOTPay,
			@NightDifferentialPay,
			@NOofDays,
			@DailyRate,
			@OutstandingVale,
			@HazardPay,
			@LogBy
		)
		SET @ID = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE cnb.Payroll
		SET [Tax] = @Tax,
			[Total Deductions] = @TotalDeductions,
			[Allowance] = @Allowance,
			[Regular OT Pay] = @RegularOTPay,
			[Sunday OT Pay] = @SundayOTPay,
			[Holiday OT Pay] = @HolidayOTPay,
			[Night Differential Pay] = @NightDifferentialPay,
			[NO of Days] = @NOofDays,
			[Daily Rate] = @DailyRate,
			[Outstanding Vale] = @OutstandingVale,
			[Hazard Pay] = @HazardPay,
			[Modified By] = @LogBy,
			[Modified On] = GETDATE()
		WHERE ID = @ID
	END
END
GO


ALTER PROC [cnb].[CreateOrUpdatePayrollDetails](
		@ID BIGINT OUT,
		@PayrollID BIGINT,
		@LoggedDate DATE,
		@TotalRegularMinutes SMALLINT,
		@TotalOTMinutes SMALLINT,
		@TotalLeaveMinutes SMALLINT,
		@TotalNightDiffMinutes SMALLINT,
		@OTRate DECIMAL(10,2),
		@LocationID TINYINT,
		@IsHolday BIT,
		@IsSunday BIT,
		@LogBy INT
)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO cnb.[Payroll Details]( 
			[Payroll ID],
			[Logged Date],
			[Total Regular Minutes],
			[Total OT Minutes],
			[Total Leave Minutes],
			[Total Night Diff Minutes],
			[OT Rate],
			[Location ID],
			[IsHoliday],
			[IsSunday],
			[Created By])
		VALUES (
			@PayrollID,
			@LoggedDate,
			@TotalRegularMinutes,
			@TotalOTMinutes,
			@TotalLeaveMinutes,
			@TotalNightDiffMinutes,
			@OTRate,
			@LocationID,
			@IsHolday,
			@IsSunday,
			@LogBy
		)
		SET @ID = SCOPE_IDENTITY();
	END
	--ELSE
	--BEGIN
	--	UPDATE cnb.[Payroll Details]
	--	SET [Logged Date] = @LoggedDate,
	--		[Total Regular Minutes] = @TotalRegularMinutes,
	--		[Total OT Minutes] = @TotalOTMinutes,
	--		[Total Leave Minutes] = @TotalLeaveMinutes,
	--		[Total Night Diff Minutes] = @TotalNightDiffMinutes,
	--		[IsHoliday] = @IsHolday,
	--		[IsSunday] = @IsSunday,
	--		[Modified By] = @LogBy,
	--		[Modified On] = GETDATE()
	--	WHERE ID = @ID
	--END
END
GO


ALTER TABLE cnb.[Payroll Details]
ADD [OT Allowance] DECIMAL(16,2)
	, [Allowance] DECIMAL(16,2)
GO


ALTER PROC [cnb].[CreateOrUpdatePayrollDetails](
		@ID BIGINT OUT,
		@PayrollID BIGINT,
		@LoggedDate DATE,
		@TotalRegularMinutes SMALLINT,
		@TotalOTMinutes SMALLINT,
		@TotalLeaveMinutes SMALLINT,
		@TotalNightDiffMinutes SMALLINT,
		@OTRate DECIMAL(10,2),
		@LocationID TINYINT,
		@IsHolday BIT,
		@IsSunday BIT,
		@Allowance DECIMAL(16,2),
		@OTAllowance DECIMAL(16,2),
		@LogBy INT
)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO cnb.[Payroll Details]( 
			[Payroll ID],
			[Logged Date],
			[Total Regular Minutes],
			[Total OT Minutes],
			[Total Leave Minutes],
			[Total Night Diff Minutes],
			[OT Rate],
			[Location ID],
			[IsHoliday],
			[IsSunday],
			[Created By])
		VALUES (
			@PayrollID,
			@LoggedDate,
			@TotalRegularMinutes,
			@TotalOTMinutes,
			@TotalLeaveMinutes,
			@TotalNightDiffMinutes,
			@OTRate,
			@LocationID,
			@IsHolday,
			@IsSunday,
			@LogBy
		)
		SET @ID = SCOPE_IDENTITY();
	END
	--ELSE
	--BEGIN
	--	UPDATE cnb.[Payroll Details]
	--	SET [Logged Date] = @LoggedDate,
	--		[Total Regular Minutes] = @TotalRegularMinutes,
	--		[Total OT Minutes] = @TotalOTMinutes,
	--		[Total Leave Minutes] = @TotalLeaveMinutes,
	--		[Total Night Diff Minutes] = @TotalNightDiffMinutes,
	--		[IsHoliday] = @IsHolday,
	--		[IsSunday] = @IsSunday,
	--		[Modified By] = @LogBy,
	--		[Modified On] = GETDATE()
	--	WHERE ID = @ID
	--END
END
GO