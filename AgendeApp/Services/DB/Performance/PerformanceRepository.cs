using AgendeApp.Models.DB;
using AgendeApp.Models.shared;
using AgendeApp.Services.DB.IMySQLContext;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using AgendeApp.Models.GetPerformanceProfits;

namespace AgendeApp.Services.DB.Performance
{
    public class PerformanceRepository : IPerformanceRepository
    {
        private readonly MySQLContext mySQLContext;

        public PerformanceRepository(MySQLContext mySQLContext)
        {
            this.mySQLContext = mySQLContext;
        }

        async public Task<BaseResult> GetConsultantsLucroAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth)
        {
            BaseResult baseResult = new BaseResult();

            var InputParameter = new { Consultants = consultants };

            try
            {
                if(initYear > EndYear)
                {
                    baseResult.Code = "ERROR_GETTING_CONSULTANTSLIST_StartYear_Greater_Than_EndYear";
                    baseResult.Message = "Starting Year is greater than Ending Year";
                }

                using (MySqlConnection conn = mySQLContext.GetConnection())
                {

                    await conn.OpenAsync();

                    // SP parameter
                    var param = new DynamicParameters();
                    param.Add("cusers", JsonSerializer.Serialize(InputParameter));
                    param.Add("initYear", initYear);
                    param.Add("initMonth", initMonth);
                    param.Add("endYear", EndYear);
                    param.Add("endMonth", EndMonth);

                    // Call SP
                    List<SP_GetConsultantsLucroResult> tempResult = (await conn.QueryAsync<SP_GetConsultantsLucroResult>(
                            "SP_GetConsultantsLucro",
                            param,
                            commandType: System.Data.CommandType.StoredProcedure)).AsList<SP_GetConsultantsLucroResult>();


                    await conn.CloseAsync();

                    baseResult.Data = tempResult;
                }

                baseResult.Result = true;
                
                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsLucroAsync_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsLucroAsync_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }
        }

        #region Profits per Consultant

        async private Task<SP_GetConsultantsLucroResult> ExecuteProfitQuery(string UserID, int i, int j)
        {
            using (MySqlConnection conn = mySQLContext.GetConnection())
            {

                await conn.OpenAsync();

                // SP parameter
                var param = new DynamicParameters();
                param.Add("cusers", UserID);
                param.Add("cyear", i);
                param.Add("cmonth", j);

                // Call SP
                SP_GetConsultantsLucroResult userResult = await conn.QuerySingleOrDefaultAsync<SP_GetConsultantsLucroResult>(
                        "SP_GetConsultantProfits",
                        param,
                        commandType: System.Data.CommandType.StoredProcedure);

                await conn.CloseAsync();

                if (userResult == null)
                {
                    return 
                        new SP_GetConsultantsLucroResult
                        {
                            OSYear = i,
                            OSMonth = j,
                            OSMonthName = new DateTime(i, j, 1).ToString("MMM"),
                            UserId = UserID
                        };
                }
                else
                {
                    userResult.OSMonthName = new DateTime(i, j, 1).ToString("MMM");
                    return userResult;
                }
            }
        }

