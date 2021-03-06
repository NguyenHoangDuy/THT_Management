

alter PROCEDURE [dbo].[p_Get_CategoryConfig]	
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
					SELECT DISTINCT '','' + b.GroupName
					FROM GroupCategory a
					JOIN GroupDocument b ON a.GroupID = b.GroupID
					WHERE a.CategoryID = cc.CategoryID
					FOR XML PATH('''')
					 ), 1, 1, ''''),'''') AS ListGroup
		
	from CategoryConfig cc
	) AS data 
	WHERE  1 = 1 
'
EXEC (@SQL + @WhereCondition)
END
