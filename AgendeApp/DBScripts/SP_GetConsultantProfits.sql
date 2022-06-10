CREATE PROCEDURE `SP_GetConsultantProfits`(in cusers varchar(50), in cyear int, in cmonth int)
begin
	select
		OS.co_usuario as UserId
		, cu.no_usuario as Name
		, YEAR(cf.data_emissao) as OSYear
	    , MONTH(cf.data_emissao) as OSMonth
	    , monthname(str_to_date(MONTH(cf.data_emissao), '%m')) as OSMonthName 
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
		inner join cao_usuario cu on cu.co_usuario  = OS.co_usuario
	where 
		 OS.co_usuario = cusers
		 and YEAR(cf.data_emissao) = cyear
		 and MONTH(cf.data_emissao) = cmonth

	group by 
		OS.co_usuario
		, YEAR(cf.data_emissao)
	    , MONTH(cf.data_emissao)
	ORDER BY
		OSYear, OSMonth, OS.co_usuario;

END;
