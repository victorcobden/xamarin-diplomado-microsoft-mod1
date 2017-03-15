using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EjercicioAlternativo.Models
{
    public class UbicacionEntity : TableEntity
    {
        public UbicacionEntity(string archivo, string pais)
        {
            this.PartitionKey = archivo;
            this.RowKey = pais;
        }
        public UbicacionEntity()
        {

        }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string Localidad { get; set; }
    }
}
