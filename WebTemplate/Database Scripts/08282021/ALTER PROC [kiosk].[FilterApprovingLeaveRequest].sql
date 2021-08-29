USE [TPJ PROD] 
GO
/****** Object:  StoredProcedure [kiosk].[FilterApprovingLeaveRequest]    Script Date: 08/28/21 3:48:08 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [kiosk].[FilterApprovingLeaveRequest](
	@PageCount INT OUT,
	@Personnel NVARCHAR(MAX) = NULL,
	@LeaveTypeID TINYINT = NULL,
	@Approver INT = NULL,
	@IsExpired BIT = 0,
	@IsPending BIT = 0,
	@IsApproved BIT = 0,
	@IsCancelled BIT = 0,
	@StartDate DATE = NULL,
	@EndDate DATE = NULL,
	@PageNumber INT = NULL,
	@GridCount INT = NULL
)
AS
BEGIN
	DECLARE @PersonnelID BIGINT = hr.GetPersonnelIDByUserID(@Approver);
	IF @PersonnelID = '' or @PersonnelID IS NULL
		RAISERROR('This approver is not connected to any personnel', 16, 2);
	ELSE
	BEGIN
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
		
			SELECT DISTINCT @PageCount = COUNT([Leave Request].ID) 
			FROM kiosk.[Leave Request]
				INNER JOIN hr.Personnel
					ON [Leave Request].[Personnel ID] = Personnel.ID
						AND Personnel.Deleted = 0 
				INNER JOIN lookup.[Kiosk Approvers]
					ON [Kiosk Approvers].[Approver ID] = @PersonnelID
						AND hr.GetPersonnelDepartmentOnSpecificDate([Leave Request].[Personnel ID], IIF(@EndDate IS NULL, GETDATE(), CAST(@EndDate AS DATE))) = [Kiosk Approvers].[Department ID]
						AND [Kiosk Approvers].Deleted = 0 
			WHERE (@Personnel IS NULL OR CONCAT([Last Name], [First Name], [Middle Name]) LIKE CONCAT('%', @Personnel, '%'))
				AND (@LeaveTypeID IS NULL OR [Leave Type ID] = @LeaveTypeID)
			AND ((@IsPending = 1 AND ((COALESCE(Approved, 0) = 0) AND (COALESCE(Cancelled, 0) = 0)))
				OR (@IsCancelled = 1 AND (COALESCE(Approved, 0) = 0))
				OR (@IsApproved = 1 AND (COALESCE(Cancelled, 0) = 0))
				OR (@IsExpired = 0 OR (CONVERT(INT, DATEDIFF(HOUR, [Leave Request].[Created On], GETDATE())) >= 48 AND (Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0))))
				AND (@StartDate IS NULL OR @EndDate IS NULL OR ([Requested Date] BETWEEN @StartDate AND @EndDate))

			SET @PageCount = CEILING(CONVERT(FLOAT, @PageCount) / CONVERT(FLOAT, @GridCount))

			SELECT DISTINCT [Leave Request].*
			FROM kiosk.[Leave Request]
				INNER JOIN hr.Personnel
					ON [Leave Request].[Personnel ID] = Personnel.ID
						AND Personnel.Deleted = 0 
				INNER JOIN lookup.[Kiosk Approvers]
					ON [Kiosk Approvers].[Approver ID] = @PersonnelID
						AND hr.GetPersonnelDepartmentOnSpecificDate([Leave Request].[Personnel ID], IIF(@EndDate IS NULL, GETDATE(), CAST(@EndDate AS DATE))) = [Kiosk Approvers].[Department ID]
						AND [Kiosk Approvers].Deleted = 0 
			WHERE (@Personnel IS NULL OR CONCAT([Last Name], [First Name], [Middle Name]) LIKE CONCAT('%', @Personnel, '%'))
				AND (@LeaveTypeID IS NULL OR [Leave Type ID] = @LeaveTypeID)
			AND ((@IsPending = 1 AND ((COALESCE(Approved, 0) = 0) AND (COALESCE(Cancelled, 0) = 0)))
				OR (@IsCancelled = 1 AND (COALESCE(Approved, 0) = 0))
				OR (@IsApproved = 1 AND (COALESCE(Cancelled, 0) = 0))
				OR (@IsExpired = 0 OR (CONVERT(INT, DATEDIFF(HOUR, [Leave Request].[Created On], GETDATE())) >= 48 AND (Approved IS NULL OR Approved = 0) AND (Cancelled IS NULL OR Cancelled = 0))))
				AND (@StartDate IS NULL OR @EndDate IS NULL OR ([Requested Date] BETWEEN @StartDate AND @EndDate))
			ORDER BY [Leave Request].[Requested Date] DESC
			OFFSET @Sum ROWS
			FETCH NEXT @GridCount 
			ROWS ONLY
		END
	END
END
