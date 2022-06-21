

create procedure usp_DropSPFunctionsViews
as
 
-- variable to object name
declare @name  varchar(1000)
-- variable to hold object type
declare @xtype varchar(20)
-- variable to hold sql string
declare @sqlstring nvarchar(4000)
 
declare SPViews_cursor cursor for
SELECT QUOTENAME(ROUTINE_SCHEMA) + '.' + QUOTENAME(ROUTINE_NAME) AS name, ROUTINE_TYPE AS xtype
FROM
INFORMATION_SCHEMA.ROUTINES
UNION
SELECT QUOTENAME(TABLE_SCHEMA) + '.' + QUOTENAME(TABLE_NAME) AS name, 'VIEW' AS xtype
FROM
INFORMATION_SCHEMA.VIEWS
 
open SPViews_cursor
 
fetch next from SPViews_cursor into @name, @xtype
 
while @@fetch_status = 0
  begin
-- test object type if it is a stored procedure
   if @xtype = 'PROCEDURE'
      begin
        set @sqlstring = 'drop procedure ' + @name
        exec sp_executesql @sqlstring
        set @sqlstring = ' '
      end
-- test object type if it is a function
   if @xtype = 'FUNCTION'
      begin
        set @sqlstring = 'drop FUNCTION ' + @name
        exec sp_executesql @sqlstring
        set @sqlstring = ' '
      end
-- test object type if it is a view
   if @xtype = 'VIEW'
      begin
         set @sqlstring = 'drop view ' + @name
         exec sp_executesql @sqlstring
         set @sqlstring = ' '
      end
 
-- get next record
    fetch next from SPViews_cursor into @name, @xtype
  end
 
close SPViews_cursor
deallocate SPViews_cursor
GO

--Drop all  views and tables
--https://blogs.msdn.microsoft.com/patrickgallucci/2008/04/29/how-to-drop-all-tables-all-views-and-all-stored-procedures-from-a-sql-2005-db/
execute usp_DropSPFunctionsViews