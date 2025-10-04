using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrabajoProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocioController : ControllerBase
    {
        public IConfiguration _config;
        public string connexionString;


        public SocioController(IConfiguration config)
        {
            _config = config;
            connexionString = _config["ConnectionStrings:defaultconnection"];
        }

        private static SqlConnection OpenConn(string connectionString)
        {
            var cn = new SqlConnection(connectionString);
            cn.Open();
            return cn;
        }


        [HttpGet]
        [Route("getSocio")]
        public List<Socio> GetSocio()
        {
            const string query = "select SocioId, ClubId, Nombre, Apellido, FechaNacimiento,FechaAsociado, Dni, CantidadAsistencias from Socio";
            using var cn = OpenConn(connexionString);
            var cmd = new SqlCommand(query, cn);

            var rd = cmd.ExecuteReader();

            List<Socio> ls = new List<Socio>();

            while (rd.Read())
            {
                ls.Add(new Socio
                {
                    SocioId = rd.GetInt32(0),
                    ClubId = rd.GetInt32(1),
                    Nombre = rd.GetString(2),
                    Apellido = rd.GetString(3),
                    FechaNacimiento = rd.GetDateTime(4),
                    FechaAsociado = rd.GetDateTime(5),
                    Dni = rd.GetInt32(6),
                    CantidadAsistencias = rd.GetInt32(7)
                });

            }
            return ls;

        }


        [HttpGet("{socioId}")]
        public ActionResult<Socio> Get(int socioId)
        {
            const string query = "select SocioId, ClubId, Nombre, Apellido, FechaNacimiento, FechaAsociado, Dni, CantidadAsistencias from Socio where SocioId =@socioId";
            using var cn = OpenConn(connexionString);
            var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@socioId", System.Data.SqlDbType.Int).Value = socioId;

            var rd = cmd.ExecuteReader();
            Socio socio = new Socio();

            while (rd.Read())
            {
                socio.SocioId = rd.GetInt32(0);
                socio.ClubId = rd.GetInt32(1);
                socio.Nombre = rd.GetString(2);
                socio.Apellido = rd.GetString(3);
                socio.FechaNacimiento = rd.GetDateTime(4);
                socio.FechaAsociado = rd.GetDateTime(5);
                socio.Dni = rd.GetInt32(6);
                socio.CantidadAsistencias = rd.GetInt32(7);
            };

            if (socio.Nombre == null)
            {
                return NotFound(new ErrorResponse { ErrorCode = "404", Message = "SOCIOID NO CORRESPONDE A UN SOCIO" });
            }
            else
            {
                return socio;
            }
        }


        [HttpPost]
        [Route("CreateSocio")]
        public ActionResult<Socio> CreateSocio([FromBody] Socio socio) 
        {
            const string query = @"INSERT INTO [dbo].[Socio]
            ([ClubId]
            ,[Nombre]
            ,[Apellido]
            ,[FechaNacimiento]
            ,[FechaAsociado]
            ,[Dni]
            ,[CantidadAsistencias])
            VALUES
            (@clubid, @nombre, @apellido, @fechanacimiento, @fechaasociado, @dni, @cantidadasistencias);";

            try
            {
                using var cs = OpenConn(connexionString);
                var cmd = new SqlCommand(query, cs);
                cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = socio.ClubId;
                cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = socio.Nombre;
                cmd.Parameters.Add("@apellido", System.Data.SqlDbType.VarChar).Value = socio.Apellido;
                cmd.Parameters.Add("@fechanacimiento", System.Data.SqlDbType.DateTime).Value = socio.FechaNacimiento;
                cmd.Parameters.Add("@fechaasociado", System.Data.SqlDbType.DateTime).Value = socio.FechaAsociado;
                cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = socio.Dni;
                cmd.Parameters.Add("@cantidadasistencias", System.Data.SqlDbType.Int).Value = socio.CantidadAsistencias;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "201", Message = $"Se insertaron {rowsAffected} registro(s) correctamente." });
                    }
                    else
                    {
                        return BadRequest(new ErrorResponse { ErrorCode = "400", Message = "No se insertó el registro." });
                    }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }


        // PUT api/<SocioController>/5
        [HttpPut("{id}")]
        public IActionResult PutSocio(int id, [FromBody] Socio socio)
        {
            if (id != socio.SocioId)
            {
                return BadRequest(new ErrorResponse { ErrorCode = "400", Message = "El SocioId  no coincide con el ID de un socio." });
            }

            const string query = @"UPDATE [dbo].[Socio] SET
        [ClubId] = @clubid,
        [Nombre] = @nombre,
        [Apellido] = @apellido,
        [FechaNacimiento] = @fechanacimiento,
        [FechaAsociado] = @fechanaasociado,
        [Dni] = @dni,
        [CantidadAsistencias] = @cantidadasistencias
    WHERE [SocioId] = @socioid;";

            try
            {
                using (var cn = OpenConn(connexionString))
                {
                    var cmd = new SqlCommand(query, cn);
                    cmd.Parameters.Add("@socioid", System.Data.SqlDbType.Int).Value = socio.SocioId;
                    cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = socio.ClubId;
                    cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = socio.Nombre;
                    cmd.Parameters.Add("@apellido", System.Data.SqlDbType.VarChar).Value = socio.Apellido;
                    cmd.Parameters.Add("@fechanacimiento", System.Data.SqlDbType.DateTime).Value = socio.FechaNacimiento;
                    cmd.Parameters.Add("@fechanaasociado", System.Data.SqlDbType.DateTime).Value = socio.FechaAsociado;
                    cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = socio.Dni;
                    cmd.Parameters.Add("@cantidadasistencias", System.Data.SqlDbType.Int).Value = socio.CantidadAsistencias;


                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "200", Message = $"Se actualizó el Club con ID {id} correctamente." });
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { ErrorCode = "404", Message = $"No se encontró el Club con ID {id} para actualizar." });
                    }
                }


            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { ErrorCode = "500", Message = "Ocurrió un error inesperado al actualizar el Club." });
            }
        }


        [HttpDelete("{socioId}")]
        public IActionResult DeleteSocio(int socioId)
        {
            const string query = "DELETE FROM [dbo].[Socio] WHERE [SocioId] = @socioId";

            try
            {
                using (var cn = OpenConn(connexionString))
                {
                    var cmd = new SqlCommand(query, cn);

                    cmd.Parameters.Add("@socioId", System.Data.SqlDbType.Int).Value = socioId;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "200", Message = $"Se eliminó el Dirigente con ID {socioId} correctamente." });
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { ErrorCode = "404", Message = $"No se encontró el Dirigente con ID {socioId} para eliminar." });
                    }

                }

            }


            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse
                {
                    ErrorCode = "500",
                    Message = "Ocurrió un error inesperado al eliminar el Socio."
                });
            }
        }









    }
}


      