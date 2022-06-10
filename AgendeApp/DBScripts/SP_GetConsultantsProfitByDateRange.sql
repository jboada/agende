CREATE PROCEDURE `SP_GetConsultantsProfitByDateRange`(in cusers JSON, in initYear int, in initMonth int, in endYear int, in endMonth int)
begin
	select
		OS.co_usuario as UserId
		, cu.no_usuario as Name
	    , SUM(cf.total - (cf.total * (cf.total_imp_inc / 100))) as ReceitaLiquida
	FROM
		CAO_FATURA cf
		INNER JOIN cao_os OS ON OS.co_os = cf.co_os
		inner join cao_salario cs on cs.co_usuario  = OS.co_usuario  
		inner join cao_usuario cu on cu.co_usuario  = OS.co_usuario
	where 
		JSON_SEARCH (JSON_EXTRACT(cusers, "$.Consultants"), 'one', OS.co_usuario) is not null	
		and YEAR(cf.data_emissao) >= initYear
		and MONTH(cf.data_emissao) >= initMonth
		and YEAR(cf.data_emissao) <= endYear
		and MONTH(cf.data_emissao) <= endMonth
	group by 
		OS.co_usuario
	ORDER BY
		ReceitaLiquida;
END;
