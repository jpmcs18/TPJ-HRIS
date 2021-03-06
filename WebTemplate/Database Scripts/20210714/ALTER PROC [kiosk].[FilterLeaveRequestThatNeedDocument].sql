USE [TPJ PROD LATEST]
GO
/****** Object:  StoredProcedure [kiosk].[FilterLeaveRequestThatNeedDocument]    Script Date: 07/14/21 1:32:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [kiosk].[FilterLeaveRequestThatNeedDocument] (
	@PageCount INT OUT,
	@Personnel NVARCHAR(MAX) = NULL,
	@LeaveTypeID TINYINT = NULL,
	@IsExpired BIT = 0,
	@IsPending BIT = 0,
	@IsApproved BIT = 0,
	@IsCancelled BIT = 0,
	@StartDateTime DATETIME = NULL,
	@EndDateTime DATETIME = NULL,
	@PageNumber INT = NULL,
	@GridCount INT = NULL
)
AS
BEGIN
	DECLARE @sql NVARCHAR(3000) = null;
		IF @Personnel = ''
			SET @Personnel = null
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

			IF @IsPending = 1  OR @IsCancelled = 1 OR @IsApproved = 1 OR @IsExpired = 1
			BEGIN
			
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')


				SET @Condition =  CONCAT(@Condition, ' ( ' );
				DECLARE @xcon NVARCHAR(500);
			
				IF @IsPending = 1
					SET @xcon = '((Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0) AND CONVERT(INT, DATEDIFF(HOUR, l.[Created On], GETDATE())) < 48)'
			
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
					SET @xcon =  CONCAT(@xcon, '(CONVERT(INT, DATEDIFF(HOUR, l.[Created On], GETDATE())) >= 48 AND (Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0))' );
				END

				SET @Condition =  CONCAT(@Condition, @xcon, ' ) ' );

			END

			IF @StartDateTime IS NOT NULL AND @EndDateTime IS NOT NULL
			BEGIN
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '([Start Date Time] BETWEEN '''
										, CAST(@StartDateTime AS DATE)
										, ''' AND '''
										, CAST(@EndDateTime AS DATE)
										, ''' OR [End Date Time] BETWEEN '''
										, CAST(@StartDateTime AS DATE)
										, ''' AND '''
										, CAST(@EndDateTime AS DATE)
										, ''')' );
			END
			ELSE IF @StartDateTime IS NOT NULL AND @EndDateTime IS NULL	
			BEGIN
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '([Start Date Time] >= '''
										, CAST(@StartDateTime AS DATE)
										, '''  OR [End Date Time] <= '''
										, CAST(@StartDateTime AS DATE)
										, ''')' );
			END
			ELSE IF @StartDateTime IS NULL AND @EndDateTime IS NOT NULL	
			BEGIN
				IF @Condition IS NOT NULL
					SET @Condition = CONCAT(@Condition, ' AND ')
				SET @Condition = CONCAT(@Condition
										, '([Start Date Time] <= '''
										, CAST(@EndDateTime AS DATE)
										, '''  OR [End Date Time] >= '''
										, CAST(@EndDateTime AS DATE)
										, ''')' );
			END
		
			IF @Condition IS NOT NULL
				SET @Condition = CONCAT('WHERE ', @Condition);

			SET @sql = CONCAT('SELECT DISTINCT @PageCount = COUNT(l.ID) 
								FROM kiosk.[Leave Request] l
									INNER JOIN hr.Personnel
										ON l.[Personnel ID] = Personnel.ID
											AND Personnel.Deleted = 0 
									INNER JOIN lookup.[Leave Type] lt
										ON l.[Leave Type ID] = lt.ID
											AND lt.[Has Document Needed] = 1 ', @Condition);

			exec sp_executesql @sql, N'@PageCount INT OUT', @PageCount OUT;

			SET @PageCount = CEILING(CONVERT(FLOAT, @PageCount) / CONVERT(FLOAT, @GridCount))

			 SET @sql = CONCAT('SELECT DISTINCT l.*
								FROM kiosk.[Leave Request] l 
									INNER JOIN hr.Personnel
										ON l.[Personnel ID] = Personnel.ID
											AND Personnel.Deleted = 0
									INNER JOIN lookup.[Leave Type] lt
										ON l.[Leave Type ID] = lt.ID
											AND lt.[Has Document Needed] = 1 '
								, @Condition
								, ' ORDER BY l.[Start Date Time] DESC
								OFFSET ', @Sum,' ROWS
								FETCH NEXT ', @GridCount,' ROWS ONLY');
			--SELECT @sql
			EXEC(@sql);
	END
END
