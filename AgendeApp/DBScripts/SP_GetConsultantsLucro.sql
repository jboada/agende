CREATE PROCEDURE `SP_GetConsultantsLucro`(in cusers JSON, in initYear int, in initMonth int, in endYear int, in endMonth int)
begin
	select
		OS.co_usuario as UserId
		, YEAR(cf.data_emissao) as OSYear
	    , MONTH(cf.data_emissao) as OSMonth
	    , monthname(str_to_date(MONTH(cf.data_emissao), '%m')) as OSMonthName 
	    , CONCAT(	    	
	    monthname(str_to_date(MONTH(cf.data_emissao), '%m')) 
	    , " de "
	    , YEAR(cf.data_emissao)
	    ) as Periodo
		, SUM(cf.total) as VALOR
		, SUM(cf.total * (cf.total_imp_inc / 100)) as TotalTaxes
	    , SUM(cf.total - (cf.total * (cf.total_imp_inc / 100))) as ReceitaLiquida
	    , cs.brut_salario as Brutsalario
		, SUM((cf.total - (cf.total * (cf.total_imp_inc / 100))) * (cf.comissao_cn / 100)) as ConsultantComission
		, cs.brut_salario + SUM((cf.total - (cf.total * (cf.total_imp_inc / 100))) * (cf.comissao_cn / 100)) as ConsultantLucro
		, 
			(SUM(cf.total) - SUM(cf.total * (cf.total_imp_inc / 100)))
			-
			(cs.brut_salario + SUM((cf.total - (cf.total * (cf.total_imp_inc / 100))) * (cf.comissao_cn / 100)))
			as Lucro
	FROM
		CAO_FATURA cf
		INNER JOIN cao_os OS ON OS.co_os = cf.co_os
		inner join cao_salario cs on cs.co_usuario  = OS.co_usuario  
	where 
		JSON_SEARCH (JSON_EXTRACT(cusers, "$.Consultants"), 'one', OS.co_usuario) is not null
		-- JSON_SEARCH (cusers, 'one', OS.co_usuario) is not null
		
		and YEAR(cf.data_emissao) >= initYear
		and MONTH(cf.data_emissao) >= initMonth
		and YEAR(cf.data_emissao) <= endYear
		and MONTH(cf.data_emissao) <= endMonth
	group by 
		OS.co_usuario
		, YEAR(cf.data_emissao)
	    , MONTH(cf.data_emissao)
	ORDER BY
		OSYear, OSMonth, OS.co_usuario;
	
END;
