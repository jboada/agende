CREATE PROCEDURE `SP_GetConsultantsFixedCosts`(in cusers JSON, in initYear int, in initMonth int, in endYear int, in endMonth int)
begin
	select
	    avg(cs.brut_salario) as AverageFixedCost
	FROM
		CAO_FATURA cf
		INNER JOIN cao_os OS ON OS.co_os = cf.co_os
		inner join cao_salario cs on cs.co_usuario  = OS.co_usuario  
	where 
		JSON_SEARCH (JSON_EXTRACT(cusers, "$.Consultants"), 'one', OS.co_usuario) is not null		
		and YEAR(cf.data_emissao) >= initYear
		and MONTH(cf.data_emissao) >= initMonth
		and YEAR(cf.data_emissao) <= endYear
		and MONTH(cf.data_emissao) <= endMonth;
	
END;
