USE [TPJ]
GO
/****** Object:  StoredProcedure [vessel].[ApprovedVesselMovement]    Script Date: 04/06/22 4:52:06 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROC [vessel].[ApprovedVesselMovement](
	@ID BIGINT,
	@LogBy INT
)
AS
BEGIN
	UPDATE vessel.[Vessel Movement]
	SET [Movement Status ID] = 4,
		[Approved By] = @LogBy,
		[Approved Date] = GETDATE()
	WHERE ID = @ID
END
