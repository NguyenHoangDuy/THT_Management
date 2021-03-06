

alter PROCEDURE [dbo].[p_get_Document]		
	@Page int = 1,
	@PageSize int = 100,
	@WhereCondition nvarchar(MAX) = ''
AS
	DECLARE @Sql nvarchar(MAX), @Start int, @End int
	SET @Start = (@PageSize * @Page) - (@PageSize - 1)
	SET @End = @PageSize * @Page
	
	SET @Sql = 
	'
		;WITH FullSet AS
		(
			select * ,
			case when DATEDIFF(Y,DateSave,getdate())>TimeSave then ''TTHL2'' else ''TTHL1'' end as StatusSaved,
			case when DATEDIFF(d,ExpirationDate,getdate())>0 then ''TTHH2'' else ''TTHH1'' end as StatusExpirated
			from Document
	)
	 
	,FilteredSet AS
	(
		SELECT ROW_NUMBER() OVER (ORDER BY DocumentID) AS ''Number''
				,*
		FROM FullSet
		WHERE 1 = 1 '+ @WhereCondition +'
	)
	 
	,CountSet AS
	(
		SELECT COUNT(*) AS ''RowCount''
		FROM FilteredSet
	)
	 
	SELECT b.[RowCount], a.*
	FROM FilteredSet a CROSS APPLY CountSet b
	WHERE Number BETWEEN '+ CAST(@Start AS varchar(10)) +' AND '+ CAST(@End AS varchar(10)) +'
	'
	--print @sql
	EXEC (@Sql)