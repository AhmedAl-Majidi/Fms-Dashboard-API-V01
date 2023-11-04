using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace webapi.Controllers
{
    public class AssetsLiabilitiesData
    {
        public int Type { get; set; }
        public int Year { get; set; }
        public int Month { get; set; } 
        public int Amount { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceSheetController : Controller
    {
        private readonly string _connectionString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orcl))); USER ID=FMS; PASSWORD=SECSEC44";
        private readonly IConfiguration _configuration;

        //ExpensesAndRevenuesController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        [HttpGet]
        public async Task<IEnumerable<AssetsLiabilitiesData>> Index()
        {

            await using (var connection = new OracleConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = connection.CreateCommand())
                {
                    var ref_cursor_param = new OracleParameter("p_Cursor", OracleDbType.RefCursor)
                    {
                        Direction = ParameterDirection.Output
                    };

                    command.CommandText = "FMS.Balance_Sheet";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("1", OracleDbType.Int64, 1, ParameterDirection.Input);
                    command.Parameters.Add("ref_cursor_param", OracleDbType.RefCursor, ref_cursor_param, ParameterDirection.Output);

                    await using (var reader = command.ExecuteReader())
                    {
                        var data = new List<AssetsLiabilitiesData>();

                        while (reader.Read())
                        {
                            var model = new AssetsLiabilitiesData
                            {
                                
                                Type = Convert.ToInt32(reader["type"]),
                                Year = Convert.ToInt32(reader["year"]),
                                Month = Convert.ToInt32(reader["month"]),
                                Amount = Convert.ToInt32(reader["amount"]),

                            };
                            data.Add(model);
                        }
                        //return Ok("hello");
                        return data;
                    }
                }
            }
            //return Ok("ok");
        }
    }
}
