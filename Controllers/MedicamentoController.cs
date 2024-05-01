using Crud.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;

namespace Crud.Controllers
{
    public class MedicamentoController : Controller
    {
        private static string conexion = ConfigurationManager.ConnectionStrings["cadena"].ToString(); // Permite acceder a la hoja de web config

        private static List<Medicamento> listaMedicamentos = new List<Medicamento>();
        // 

        // GET: Medicamento
        public ActionResult Inicio()
        {
           listaMedicamentos.Clear(); // Limpia la lista

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Medicamento", oconexion); // 
                cmd.CommandType = CommandType.Text;
                oconexion.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Medicamento nuevoMedicamento = new Medicamento(); 
                        nuevoMedicamento.MedicamentoID = Convert.ToInt32(dr["MedicamentoID"]); 
                        nuevoMedicamento.Nombre = dr["Nombre"].ToString();
                        nuevoMedicamento.Descripcion = dr["Descripcion"].ToString();
                        nuevoMedicamento.Precio = Convert.ToDecimal(dr["Precio"]);
                        nuevoMedicamento.Stock = Convert.ToInt32(dr["Stock"]);
                        nuevoMedicamento.FechaExpiracion = Convert.ToDateTime(dr["FechaExpiracion"]);

                        listaMedicamentos.Add(nuevoMedicamento);
                    }
                }
            }
            return View(listaMedicamentos); 
        }
        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Editar(int? idmedicamento)
        {
            if (idmedicamento == null)
                return RedirectToAction("Inicio", "Medicamento");

          Medicamento omedicamento = listaMedicamentos.Where(c => c.MedicamentoID == idmedicamento).FirstOrDefault();


            return View(omedicamento);
        }

        [HttpGet]
        public ActionResult Eliminar(int? idmedicamento)
        {
            if (idmedicamento == null)
                return RedirectToAction("Inicio", "Medicamento");

            Medicamento omedicamento = listaMedicamentos.Where(c => c.MedicamentoID == idmedicamento).FirstOrDefault();


            return View(omedicamento);
        }
        [HttpPost]
        public ActionResult Eliminar(string MedicamentoID)
        {

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_EliminarMedicamento", oconexion);
                cmd.Parameters.AddWithValue("@MedicamentoID", MedicamentoID);

                cmd.CommandType = CommandType.StoredProcedure;
                oconexion.Open();                  
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Inicio", "Medicamento");
        }




        [HttpPost]
        public ActionResult Registrar(Medicamento omedicamento)
        {

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_Registrar", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

      
                cmd.Parameters.Add("@Nombre", SqlDbType.VarChar).Value = omedicamento.Nombre;
                cmd.Parameters.Add("@Descripcion", SqlDbType.Text).Value = omedicamento.Descripcion;
                cmd.Parameters.Add("@Precio", SqlDbType.Decimal).Value = omedicamento.Precio;
                cmd.Parameters["@Precio"].Precision = 10;
                cmd.Parameters["@Precio"].Scale = 2;
                cmd.Parameters.Add("@Stock", SqlDbType.Int).Value = omedicamento.Stock;
                cmd.Parameters.Add("@FechaExpiracion", SqlDbType.Date).Value = omedicamento.FechaExpiracion;

                oconexion.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Inicio", "Medicamento");
        }


        [HttpPost]
        public ActionResult Editar(Medicamento omedicamento)
        {
            System.Diagnostics.Debug.WriteLine("Precio recibido: " + omedicamento.Precio);

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("sp_EditarMedicamento", oconexion);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@MedicamentoID", omedicamento.MedicamentoID);
                cmd.Parameters.AddWithValue("@Nombre", omedicamento.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", omedicamento.Descripcion);
                cmd.Parameters.AddWithValue("@Precio", omedicamento.Precio);
                cmd.Parameters.AddWithValue("@Stock", omedicamento.Stock);
                cmd.Parameters.AddWithValue("@FechaExpiracion", omedicamento.FechaExpiracion);

                oconexion.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Inicio", "Medicamento");
        }


        [HttpGet]
        public ActionResult Buscar(string nombre)
        {
            List<Medicamento> resultados = new List<Medicamento>();

            using (SqlConnection oconexion = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Medicamento WHERE Nombre LIKE @nombre", oconexion);
                cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");
                cmd.CommandType = CommandType.Text;
                oconexion.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Medicamento nuevoMedicamento = new Medicamento
                        {
                            MedicamentoID = Convert.ToInt32(dr["MedicamentoID"]),
                            Nombre = dr["Nombre"].ToString(),
                            Descripcion = dr["Descripcion"].ToString(),
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Stock = Convert.ToInt32(dr["Stock"]),
                            FechaExpiracion = Convert.ToDateTime(dr["FechaExpiracion"])
                        };

                        resultados.Add(nuevoMedicamento);
                    }
                }
            }

            return View("Inicio", resultados);
        }

    }
}
