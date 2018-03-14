using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.IO;

namespace ServicioVentas_208
{
    public partial class ServicioVentas_208 : ServiceBase
    {
        Timer tmservicio = null;
        public ServicioVentas_208()
        {
            InitializeComponent();
            tmservicio = new Timer(600000);
            tmservicio.Elapsed += new ElapsedEventHandler(tmpServicio_Elapsed);
        }
        void tmpServicio_Elapsed(object sender, ElapsedEventArgs e)
        {
            /*string path = @"C:\log.txt";
            TextWriter tw = new StreamWriter(path, true);
            tw.WriteLine("A fecha de : " + DateTime.Now.ToString() + ", Intervalo: " + tmservicio.Interval.ToString());
            tw.Close();*/
            
            try
            {
                LeerVenta.ActualizaLogVentas("", "Inicio del Proceso.", "sin error", LeerVenta.proceso_log);
                LeerVenta.Actualiza208();
                LeerVenta.ActualizaLogVentas("", "Fin del Proceso.", "sin error", LeerVenta.proceso_log);
            }
            catch (Exception ex)
            {
                LeerVenta.ActualizaLogVentas("", "Error en Servicio.", ex.Message, LeerVenta.proceso_log);
                LeerVenta.ActualizaLogVentas("", "Error en el Proceso.", "con error", LeerVenta.proceso_log);
            }
        }
        protected override void OnStart(string[] args)
        {
            tmservicio.Start();
        }

        protected override void OnStop()
        {
            tmservicio.Stop();
        }
    }
}
