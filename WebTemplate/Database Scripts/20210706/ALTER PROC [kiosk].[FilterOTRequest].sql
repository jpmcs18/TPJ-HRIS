USE [TPJ PROD LATEST]
GO
/****** Object:  StoredProcedure [kiosk].[FilterOTRequest]    Script Date: 07/06/21 12:06:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [kiosk].[FilterOTRequest](
	@PageCount INT OUT,
	@Personnel NVARCHAR(MAX) = NULL,
	@IsExpired BIT = 0,
	@IsPending BIT = 0,
	@IsApproved BIT = 0,
	@IsCancelled BIT = 0,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL,
	@PageNumber INT = NULL,
	@GridCount INT = NULL,
	@Approver INT = NULL
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
		
		IF @Personnel IS NOT NULL
		BEGIN
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')
			SET @Condition =  CONCAT(@Condition, ' CONCAT([Last Name], '', '', [First Name], '' '',  [Middle Name]) LIKE ''%', @Personnel, '%''' );
		END
		
		IF @IsPending = 1 OR @IsCancelled = 1 OR @IsApproved = 1 OR @IsExpired = 1
		BEGIN			
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT(@Condition, ' AND ')

			SET @Condition =  CONCAT(@Condition, ' ( ' );
			DECLARE @xcon NVARCHAR(500);

			IF @IsPending = 1
				SET @xcon = '((Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0) AND CONVERT(INT, DATEDIFF(HOUR, [OT Request].[Created On], GETDATE())) < 48)'
			
			IF @IsCancelled = 1
			BEGIN
				IF @xcon IS NOT NULL
					SET @xcon = CONCAT(@xcon, ' OR ')
				SET @xcon =  CONCAT(@xcon, '(Cancelled = 1 AND (Approved IS NULL OR Approved = 0))' );
			END
			
			IF @IsApproved = 1
			BEGIN
				IF @xcon IS NOT NULL
					SET @xcon = CONCAT(@xcon, ' OR ')
				SET @xcon =  CONCAT(@xcon, '(Approved = 1 AND (Cancelled IS NULL OR Cancelled = 0))' );
			END

			IF @IsExpired = 1
			BEGIN
				IF @xcon IS NOT NULL
					SET @xcon = CONCAT(@xcon, ' OR ')
				SET @xcon =  CONCAT(@xcon, '(CONVERT(INT, DATEDIFF(HOUR, [OT Request].[Created On], GETDATE())) >= 48 AND (Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0))' );
			END

			SET @Condition =  CONCAT(@Condition, @xcon, ' ) ' );
		END

		IF @StartDate IS NOT NULL AND @EndDate IS NOT NULL
		BEGIN
			IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '[Request Date] BETWEEN '''
										, @StartDate
										, ''' AND '''
										, @EndDate 
										, '''');
			END
			ELSE IF @StartDate IS NOT NULL AND @EndDate IS NULL	
			BEGIN
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '[Request Date] >= '''
										, @StartDate
										, '''');
			END
			ELSE IF @StartDate IS NULL AND @EndDate IS NOT NULL	
			BEGIN
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '([Request Date] <= '''
										, @EndDate
										, '''' );
		END
		
		IF @Condition IS NOT NULL
			SET @Condition = CONCAT('WHERE ', @Condition);

		SET @sql = CONCAT('SELECT @PageCount = COUNT([OT Request].ID) 
							FROM kiosk.[OT Request] 
								INNER JOIN hr.Personnel
									ON [OT Request].[Personnel ID] = Personnel.ID
										AND Personnel.Deleted = 0 ', @Condition);

		exec sp_executesql @sql, N'@PageCount INT OUT', @PageCount OUT;

		SET @PageCount = CEILING(CONVERT(FLOAT, @PageCount) / CONVERT(FLOAT, @GridCount))
		
		SET @sql = CONCAT('SELECT [OT Request].*
						FROM kiosk.[OT Request] INNER JOIN hr.Personnel
						ON [OT Request].[Personnel ID] = Personnel.ID
							AND Personnel.Deleted = 0 ', 
						@Condition, ' ORDER BY [OT Request].[Modified On] DESC
						OFFSET ', @Sum,' ROWS
						FETCH NEXT ', @GridCount,' ROWS ONLY');
		EXEC(@sql);
	END
END
