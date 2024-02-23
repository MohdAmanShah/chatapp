SELECT is_identity 
FROM sys.columns 
WHERE object_id = OBJECT_ID('users') 
AND name = 'Id';