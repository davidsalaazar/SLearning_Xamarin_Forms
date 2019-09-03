using GalaSoft.MvvmLight.Command;
using Plugin.FilePicker;
using Plugin.FilePicker.Abstractions;
using S_LEARNING.Servicios;
using S_LEARNING.util;
using SlearningWS;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S_LEARNING.vistamodelo
{
    public class ActividadVistaModelo : BaseVistaModelo
    {
        private string nombre;
        private string descripcion;
        private DateTime fechaInicio;
        private DateTime fechaFin;
        private DateTime horaInicio;
        private DateTime horaFin;
        private double valor;

        private bool proceso;
        private bool botonActivo;

        private bool resgistraActividad;
        private bool muestraActividad;

        private temaModelo tema;
        private actividadModelo actividad;

        private FileData archivo;
        private archivoModelo archivoM;
        private string nombreArchivo;

        private string rutaImagen;
        private bool imagenVisible;
        private bool cargarArchivo;

        public ActividadVistaModelo(temaModelo tema, actividadModelo actividad)
        {
            this.tema = tema;
            this.actividad = actividad;

            if (tema != null)
            {
                this.BotonActivo = true;
                this.ResgistraActividad = true;
                this.FechaInicio = DateTime.Today;
                this.FechaFin = DateTime.Today;
                this.HoraInicio = DateTime.Today;
                this.HoraFin = DateTime.Today;
                this.ResgistraActividad = true;
            }

            if (actividad != null)
            {
                this.MuestraActividad = true;
                this.Descripcion = actividad.descripcion;

                this.CargaArchivo();
            }
        }

        #region GET & SET
        public string Nombre
        {
            get { return this.nombre; }
            set { setValor(ref this.nombre, value); }
        }
        public string Descripcion
        {
            get { return this.descripcion; }
            set { setValor(ref this.descripcion, value); }
        }
        public DateTime FechaInicio
        {
            get { return this.fechaInicio; }
            set { setValor(ref this.fechaInicio, value); }
        }
        public DateTime FechaFin
        {
            get { return this.fechaFin; }
            set { setValor(ref this.fechaFin, value); }
        }
        public DateTime HoraInicio
        {
            get { return this.horaInicio; }
            set { setValor(ref this.horaInicio, value); }
        }
        public DateTime HoraFin
        {
            get { return this.horaFin; }
            set { setValor(ref this.horaFin, value); }
        }
        public double Valor
        {
            get { return this.valor; }
            set { setValor(ref this.valor, value); }
        }
        public bool Proceso
        {
            get { return this.proceso; }
            set { setValor(ref this.proceso, value); }
        }
        public bool BotonActivo
        {
            get { return this.botonActivo; }
            set { setValor(ref this.botonActivo, value); }
        }
        public bool ResgistraActividad
        {
            get { return this.resgistraActividad; }
            set { setValor(ref this.resgistraActividad, value); }
        }
        public bool MuestraActividad
        {
            get { return this.muestraActividad; }
            set { setValor(ref this.muestraActividad, value); }
        }
        public string NombreArchivo
        {
            get { return this.nombreArchivo; }
            set { setValor(ref this.nombreArchivo, value); }
        }
        public string RutaImagen
        {
            get { return this.rutaImagen; }
            set { setValor(ref this.rutaImagen, value); }
        }
        public bool ImagenVisible
        {
            get { return this.imagenVisible; }
            set { setValor(ref this.imagenVisible, value); }
        }
        public bool CargarArchivo
        {
            get { return this.cargarArchivo; }
            set { setValor(ref this.cargarArchivo, value); }
        }
        #endregion

        #region Eventos
        public ICommand actionRegistrarActividad
        {
            get { return new RelayCommand(RegistrarActividad); }
        }
        public ICommand actionCancelar
        {
            get { return new RelayCommand(() => limpiaPantalla()); }
        }
        public ICommand actionCargaArchivo
        {
            get { return new RelayCommand(BuscaArchivo); }
        }
        public ICommand actionGuardaArchivo
        {
            get { return new RelayCommand(GuardaArchivo); }
        }
        public ICommand actionCancelarArchivo
        {
            get
            {
                return new RelayCommand(() =>
                {
                    this.archivo = null;
                    this.NombreArchivo = string.Empty;
                    this.BotonActivo = false;
                });
            }
        }
        public ICommand actionDescargarArchivo
        {
            get { return new RelayCommand(descargarArchivo); }
        }
        #endregion

        #region Metodos
        public async void RegistrarActividad()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                Util.mensaje(1, "El nombre esta vacío");
                return;
            }

            this.Proceso = true;

            var conexion = await new APIServicio().verificaConexion();

            if (!conexion.IsSuccess)
            {
                this.Proceso = false;
                Util.mensaje(1, conexion.Mensaje);
                return;
            }

            var client = MainVistaModelo.getInstancia().getWS();
            client.Timeout = 10000;

            actividadModelo actividad = new actividadModelo
            {
                nombre = this.Nombre,
                descripcion = this.Descripcion,
                fechaInicio = this.FechaInicio.ToString("yyyy-MM-dd"),
                fechaFin = this.FechaFin.ToString("yyyy-MM-dd"),
                horaInicio = this.HoraInicio.ToString("hh:mm tt"),
                horaFin = this.HoraFin.ToString("hh:mm tt"),
                valor = this.Valor,
                tema = new temaModelo { idTema = tema.idTema }
            };

            bool res = false;

            await Task.Run(() =>
            {
                try
                {
                    res = client.agregaActividad(actividad);
                }
                catch (Exception e)
                {
                    Util.mensaje(1, e.Message);
                }
            });

            if (!res)
            {
                Util.mensaje(1, "La actividad no pudo ser creada");
                this.Proceso = false;
                return;
            }

            TemaVistaModelo.getInstancia().cargaActividades();
            this.Proceso = false;
            Util.pantallaAnterior();
        }

        public void limpiaPantalla()
        {
            this.Nombre = string.Empty;
            this.Proceso = false;
            this.FechaFin = new DateTime();
            this.HoraFin = new DateTime();
            this.Valor = 0;
            this.BotonActivo = true;
            Util.pantallaAnterior();
        }
        

        private async void BuscaArchivo()
        {
            archivo = await CrossFilePicker.Current.PickFile();

            if (archivo != null)
            {
                this.NombreArchivo = archivo.FileName;
                this.BotonActivo = true;
            }
        }

        public async void GuardaArchivo()
        {
            if (archivo == null) { Util.mensaje(1, "El archivo no ha sido cargado"); return; }

            this.Proceso = true;

            var conexion = await new APIServicio().verificaConexion();

            if (!conexion.IsSuccess)
            {
                this.Proceso = false;
                Util.mensaje(1, conexion.Mensaje);
                return;
            }

            int indiceExtension = archivo.FileName.IndexOf(".") + 1;

            archivoM = new archivoModelo();

            archivoM.bufferArchivo = archivo.DataArray;
            archivoM.actividad = new actividadModelo { idActividad = this.actividad.idActividad };
            archivoM.usuario = MainVistaModelo.getInstancia().usuarioCurso;
            archivoM.extension = archivo.FileName.Substring(indiceExtension);
            archivoM.nombre = archivo.FileName.Substring(0, indiceExtension);

            var cliente = MainVistaModelo.getInstancia().getWS();

            bool res = false;

            await Task.Run(() => res = cliente.agregaActualizaArchivo(archivoM));

            if (!res)
            {
                Util.mensaje(1, "EL archivo no pudo ser cargado");
            }
            else
            {
                //Mostrar una imagen de OK
                Util.mensaje(1, "Archivo Agregado Correctamente");
                this.CargaArchivo();
            }
            this.Proceso = false;
        }

        private async void CargaArchivo()
        {
            this.Proceso = true;
            var main = MainVistaModelo.getInstancia();

            var cliente = main.getWS();

            await Task.Run(() => archivoM = cliente.getArchivo(
                this.actividad.idActividad, main.usuarioCurso.idPersona));

            if (archivoM != null)
            {
                this.RutaImagen = archivoM.extension + ".png";
                this.BotonActivo = false;
                this.ImagenVisible = true;
                this.CargarArchivo = false;
            }
            else
            {
                this.CargarArchivo = true; 
            }

            this.Proceso = false;
        }

        public void descargarArchivo()
        {
            Util.mensaje(0, "Archiv descargadi");
        }

        #endregion
    }
}
