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
    public class CursoVistaModelo : BaseVistaModelo
    {
        private bool proceso;
        private bool registroCurso;
        private bool esTema;
        private ObservableCollection<TemaItem> lTema;
        private int idCurso;
        private string nombreCurso;
        private string periodo;

        private bool listaVacia;
        private bool botonActivo;
        private static CursoVistaModelo instancia;

        public CursoVistaModelo(cursoModelo curso)
        {
            this.idCurso = curso.idCurso;

            instancia = this;
            //Si el curso id es igual a -1 entonces se debe registrar un nuevo curso
            if (idCurso == -1)
            {
                this.RegistroCurso = true;
            }
            else
            {
                this.EsTema = true;
                this.cargaTemas();
            }
        }

        public static CursoVistaModelo getInstancia()
        {
            return instancia;
        }

        #region Get and set
        public bool Proceso
        {
            get { return this.proceso; }
            set { setValor(ref this.proceso, value); }
        }

        public bool RegistroCurso
        {
            get { return this.registroCurso; }
            set { setValor(ref this.registroCurso, value); }
        }

        public bool EsTema
        {
            get { return this.esTema; }
            set { setValor(ref this.esTema, value); }
        }

        public string NombreCurso
        {
            get { return this.nombreCurso; }
            set { setValor(ref this.nombreCurso, value); }
        }

        public string Periodo
        {
            get { return this.periodo; }
            set { setValor(ref this.periodo, value); }
        }

        public bool ListaVacia
        {
            get { return this.listaVacia; }
            set { setValor(ref this.listaVacia, value); }
        }

        public bool BotonActivo
        {
            get { return this.botonActivo; }
            set { setValor(ref this.botonActivo, value); }
        }

        public ObservableCollection<TemaItem> Temas
        {
            get { return this.lTema; }
            set { setValor(ref this.lTema, value); }
        }
        #endregion

        #region Eventos
        public ICommand CargaTemas
        {
            get { return new RelayCommand(cargaTemas); }
        }

        public ICommand actionRegistrarCurso
        {
            get { return new RelayCommand(RegistraCurso); }
        }

        public ICommand actionRedireccionaRegistroTema
        {
            get { return new RelayCommand(RedireccionaRegistroTema); }
        }

        public ICommand actionEliminarCurso
        {
            get { return new RelayCommand(EliminaCurso); }
        }
        #endregion

        #region Metodos
        public async void cargaTemas()
        {
            this.Proceso = true;
            await Task.Run(() =>
            {
                this.Temas = new ObservableCollection<TemaItem>(convierteTemaToTemaItem());
                this.Proceso = false;
            });
        }

        public IEnumerable<TemaItem> convierteTemaToTemaItem()
        {
            var cliente = MainVistaModelo.getInstancia().getWS();

            temaModelo[] aTema = null;

            try
            {
                aTema = cliente.getTema(idCurso);
            }
            catch (Exception) { }

            List<temaModelo> lTema = null;

            if (aTema == null)
            {
                ListaVacia = true;
                lTema = new List<temaModelo>();
                this.Proceso = false;
            }
            else
            {
                ListaVacia = false;
                lTema = aTema.ToList<temaModelo>();
            }

            return lTema.Select(l => new TemaItem
            {
                idTema = l.idTema,
                curso = l.curso,
                nombre = l.nombre
            }).OrderBy(x => x.idTema);
        }


        private void RedireccionaRegistroTema()
        {
            MainVistaModelo.getInstancia().Tema = new TemaVistaModelo(0, null, this.idCurso);
            Util.muestraPantalla(new VTema("Registrar nuevo tema"));
        }

        private async void EliminaCurso()
        {
            bool res = await Application.Current.MainPage.DisplayAlert("¡ATENCIÓN!",
                "Esta a punto de eliminar todo el curso" + this.NombreCurso, "ACEPTAR", "CANCELAR");

            if (res)
            {
                this.Proceso = true;
                var main = MainVistaModelo.getInstancia();
                var cliente = main.getWS();

                bool resS = false;
                try
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            resS = cliente.eliminaCurso(new cursoModelo { idCurso = this.idCurso });
                        }
                        catch (Exception) { }
                    });

                    if (!resS)
                    {
                        string mensaje = string.Format("No se ha podido eliminar el curso{0}Pruebe " +
                            "con borrar las actividades de este curso{0}", Environment.NewLine);
                        Util.mensaje(1, mensaje);
                    }
                    else
                    {
                        usuarioModelo usuarioM = null;

                        usuarioModelo usuarioN = main.usuarioCurso;

                        await Task.Run(() =>
                        {
                            usuarioM = cliente.login(usuarioN.nickname, usuarioN.contrasena, true);
                            main.usuarioCurso = usuarioM;
                            HomeVistaModelo.getInstancia().cargaCursos();
                        });
                        Util.mensaje(0, "Curso Eliminado");

                        VMenu.getInstancia().irAlInicio();
                    }
                }
                catch (Exception)
                {
                    Util.mensaje(1, "No se ha podido eliminar el curso");
                }
            }
            this.Proceso = false;
        }

        public async void RegistraCurso()
        {
            if (string.IsNullOrEmpty(NombreCurso))
            {
                Util.mensaje(1, "Nombre del curso vacío");
                return;
            }

            if (string.IsNullOrEmpty(Periodo))
            {
                Util.mensaje(1, "Seleccione un periodo");
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

            cursoModelo curso = new cursoModelo
            {
                anio = 2018,
                nombre = NombreCurso,
                periodo = Periodo
            };

            List<cursoModelo> lCurso = new List<cursoModelo>();

            var main = MainVistaModelo.getInstancia();

            usuarioModelo usuario = main.usuarioCurso;

            lCurso = (usuario.cursos != null) ? usuario.cursos.ToList<cursoModelo>() : lCurso;

            lCurso.Add(curso);

            usuario.cursos = lCurso.ToArray();

            bool res = main.getWS().registro(usuario);

            if (!res)
            {
                Util.mensaje(1, "El curso no ha podido ser creado");
            }
            else
            {
                usuarioModelo usuarioM = null;

                await Task.Run(() =>
                {
                    usuarioM = main.getWS().login(usuario.nickname, usuario.contrasena, true);
                    main.usuarioCurso = usuarioM;
                    HomeVistaModelo.getInstancia().cargaCursos();
                });
                Util.mensaje(0, "Curso creado");
            }
            this.Proceso = false;
            this.Periodo = null;
            this.NombreCurso = string.Empty;
        }
        #endregion
    }
}
