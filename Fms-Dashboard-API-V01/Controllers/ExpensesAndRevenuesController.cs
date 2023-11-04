using System.Data;
using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;


namespace webapi.Controllers
{
    public class ExpensesRevenuesData
    {
        public int Debit { get; set; }
        public int Credit { get; internal set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ExpensesAndRevenuesController : Controller
    {
        private readonly string _connectionString = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))(CONNECT_DATA =(SERVER = DEDICATED)(SERVICE_NAME = orcl))); USER ID=FMS; PASSWORD=SECSEC44";
        //private readonly IConfiguration _configuration;

       
        [HttpGet]
        public async Task<IEnumerable<ExpensesRevenuesData>> Index()
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

                    command.CommandText = "FMS.EXPENSES_AND_REVENUES";
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("1", OracleDbType.Int64, 1, ParameterDirection.Input);
                    command.Parameters.Add("ref_cursor_param", OracleDbType.RefCursor, ref_cursor_param, ParameterDirection.Output);

                    await using (var reader = command.ExecuteReader())
                    {
                        //var data = new List<>();
                        //List<int> data = new List<int>();
                        var data = new List<ExpensesRevenuesData>();


                        while (reader.Read())
                        {
                            var model = new ExpensesRevenuesData
                            {
                                //CompId = Convert.ToInt32(reader["Comp_Id"]),
                                Debit = Convert.ToInt32(reader["debit"]),
                                Credit = Convert.ToInt32(reader["credit"])

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