        async public Task<BaseResult> GetConsultantsProfitsAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth)
        {
            BaseResult baseResult = new BaseResult();

            try
            {
                if (initYear > EndYear)
                {
                    baseResult.Code = "ERROR_GETTING_CONSULTANTSLIST_StartYear_Greater_Than_EndYear";
                    baseResult.Message = "Starting Year is greater than Ending Year";
                }

                if(EndYear - initYear == 0)
                {
                    if(EndMonth - initMonth < 0)
                    {
                        baseResult.Code = "ERROR_GETTING_CONSULTANTSLIST_StartMonth_Greater_Than_EndMonth";
                        baseResult.Message = "Starting Month is greater than Ending Month";
                    }
                }

                //List<GetPerformancePerConsultantProfitsResult> tempResultUsers = new List<GetPerformancePerConsultantProfitsResult>();
                GetConsultantProfitsResult tempResultUsers = new GetConsultantProfitsResult();

                foreach (string UserID in consultants)
                {
                    BaseResult tempUserInfo = await GetConsultantInfoAsync(UserID);

                    if(tempUserInfo.Result == false)
                    {
                        tempUserInfo = null;
                        continue; // user does not exist, go to next user
                    }

                    GetPerformancePerConsultantProfitsResult tempResultPerUser = new GetPerformancePerConsultantProfitsResult();

                    tempResultPerUser.Name = ((SP_GetConsultantInfoResult)tempUserInfo.Data).Name;
                    tempResultPerUser.UserID = UserID;

                    for (int i = initYear; i <= EndYear; i++)
                    {
                        if (i == initYear)
                        {
                            if(EndYear - initYear > 0)
                            {
                                for (int j = initMonth; j <= 12; j++)
                                {
                                    SP_GetConsultantsLucroResult x = await ExecuteProfitQuery(UserID, i, j);
                                    tempResultPerUser.Profits.Add(x);
                                }
                                continue; //i
                            } 
                            else
                            {
                                for (int j = initMonth; j <= EndMonth; j++)
                                {
                                    SP_GetConsultantsLucroResult x = await ExecuteProfitQuery(UserID, i, j);
                                    tempResultPerUser.Profits.Add(x);
                                }
                                continue; //i
                            }
                        }

                        if (i == EndYear)
                        {
                            for (int j = 1; j <= EndMonth; j++)
                            {
                                SP_GetConsultantsLucroResult x = await ExecuteProfitQuery(UserID, i, j);
                                tempResultPerUser.Profits.Add(x);
                            }
                            continue; //i
                        }

                        for (int j = 1; j <= 12; j++)
                        {
                            SP_GetConsultantsLucroResult x = await ExecuteProfitQuery(UserID, i, j);
                            tempResultPerUser.Profits.Add(x);
                        }

                    }

                    tempResultUsers.Consultants.Add(tempResultPerUser);
                }

                baseResult.Data = tempResultUsers;
                baseResult.Result = true;

                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitsAsync_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitsAsync_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }
        }

        #endregion

        async public Task<BaseResult> GetConsultantsProfitsByRangeAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth)
        {
            BaseResult baseResult = new BaseResult();

            var InputParameter = new { Consultants = consultants };

            try
            {
                if (initYear > EndYear)
                {
                    baseResult.Code = "ERROR_GETTING_CONSULTANTSLIST_StartYear_Greater_Than_EndYear";
                    baseResult.Message = "Starting Year is greater than Ending Year";
                }

                using (MySqlConnection conn = mySQLContext.GetConnection())
                {

                    await conn.OpenAsync();

                    // SP parameter
                    var param = new DynamicParameters();
                    param.Add("cusers", JsonSerializer.Serialize(InputParameter));
                    param.Add("initYear", initYear);
                    param.Add("initMonth", initMonth);
                    param.Add("endYear", EndYear);
                    param.Add("endMonth", EndMonth);

                    // Call SP
                    List<SP_GetConsultantsProfitByDateRangeResult> tempResult = (await conn.QueryAsync<SP_GetConsultantsProfitByDateRangeResult>(
                            "SP_GetConsultantsProfitByDateRange",
                            param,
                            commandType: System.Data.CommandType.StoredProcedure)).AsList<SP_GetConsultantsProfitByDateRangeResult>();


                    await conn.CloseAsync();

                    baseResult.Data = tempResult;
                }

                baseResult.Result = true;

                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitByDateRange_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitByDateRange_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }
        }

        async public Task<BaseResult> GetGetConsultantsListAsync()
        {

            BaseResult baseResult = new BaseResult();

            try
            {
                using (MySqlConnection conn = mySQLContext.GetConnection())
                {

                    await conn.OpenAsync();

                    baseResult.Data = (await conn.QueryAsync<Consultants>(
                            "SP_GetConsultantsList",
                            null,
                            commandType: System.Data.CommandType.StoredProcedure)).AsList();

                    conn.Close();

                }
                baseResult.Result = true;

                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetGetConsultantsListAsync_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetGetConsultantsListAsync_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }

        }

        async public Task<BaseResult> GetConsultantInfoAsync(string UserID)
        {
            BaseResult baseResult = new BaseResult();

            try
            {
                using (MySqlConnection conn = mySQLContext.GetConnection())
                {

                    await conn.OpenAsync();

                    // SP parameter
                    var param = new DynamicParameters();
                    param.Add("cuserid", UserID);

                    SP_GetConsultantInfoResult UserInfo = await conn.QuerySingleOrDefaultAsync<SP_GetConsultantInfoResult>(
                            "SP_GetConsultantInfo",
                            param,
                            commandType: System.Data.CommandType.StoredProcedure);

                    await conn.CloseAsync();

                    if(UserInfo != null)
                    {
                        baseResult.Data = UserInfo;
                        baseResult.Result = true;
                    }

                }

                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetGetConsultantsListAsync_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetGetConsultantsListAsync_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }
        }

        async public Task<BaseResult> GetConsultantsFixedCostsByRangeAsync(string[] consultants, int initYear, int initMonth, int EndYear, int EndMonth)
        {
            BaseResult baseResult = new BaseResult();

            var InputParameter = new { Consultants = consultants };

            try
            {
                if (initYear > EndYear)
                {
                    baseResult.Code = "ERROR_GETTING_CONSULTANTSLIST_StartYear_Greater_Than_EndYear";
                    baseResult.Message = "Starting Year is greater than Ending Year";
                }

                using (MySqlConnection conn = mySQLContext.GetConnection())
                {

                    await conn.OpenAsync();

                    // SP parameter
                    var param = new DynamicParameters();
                    param.Add("cusers", JsonSerializer.Serialize(InputParameter));
                    param.Add("initYear", initYear);
                    param.Add("initMonth", initMonth);
                    param.Add("endYear", EndYear);
                    param.Add("endMonth", EndMonth);

                    // Call SP
                    SP_GetConsultantsFixedCostsResult tempResult = await conn.QueryFirstOrDefaultAsync<SP_GetConsultantsFixedCostsResult>(
                            "SP_GetConsultantsFixedCosts",
                            param,
                            commandType: System.Data.CommandType.StoredProcedure);


                    await conn.CloseAsync();

                    baseResult.Data = tempResult;
                }

                baseResult.Result = true;

                return baseResult;

            }
            catch (MySqlException mysqlEx)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitByDateRange_INFODB";
                baseResult.Data = mysqlEx;
                return baseResult;
            }
            catch (Exception ex)
            {
                baseResult.Code = "ERROR_GETTING_GetConsultantsProfitByDateRange_INFOEX";
                baseResult.Data = ex;
                return baseResult;
            }
        }
    }
}
