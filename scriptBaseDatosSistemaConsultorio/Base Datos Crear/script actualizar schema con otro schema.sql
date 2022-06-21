--SELECT 'ALTER SCHEMA dbo TRANSFER ' + s.Name + '.' + o.Name
--FROM sys.Objects o
--INNER JOIN sys.Schemas s on o.schema_id = s.schema_id
--WHERE s.Name = 'dbo'
--And (o.Type = 'U' Or o.Type = 'P' Or o.Type = 'V')




declare @sql varchar(8000), @table varchar(1000), @oldschema varchar(1000), @newschema varchar(1000)

set @oldschema = 'dbo'
set @newschema = 'sdoc'

while exists(select * from sys.tables where schema_name(schema_id) = @oldschema)
begin
select @table = name from sys.tables 
where object_id in(select min(object_id) from sys.tables where schema_name(schema_id) = @oldschema)

set @sql = 'alter schema ' + @newschema + ' transfer ' + @oldschema + '.' + @table

exec(@sql)
end


