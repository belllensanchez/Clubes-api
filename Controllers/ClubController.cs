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
    public class ClubController : ControllerBase
    {
        public IConfiguration _config;
        public string connexionString;


        public ClubController (IConfiguration config)
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
        [Route("getClub")]
        public List<Club> getClub()
        {
            const string query= "select ClubId,Nombre,CantidadSocios,CantidadTitulos,FechaFundacion,UbicacionEstadio,NombreEstadio from club";
            using var cn = OpenConn(connexionString);
            var cmd = new SqlCommand(query, cn);

            var rd = cmd.ExecuteReader();

            List<Club>lc = new List<Club>();

            while (rd.Read()) {
                lc.Add(new Club
                {
                    ClubId = rd.GetInt32(0),
                    Nombre = rd.GetString(1),
                    CantidadSocios = rd.GetInt32(2),
                    CantidadTitulos = rd.GetInt32(3),
                    FechaFundacion = rd.GetDateTime(4),
                    UbicacionEstadio = rd.GetString(5),
                    NombreEstadio = rd.GetString(6)
                });
                
            }
            return lc;
        }


        // GET api/<ClubController>/5
    
        [HttpGet("{clubId}")]
        public ActionResult <Club> Get(int clubId)
        {
            const string query = "select ClubId,Nombre,CantidadSocios,CantidadTitulos,FechaFundacion,UbicacionEstadio,NombreEstadio from club where ClubId =@clubId";
            using var cn = OpenConn(connexionString);
            var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@clubId", System.Data.SqlDbType.Int).Value = clubId;

            var rd = cmd.ExecuteReader();
            Club club = new Club();

            while (rd.Read())
            {
                
                club.ClubId = rd.GetInt32(0);
                club.Nombre = rd.GetString(1);
                club.CantidadSocios = rd.GetInt32(2);
                club.CantidadTitulos = rd.GetInt32(3);
                club.FechaFundacion = rd.GetDateTime(4);
                club.UbicacionEstadio = rd.GetString(5);
                club.NombreEstadio = rd.GetString(6);
            };

            if (club.Nombre==null)
            {
                return NotFound( new ErrorResponse { ErrorCode="404", Message="CLUBID NO CORRESPONDE A UN CLUB"});
            }
            else
            {
                return  club;
            }
        }






        // POST api/<ClubController>
        [HttpPost]
        [Route("CreateClub")]
        public ActionResult<Club> club ([FromBody] Club club)
        {
            const string query = @"INSERT INTO [dbo].[Club]
            ([Nombre]
           ,[CantidadSocios]
           ,[CantidadTitulos]
           ,[FechaFundacion]
           ,[UbicacionEstadio]
           ,[NombreEstadio])
     VALUES
            (@nombre,@cantidadsocios,@cantidadtitulos,@fechafundacion,@ubicacionestadio,@nombreestadio);";

            try { 
            using var cn = OpenConn(connexionString);

                if (club.FechaFundacion > DateTime.Now)
                {
                    return BadRequest(new Responses
                    {
                        Code = "400",
                        Message = "La fecha de fundación no puede ser futura."
                    });
                }

                var cmd = new SqlCommand(query, cn);
            cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = club.Nombre;
            cmd.Parameters.Add("@cantidadsocios", System.Data.SqlDbType.Int).Value = club.CantidadSocios;
            cmd.Parameters.Add("@cantidadtitulos", System.Data.SqlDbType.Int).Value = club.CantidadTitulos;;
            cmd.Parameters.Add("@fechafundacion", System.Data.SqlDbType.DateTime).Value = club.FechaFundacion;
            cmd.Parameters.Add("@ubicacionestadio", System.Data.SqlDbType.VarChar).Value = club.UbicacionEstadio;
            cmd.Parameters.Add("@nombreestadio", System.Data.SqlDbType.VarChar).Value = club.NombreEstadio;

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)

                {
                    return Ok(new Responses { Code = "200", Message = $"Se insertaron {rowsAffected} registro(s) correctamente." });
                }
                else
                {
                    return StatusCode(500, new Responses { Code = "500", Message = "Ocurrió un error inesperado al procesar la solicitud." });
                }
            }
            catch (Exception ex)
            {
                

                // Return a generic, non-technical error to the client.
                return StatusCode(500, new { error = ex.Message });
            }


        }
        
        // PUT api/<ClubController>/5
        [HttpPut("{id}")]
        public IActionResult PutClub(int id, [FromBody] Club club)
        {
          
            if (id != club.ClubId)
            {
                return BadRequest(new Responses { Code = "400", Message = "El ID de la ruta no coincide con el ID del Club en el cuerpo de la petición." });
            }

        
            const string query = @"UPDATE [dbo].[Club] SET 
        [Nombre] = @nombre,
        [CantidadSocios] = @cantidadsocios,
        [CantidadTitulos] = @cantidadtitulos,
        [FechaFundacion] = @fechafundacion,
        [UbicacionEstadio] = @ubicacionestadio,
        [NombreEstadio] = @nombreestadio
    WHERE [ClubId] = @clubid;"; // Es crucial el WHERE para especificar qué fila cambiar

            // 3. Ejecutar la actualización
            try
            {
                using var cn = OpenConn(connexionString);

                if (club.FechaFundacion > DateTime.Now)
                {
                    return BadRequest(new Responses
                    {
                        Code = "400",
                        Message = "La fecha de fundación no puede ser futura."
                    });
                }

                var cmd = new SqlCommand(query, cn);

                // Se deben asignar TODOS los parámetros del objeto Club
                cmd.Parameters.Add("@clubid", System.Data.SqlDbType.Int).Value = club.ClubId;
                cmd.Parameters.Add("@nombre", System.Data.SqlDbType.VarChar).Value = club.Nombre;
                cmd.Parameters.Add("@cantidadsocios", System.Data.SqlDbType.Int).Value = club.CantidadSocios;
                cmd.Parameters.Add("@cantidadtitulos", System.Data.SqlDbType.Int).Value = club.CantidadTitulos;
                cmd.Parameters.Add("@fechafundacion", System.Data.SqlDbType.DateTime).Value = club.FechaFundacion;
                cmd.Parameters.Add("@ubicacionestadio", System.Data.SqlDbType.VarChar).Value = club.UbicacionEstadio;
                cmd.Parameters.Add("@nombreestadio", System.Data.SqlDbType.VarChar).Value = club.NombreEstadio;

                // Usar ExecuteNonQuery() para obtener las filas afectadas
                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    // Éxito: Se actualizó el club
                    return Ok(new Responses { Code = "200", Message = $"Se actualizó el Club con ID {id} correctamente." });
                }
                else
                {
                    // No se encontró el club o no hubo cambios
                    return NotFound(new Responses { Code = "404", Message = $"No se encontró el Club con ID {id} para actualizar." });
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores
                // Se recomienda loggear ex.Message para depuración.
                return StatusCode(500, new Responses { Code = "500", Message = "Ocurrió un error inesperado al actualizar el Club." });
            }
        }


        // DELETE api/<ClubController>/5

        [HttpDelete("{clubID}")]
        public IActionResult DeleteClub(int clubID)
        {
            const string query = "DELETE FROM [dbo].[Club] WHERE [ClubId] = @clubId";

            try
            {
                using (var cn = OpenConn(connexionString))
                {
                    var cmd = new SqlCommand(query, cn);

                    cmd.Parameters.Add("@clubId", System.Data.SqlDbType.Int).Value = clubID;

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return Ok(new Responses { Code = "200", Message = $"Se eliminó el Club con ID {clubID} correctamente." });
                    }
                    else
                    {
                        return NotFound(new ErrorResponse { ErrorCode = "404", Message = $"No se encontró el Club con ID {clubID} para eliminar." });
                    }

                }

            }


            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse { ErrorCode = "500", Message = "Ocurrió un error inesperado al eliminar el Club." });
            }
        }

    }
}
