USE [TPJ PROD LATEST]
GO
/****** Object:  StoredProcedure [lookup].[FilterPersonnelCompensationToApprove]    Script Date: 07/10/21 2:47:31 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROC [lookup].[FilterPersonnelCompensationToApprove](
	@Filter NVARCHAR(100) = NULL,
	@PageCount INT OUT,
	@PageNumber INT = 1,
	@GridCount INT = 10
)
AS
BEGIN
	DECLARE @sql NVARCHAR(MAX)

	IF @PageNumber IS NULL AND @GridCount IS NULL
	BEGIN
		RAISERROR('Some field is missing', 16, 2);
	END
	ELSE 
	BEGIN
		DECLARE @Sum INT = 0;
		IF @PageNumber > 1
			SET @Sum = (@PageNumber - 1) * @GridCount;

		SELECT @PageCount = COUNT(pc.ID) 
		FROM hr.[Personnel Compensation] pc
			INNER JOIN hr.Personnel p
				ON pc.[Personnel ID] = p.ID
			LEFT JOIN lookup.Compensation c
				ON pc.[Compensation ID] = c.ID
		WHERE CONCAT([First Name], [Middle Name], [Last Name], [Address], Email, [Employee No]) LIKE CONCAT('%', @Filter , '%') 
			AND c.[Has Approval] = 1

		SET @PageCount = CEILING(CONVERT(FLOAT, @PageCount) / CONVERT(FLOAT, @GridCount))

		SELECT pc.*, dbo.ConstructFullName(p.[First Name], p.[Middle Name], p.[Last Name]) FullName
		FROM hr.[Personnel Compensation] pc
			INNER JOIN hr.Personnel p
				ON pc.[Personnel ID] = p.ID
			LEFT JOIN lookup.Compensation c
				ON pc.[Compensation ID] = c.ID
		WHERE CONCAT([First Name], [Middle Name], [Last Name], [Address], Email, [Employee No]) LIKE CONCAT('%', @Filter , '%') AND 
			c.[Has Approval] = 1
		ORDER BY [Personnel ID]
		OFFSET @Sum ROWS
		FETCH NEXT @GridCount 
		ROWS ONLY
	END
END
