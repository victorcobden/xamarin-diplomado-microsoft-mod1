using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;
using EjercicioAlternativo.Models;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using XamarinDiplomado.Participants.Startup;

namespace EjercicioAlternativo
{
    [Activity(Label = "EjercicioAlternativo", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        
        ImageView ImagenDrop;
        string ArchivoLocal;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            Startup startup = new Startup("Victor Hugo Cobden Roque Acuña", "vhcobden@gmail.com", 1, 1);
            startup.Init();

            SetContentView(Resource.Layout.Main);
            Button btnImagen = FindViewById<Button>(Resource.Id.btnRealizar);
            ImagenDrop = FindViewById<ImageView>(Resource.Id.imagen);
            btnImagen.Click += ArchivoImagen;
        }

        async void ArchivoImagen(object sender, EventArgs e)
        {
            var ruta = await DescargaImagen();
            Android.Net.Uri rutaImagen = Android.Net.Uri.Parse(ruta);
            ImagenDrop.SetImageURI(rutaImagen);

            CloudStorageAccount cuentaAlmacenamiento =
                CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=victorxamarin;AccountKey=2BTQRyfnHpiT41fllEmEA6eQGEV0t3/CeR0LTie2EWPmvVjnW9FYcFp2fz+KNas2dBnVykmYJT7sRceT77cLyA==");
            CloudBlobClient clientBlob = cuentaAlmacenamiento.CreateCloudBlobClient();
            CloudBlobContainer contenedor = clientBlob.GetContainerReference("lab1");
            CloudBlockBlob recursoBlob = contenedor.GetBlockBlobReference(ArchivoLocal);
            await recursoBlob.UploadFromFileAsync(ruta);

            Toast.MakeText(this, "Guardado en Azure Storage Blob", ToastLength.Short).Show();

            CloudTableClient tableClient = cuentaAlmacenamiento.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("Ubicaciones");

            await table.CreateIfNotExistsAsync();

            UbicacionEntity ubica = new UbicacionEntity(ArchivoLocal, "Perú");
            ubica.Latitud = -12.067996;
            ubica.Longitud = -75.210198;
            ubica.Localidad = "Huancayo 3";

            TableOperation insertar = TableOperation.Insert(ubica);
            await table.ExecuteAsync(insertar);

            Toast.MakeText(this, "Guardado en Azure Storage Table NoSQL", ToastLength.Short).Show();
        }

        private async Task<string> DescargaImagen()
        {
            WebClient client = new WebClient();
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (o, certificate, chain, errors) => true;
            byte[] imageData = await client.DownloadDataTaskAsync("http://www.topnews.in/files/Microsoft-Logo_4.jpg");
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            ArchivoLocal = "foto3.jpg";
            string localPath = Path.Combine(documentPath, ArchivoLocal);
            File.WriteAllBytes(localPath, imageData);
            return localPath;
        }
    }
}

