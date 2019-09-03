using GalaSoft.MvvmLight.Command;
using S_LEARNING.Servicios;
using S_LEARNING.util;
using S_LEARNING.vista;
using SlearningWS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace S_LEARNING.vistamodelo
{
    public class TemaVistaModelo : BaseVistaModelo
    {
        private bool esRegistro;
        private bool esActividad;
        private string nombre;
        private bool proceso;
        private bool botonActivo;
        private temaModelo tema;
        private int idCurso;
        private ObservableCollection<ActividadItem> lActividad;
        private bool listaVacia;

        private APIServicio apiServicio;

        private static TemaVistaModelo instancia;

        public TemaVistaModelo(int opcion, temaModelo tema, int idCurso)
        {
            this.tema = tema;
            this.idCurso = idCurso;
            this.apiServicio = new APIServicio();
            instancia = this;
            //0 Indica que se debe de mostrar el registro del tema
            //1 Indica que se debe de mostrar la subida del archivo
            if (opcion == 0 && tema == null)
            {
                this.EsRegistro = true;
                this.BotonActivo = true;
            }
            else
            {
                this.EsActividad = true;
                this.BotonActivo = true;
                this.cargaActividades();
            }
        }

        public static TemaVistaModelo getInstancia()
        {
            return instancia;
        }

        #region GET & SET
        public bool EsRegistro
        {
            get { return this.esRegistro; }
            set { setValor(ref this.esRegistro, value); }
        }
        public bool EsActividad
        {
            get { return this.esActividad; }
            set { setValor(ref this.esActividad, value); }
        }
        public string Nombre
        {
            get { return this.nombre; }
            set { setValor(ref this.nombre, value); }
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
        public ObservableCollection<ActividadItem> Actividades
        {
            get { return this.lActividad; }
            set { setValor(ref this.lActividad, value); }
        }
        public bool ListaVacia
        {
            get { return this.listaVacia; }
            set { setValor(ref this.listaVacia, value); }
        }
        #endregion

        #region Eventos
        public ICommand actionRegistrarTema
        {
            get { return new RelayCommand(RegistraTema); }
        }
        public ICommand actionCancelar
        {
            get
            {
                return new RelayCommand(() =>
                {
                    this.Nombre = string.Empty;
                    Util.pantallaAnterior();
                });
            }
        }
        public ICommand refreshActividades
        {
            get { return new RelayCommand(() => cargaActividades()); }
        }
        public ICommand actionRedireccionaRegistroActividad
        {
            get { return new RelayCommand(RedireccionaRegistroActividad); }
        }

        public ICommand actionEliminarTema
        {
            get { return new RelayCommand(EliminarTema); }
        }
        #endregion

        #region Metodos
        private async void RegistraTema()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                Util.mensaje(1, "El nombre esta vacío");
                return;
            }

            this.Proceso = true;

            var conexion = await this.apiServicio.verificaConexion();

            if (!conexion.IsSuccess)
            {
                this.Proceso = false;
                Util.mensaje(1, conexion.Mensaje);
                return;
            }

            var client = MainVistaModelo.getInstancia().getWS();
            client.Timeout = 10000;

            cursoModelo cursoN = new cursoModelo { idCurso = this.idCurso };

            temaModelo tema = new temaModelo { nombre = this.Nombre, curso = cursoN };

            bool res = false;

            await Task.Run(() => res = client.agregaTema(tema));

            if (!res)
            {
                Util.mensaje(1, "El tema no pudo ser creado");
                this.Proceso = false;
                return;
            }

            CursoVistaModelo.getInstancia().cargaTemas();
            this.Proceso = false;
            Util.pantallaAnterior();
        }

        public async void cargaActividades()
        {
            this.Proceso = true;

            var conexion = await this.apiServicio.verificaConexion();

            if (!conexion.IsSuccess)
            {
                this.Proceso = false;
                Util.mensaje(1, conexion.Mensaje);
                return;
            }

            await Task.Run(() => this.Actividades = new ObservableCollection<ActividadItem>(convierteActividadToActividadItem()));

            this.Proceso = false;
        }

        public IEnumerable<ActividadItem> convierteActividadToActividadItem()
        {
            var cliente = MainVistaModelo.getInstancia().getWS();
            cliente.Timeout = 10000;

            actividadModelo[] aActividad = null;

            try
            {
                aActividad = cliente.getActividad(this.tema.idTema);
            }
            catch (Exception) { }

            List<actividadModelo> lActividadN = null;

            if (aActividad == null)
            {
                this.ListaVacia = true;
                lActividadN = new List<actividadModelo>();
                this.Proceso = false;
            }
            else
            {
                this.ListaVacia = false;
                lActividadN = aActividad.ToList<actividadModelo>();
            }

            
            return lActividadN.Select(l => new ActividadItem
            {
                idActividad = l.idActividad,
                fechaFin = l.fechaFin,
                fechaInicio = l.fechaInicio,
                horaFin = l.horaFin,
                horaInicio = l.horaInicio,
                nombre = l.nombre,
                valor = l.valor,
                descripcion = l.descripcion
            }).OrderBy(x => x.idActividad);
        }

        public void RedireccionaRegistroActividad()
        {
            MainVistaModelo.getInstancia().Actividad = new ActividadVistaModelo(this.tema, null);
            Util.muestraPantalla(new VActividad("Registrar nueva Actividad"));
        }

        public async void EliminarTema()
        {
            bool res = await Application.Current.MainPage.DisplayAlert("¡ATENCIÓN!",
                "Esta a punto de eliminar el tema '" + this.tema.nombre + "'", "ACEPTAR", "CANCELAR");

            if (res)
            {
                this.Proceso = true;

                var cliente = MainVistaModelo.getInstancia().getWS();

                bool resS = false;

                await Task.Run(() =>
                {
                    try
                    {
                        resS = cliente.eliminaTema(new temaModelo { idTema = this.tema.idTema});
                    }
                    catch (Exception e) { Util.mensaje(1, e.Message); }
                });

                if (!resS)
                {
                    Util.mensaje(1, "No se ha podido eliminar el tema");
                }
                else
                {
                    CursoVistaModelo.getInstancia().cargaTemas();
                    this.Proceso = false;
                    Util.pantallaAnterior();
                }
            }
        }
    }
    #endregion
}
