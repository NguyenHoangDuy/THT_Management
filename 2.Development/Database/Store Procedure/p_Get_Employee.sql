IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[p_Get_Employee]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[p_Get_Employee]
GO

CREATE PROCEDURE [dbo].[p_Get_Employee]	
@Page int = 1,
@PageSize int = 100,
@WhereCondition nvarchar(MAX) = ''
AS
SET NOCOUNT ON
DECLARE @SQL nvarchar(max)
BEGIN
SET @SQL =
'
	select * from (SELECT
		  *
		,ISNULL(STUFF ((
					SELECT DISTINCT '','' + b.ten_cong_viec
					FROM Employee_Job a
					JOIN Jobs b ON a.ma_cong_viec = b.ma_cong_viec
					WHERE a.ma_nhan_vien = cc.ma_nhan_vien
					FOR XML PATH('''')
					 ), 1, 1, ''''),'''') AS ListJobs
		
	from Employee cc
	) AS data 
	WHERE  1 = 1 
'
print @SQL --+ @WhereCondition

EXEC (@SQL + @WhereCondition)
END
