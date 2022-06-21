select * from INFORMATION_SCHEMA.TABLES
where TABLE_SCHEMA = 'sdoc'
--select * from sys.foreign_keys
--select  type,type_desc  from sys.objects o 
--Group by type,type_desc


--borrar todos los constraint  f  y pk
SELECT 'ALTER TABLE ' + '[' + s.[NAME] + '].[' + t.name + '] DROP CONSTRAINT ['+ c.name + '];'
FROM sys.objects c, sys.objects t, sys.schemas s
WHERE c.type IN ( 'F', 'PK')
 AND c.parent_object_id=t.object_id and t.type='U' AND t.SCHEMA_ID = s.schema_id
 and s.name ='sdoc' 
 --for xml path('')


 --Borrar Todos los unique
select 'drop index ['+s.name+'].['+o.name+'].['+i.name+'];'
  from sys.indexes i join sys.objects o on i.object_id=o.object_id join sys.schemas s on o.schema_id=s.schema_id
  where o.type<>'S' and is_primary_key<>1 and index_id>0 and s.name = 'sdoc'
--for xml path('')


-- --borrar todos los constraint  pk
--SELECT 'ALTER TABLE ' + '[' + s.[NAME] + '].[' + t.name + '] DROP CONSTRAINT ['+ c.name + '];'
--FROM sys.objects c, sys.objects t, sys.schemas s
--WHERE c.type IN ( 'F', 'PK')
-- AND c.parent_object_id=t.object_id and t.type='U' AND t.SCHEMA_ID = s.schema_id
-- and s.name = 'sdoc'
-- --for xml path('')

 



