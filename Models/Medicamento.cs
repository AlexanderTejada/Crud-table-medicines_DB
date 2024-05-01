using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crud.Models
{
    public class Medicamento
    {
        public int MedicamentoID { get; set; }  // identificador único y autoincrementado
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateTime FechaExpiracion { get; set; }
    }
}
