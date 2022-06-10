CREATE PROCEDURE `SP_GetConsultantInfo`(in cuserid varchar(50))
begin
	SELECT
		cu.co_usuario as UserId
	    , cu.no_usuario as Name
	FROM
		cao_usuario cu
	where 
		cu.co_usuario = cuserid;
END;
