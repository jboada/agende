using AgendeApp.Models.DB;
using AgendeApp.Models.GetPerformanceProfits;
using AgendeApp.Models.shared;
using AgendeApp.Services.DB.IMySQLContext;
using AgendeApp.Services.DB.Performance;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AgendeApp.Controllers
{
    public class PerformanceController : Controller
    {
        private MySQLContext mySQLContext;

        public PerformanceController(MySQLContext mySQLContext)
        {
            this.mySQLContext = mySQLContext;
        }

        [HttpGet("")]
        async public Task<IActionResult> Index()
        {
            ViewBag.Title = "Performance Per Consultants";

            BaseResult baseResult = new BaseResult();

            try
            {
                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                baseResult = await performanceRepository.GetGetConsultantsListAsync();
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_Index";
                baseResult.Message = ex.Message;
            }

            return View(baseResult);
        }

        [HttpGet("[controller]/GetConsultanstsListAsync")]
        public async Task<IActionResult> GetConsultanstsListAsync()
        {
            BaseResult baseResult = new BaseResult();

            try
            {

                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                BaseResult baseResultPerformance = await performanceRepository.GetGetConsultantsListAsync();

                if(baseResultPerformance.Result == false)
                {
                    baseResult.Message = baseResultPerformance.Message;
                    baseResult.Code = baseResultPerformance.Code;
                    return StatusCode(500, baseResult);
                }

                baseResult.Data = baseResultPerformance.Data;
                return StatusCode(200, baseResultPerformance);
            }
            catch(Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultanstsListAsync";
                baseResult.Message = ex.Message;
                return StatusCode(500, baseResult);
            }
            
        }

        [HttpPost("[controller]/GetConsultantsLucroAsync")]
        async public Task<IActionResult> GetConsultantsLucroAsync(string consultants)
        {

            BaseResult baseResult = new BaseResult();
            string[] ConsultantsForEvaluation = null;

            // Validate Input
            try
            {
                ConsultantsForEvaluation = JsonSerializer.Deserialize<string[]>(consultants);
            }
            catch(Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsLucro_InvalidParameter";
                baseResult.Message = "Invalid Parameter. It is not an Array. Data = \"" + consultants + "\"";
                return StatusCode(404, baseResult);
            }

            //Get the Data
            try
            {
                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                BaseResult baseResultPerformance = await performanceRepository.GetConsultantsLucroAsync(ConsultantsForEvaluation, 2007, 1, 2007, 12);

                if (baseResultPerformance.Result == false)
                {
                    baseResult.Code = baseResultPerformance.Code;
                    baseResult.Message = baseResultPerformance.Message;
                    return StatusCode(500, baseResult);
                }

                performanceRepository = null;

                return StatusCode(200, baseResultPerformance);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsLucroAsync";
                baseResult.Message = ex.Message;
                return StatusCode(500, baseResult);
            }
            
        }

        [HttpPost("[controller]/GetConsultantsProfitsAsync")]
        async public Task<IActionResult> GetConsultantsProfitsAsync(string consultants, int initYear, int initMonth, int endYear, int endMonth)
        {

            BaseResult baseResult = new BaseResult();
            string[] ConsultantsForEvaluation = null;

            // Validate Input
            try
            {
                ConsultantsForEvaluation = JsonSerializer.Deserialize<string[]>(consultants);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsLucro_InvalidParameter";
                baseResult.Message = "Invalid Parameter. It is not an Array. Data = \"" + consultants + "\"";
                return StatusCode(404, baseResult);
            }

            //Get the Data
            try
            {
                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                BaseResult baseResultProfits = await performanceRepository.GetConsultantsProfitsAsync(ConsultantsForEvaluation, initYear, initMonth, endYear, endMonth);

                if(baseResultProfits.Result == false)
                {
                    baseResult.Code = baseResultProfits.Code;
                    baseResult.Message = baseResultProfits.Message;
                    baseResult.Data = baseResultProfits.Data;
                    return StatusCode(500, baseResult);
                }

                BaseResult baseResultFixedCosts = await performanceRepository.GetConsultantsFixedCostsByRangeAsync(ConsultantsForEvaluation, initYear, initMonth, endYear, endMonth);

                if (baseResultFixedCosts.Result == false)
                {
                    baseResult.Code = baseResultFixedCosts.Code;
                    baseResult.Message = baseResultFixedCosts.Message;
                    baseResult.Data = baseResultFixedCosts.Data;
                    return StatusCode(500, baseResult);
                }

                ((GetConsultantProfitsResult)baseResultProfits.Data).AverageFixedCost = ((SP_GetConsultantsFixedCostsResult)baseResultFixedCosts.Data).AverageFixedCost;


                performanceRepository = null;

                return StatusCode(200, baseResultProfits);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsProfitsAsync";
                baseResult.Message = ex.Message;
                baseResult.Data = ex;
                return StatusCode(500, baseResult);
            }

        }

        [HttpPost("[controller]/GetConsultantsProfitsByRangeAsync")]
        async public Task<IActionResult> GetConsultantsProfitsByRangeAsync(string consultants, int initYear, int initMonth, int endYear, int endMonth)
        {

            BaseResult baseResult = new BaseResult();
            string[] ConsultantsForEvaluation = null;

            // Validate Input
            try
            {
                ConsultantsForEvaluation = JsonSerializer.Deserialize<string[]>(consultants);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsProfitsByRangeAsync_InvalidParameter";
                baseResult.Message = "Invalid Parameter. It is not an Array. Data = \"" + consultants + "\"";
                return StatusCode(404, baseResult);
            }

            //Get the Data
            try
            {
                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                BaseResult baseResultProfits = await performanceRepository.GetConsultantsProfitsByRangeAsync(ConsultantsForEvaluation, initYear, initMonth, endYear, endMonth);

                if (baseResultProfits.Result == false)
                {
                    baseResult.Code = baseResultProfits.Code;
                    baseResult.Message = baseResultProfits.Message;
                    baseResult.Data = baseResultProfits.Data;
                    return StatusCode(500, baseResult);
                }

                performanceRepository = null;

                return StatusCode(200, baseResultProfits);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsProfitsByRangeAsync";
                baseResult.Message = ex.Message;
                baseResult.Data = ex;
                return StatusCode(500, baseResult);
            }

        }

        [HttpPost("[controller]/GetConsultantsFixedCostsByRangeAsync")]
        async public Task<IActionResult> GetConsultantsFixedCostsByRangeAsync(string consultants, int initYear, int initMonth, int endYear, int endMonth)
        {

            BaseResult baseResult = new BaseResult();
            string[] ConsultantsForEvaluation = null;

            // Validate Input
            try
            {
                ConsultantsForEvaluation = JsonSerializer.Deserialize<string[]>(consultants);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsFixedCostsByRangeAsync_InvalidParameter";
                baseResult.Message = "Invalid Parameter. It is not an Array. Data = \"" + consultants + "\"";
                return StatusCode(404, baseResult);
            }

            //Get the Data
            try
            {
                PerformanceRepository performanceRepository = new PerformanceRepository(mySQLContext);
                BaseResult baseResultFixedCosts = await performanceRepository.GetConsultantsFixedCostsByRangeAsync(ConsultantsForEvaluation, initYear, initMonth, endYear, endMonth);

                if (baseResultFixedCosts.Result == false)
                {
                    baseResult.Code = baseResultFixedCosts.Code;
                    baseResult.Message = baseResultFixedCosts.Message;
                    baseResult.Data = baseResultFixedCosts.Data;
                    return StatusCode(500, baseResult);
                }

                performanceRepository = null;

                return StatusCode(200, baseResultFixedCosts);
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GetConsultantsFixedCostsByRangeAsync";
                baseResult.Message = ex.Message;
                baseResult.Data = ex;
                return StatusCode(500, baseResult);
            }

        }

    }
}
