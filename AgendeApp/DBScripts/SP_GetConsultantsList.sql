CREATE PROCEDURE `SP_GetConsultantsList`()
SELECT
	cu.co_usuario as UserId
    , cu.no_usuario as Name
FROM
	cao_usuario cu
    INNER JOIN permissao_sistema ps ON ps.co_usuario = cu.co_usuario
WHERE
	ps.co_sistema = 1
    AND ps.in_ativo = 'S'
    AND ps.co_tipo_usuario in (0,1,2);
