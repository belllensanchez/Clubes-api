using Microsoft.AspNetCore.Mvc;
using TrabajoProyecto.Models;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http.HttpResults;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TrabajoProyecto.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirigenteController : ControllerBase
    {
        public IConfiguration _config;
        public string connexionString;


        public DirigenteController(IConfiguration config)
        {
            _config = config;
            connexionString = _config["ConnectionStrings:defaultconnection"];
        }

        private SqlConnection OpenConn(string connectionString)
        {
            var cs = new SqlConnection(connectionString);
            cs.Open();
            return cs;
        }


        [HttpGet]
        [Route("getDirigente")]
        public List<Dirigente> getDirigente()
        {
            const string query = "select DirigenteId, ClubId, Nombre, Apellido, FechaNacimiento,  Rol, Dni from dirigente";
            using (var cn = OpenConn(connexionString))
            {

                 var cmd = new SqlCommand(query, cn);
                 var rd = cmd.ExecuteReader();
                 List<Dirigente> ld = new List<Dirigente>();

                while (rd.Read())
                {
                     ld.Add(new Dirigente
                     {

                         DirigenteId = rd.GetInt32(0),
                         ClubId = rd.GetInt32(1),
                         Nombre = rd.GetString(2),
                         Apellido = rd.GetString(3),
                         FechaNacimiento = rd.GetDateTime(4),
                         Rol = rd.GetString(5),
                         Dni = rd.GetInt32(6)
                     });

                }
                     return ld;
            }


        }


        // GET api/<DirigenteController>/5

        [HttpGet("{dirigenteId}")]
        public ActionResult<Dirigente> Get(int dirigenteId)
        {
            const string query = "select DirigenteId, ClubId,Nombre,Apellido,FechaNacimiento,Rol,Dni from dirigente where DirigenteId =@dirigenteId";
            using var cs = OpenConn(connexionString);
            var cmd = new SqlCommand(query, cs);
            cmd.Parameters.Add("@dirigenteId", System.Data.SqlDbType.Int).Value = dirigenteId;

            var rd = cmd.ExecuteReader();
            Dirigente dirigente = new Dirigente();

            while (rd.Read())
            {

                dirigente.DirigenteId = rd.GetInt32(0);
                dirigente.ClubId = rd.GetInt32(1);
                dirigente.Nombre = rd.GetString(2);
                dirigente.Apellido = rd.GetString(3);
                dirigente.FechaNacimiento = rd.GetDateTime(4);
                dirigente.Rol = rd.GetString(5);
                dirigente.Dni = rd.GetInt32(6);
            };

            if (dirigente.Nombre == null)
            {
                return NotFound(new ErrorResponse { ErrorCode = "404", Message = "CLUBID NO CORRESPONDE A UN DIRIGENTE" });
            }
            else
            {
                return dirigente;
            }
        }






        // POST api/<DirigenteController>
        [HttpPost]
        [Route("CreateDirigente")]
        public ActionResult<Dirigente> dirigente([FromBody] Dirigente dirigente)
        {
            const string query = @"INSERT INTO [dbo].[Dirigente]
            ([ClubId]
           ,[Nombre]
           ,[Apellido]
           ,[FechaNacimiento]
           ,[ROl]
           ,[Dni])
     VALUES
            (@clubid,@nombre,@apellido,@fechanacimiento,@rol,@dni);";

            try
            {
                using var cs = OpenConn(connexionString);

                if (string.IsNullOrWhiteSpace(dirigente.Nombre))
                {
                    return BadRequest(new Responses
                    {
                        Code = "400",
                        Message = "El nombre del dirigente no puede estar vacío."
                    });
                }

                if (dirigente.FechaNacimiento > DateTime.Now)
                {
                    return BadRequest(new Responses
                    {
                        Code = "400",
                        Message = "La fecha de nacimiento no puede ser futura."
                    });
                }

                if (dirigente.Dni <= 0)
                {
                    return BadRequest(new Responses
                    {
                        Code = "400",
                        Message = "El DNI debe ser un número válido mayor a cero."
                    });
                }

                var cmd = new SqlCommand(query, cs);
                cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = dirigente.ClubId;
                cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = dirigente.Nombre;
                cmd.Parameters.Add("@apellido", System.Data.SqlDbType.VarChar).Value = dirigente.Apellido;
                cmd.Parameters.Add("@fechanacimiento", System.Data.SqlDbType.DateTime).Value = dirigente.FechaNacimiento;
                cmd.Parameters.Add("@rol", System.Data.SqlDbType.VarChar).Value = dirigente.Rol;
                cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = dirigente.Dni;

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)

                {
                    return Ok(new Responses { Code = "200", Message = $"Se insertaron {rowsAffected} registro(s) correctamente." });
                }
                else
                {
                    return StatusCode(500, new ErrorResponse { ErrorCode = "500", Message = "Ocurrió un error inesperado al procesar la solicitud." });
                }
            }
            catch (Exception ex)
            {


               
                return StatusCode(500, new { error = ex.Message });
            }

        


        }

        // PUT api/<DirigenteController>/5
        [HttpPut("{id}")]
        public IActionResult PutDirigente(int id, [FromBody] Dirigente dirigente)
        {
            // 1. Verificar que el ID de la ruta coincida con el ID del objeto
            if (id != dirigente.DirigenteId)
            {
                return BadRequest(new ErrorResponse {  ErrorCode= "400", Message = "El ID de la ruta no coincide con el ID del Dirigente en el cuerpo de la petición." });
            }

            // 2. Definir la consulta SQL de ACTUALIZACIÓN
            const string query = @"UPDATE [dbo].[Dirigente] SET
        [ClubId] = @clubid,
        [Nombre] = @nombre,
        [Apellido] = @apellido,
        [FechaNacimiento] = @fechanacimiento,
        [Rol] = @rol,
        [Dni] = @dni
    WHERE [DirigenteId] = @dirigenteid;";

            try
            {
                using (var cn = OpenConn(connexionString))
                {

                    // Validación de fecha de nacimiento
                    if (dirigente.FechaNacimiento > DateTime.Now)
                    {
                        return BadRequest(new ErrorResponse
                        {
                            ErrorCode = "400",
                            Message = "La fecha de nacimiento no puede ser futura."
                        });
                    }

                // Validación de DNI
                if (dirigente.Dni <= 0)
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorCode = "400",
                        Message = "El DNI debe ser un número válido mayor a cero."
                    });
                }

                // Validación de campos obligatorios
                if (string.IsNullOrWhiteSpace(dirigente.Nombre) ||
                    string.IsNullOrWhiteSpace(dirigente.Apellido) ||
                    string.IsNullOrWhiteSpace(dirigente.Rol))
                {
                    return BadRequest(new ErrorResponse
                    {
                        ErrorCode = "400",
                        Message = "Nombre, apellido y rol son obligatorios."
                    });
                }
                
                    var cmd = new SqlCommand(query, cn);
                    cmd.Parameters.Add("@dirigenteid", System.Data.SqlDbType.Int).Value = dirigente.DirigenteId;
                    cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = dirigente.ClubId;
                    cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = dirigente.Nombre;
                    cmd.Parameters.Add("@apellido", System.Data.SqlDbType.VarChar).Value = dirigente.Apellido;
                    cmd.Parameters.Add("@fechanacimiento", System.Data.SqlDbType.DateTime).Value = dirigente.FechaNacimiento;
                    cmd.Parameters.Add("@rol", System.Data.SqlDbType.VarChar).Value = dirigente.Rol;
                    cmd.Parameters.Add("@dni", System.Data.SqlDbType.Int).Value = dirigente.Dni;

                    
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "200", Message = $"Se actualizó el Dirigente con ID {id} correctamente." });
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { ErrorCode = "404", Message = $"No se encontró el Dirigente con ID {id} para actualizar." });
                    }
                }

                    
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });

            }
        }



        // DELETE api/<DirigenteController>/5

        [HttpDelete("{dirigenteId}")]
        public IActionResult DeleteDirigente(int dirigenteId)
        {
            const string query = "DELETE FROM [dbo].[Dirigente] WHERE [DirigenteId] = @dirigenteId";

            try
            {
                using (var cn = OpenConn(connexionString))
                {
                    var cmd = new SqlCommand(query, cn);

                    cmd.Parameters.Add("@dirigenteId", System.Data.SqlDbType.Int).Value = dirigenteId;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "200", Message = $"Se eliminó el Dirigente con ID {dirigenteId} correctamente." });
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { ErrorCode = "404", Message = $"No se encontró el Dirigente con ID {dirigenteId} para eliminar." });
                    }

                }

            }    
                
            
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

    }
}

