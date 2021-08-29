USE [TPJ PROD] 
GO
/****** Object:  StoredProcedure [kiosk].[CreateOrUpdateLeaveRequest]    Script Date: 08/28/21 2:18:30 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [kiosk].[CreateOrUpdateLeaveRequest](
	@ID BIGINT OUT
	, @PersonnelID BIGINT
	, @LeaveTypeID TINYINT
	, @RequestedDate DATE
	, @NoofDays FLOAT
	, @Reasons nvarchar(2000)
	, @LogBy INT)
AS
BEGIN
	IF @ID IS NULL OR @ID = 0
	BEGIN
		INSERT INTO kiosk.[Leave Request](
			[Personnel ID], 
			[Leave Type ID], 
			[Requested Date],
			[No of Days],
			[Reasons], 
			[Created By], 
			[Created On]
		)
		VALUES(
			@PersonnelID, 
			@LeaveTypeID, 
			@RequestedDate,
			@NoofDays,
			@Reasons, 
			@LogBy, 
			GETDATE()
		);

		SET @ID = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE kiosk.[Leave Request]
		SET [Leave Type ID] = @LeaveTypeID 
			, [Requested Date] = @RequestedDate
			, [No of Days] = @NoofDays
			, [Reasons] = @Reasons
			, [Modified By] = @LogBy
			, [Modified On] = GETDATE()
		WHERE ID = @ID;
	END
END
