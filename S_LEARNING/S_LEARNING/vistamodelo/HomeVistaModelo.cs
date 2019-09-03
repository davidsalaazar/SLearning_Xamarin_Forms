using GalaSoft.MvvmLight.Command;
using S_LEARNING.util;
using SlearningWS;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace S_LEARNING.vistamodelo
{
    public class HomeVistaModelo : BaseVistaModelo
    {
        private usuarioModelo usuario;
        private ObservableCollection<MenuVistaModelo> lCurso;
        private bool proceso;
        private static HomeVistaModelo instancia;

        public static HomeVistaModelo getInstancia()
        {
            return instancia;
        }

        #region Constructor de la clase        
        public HomeVistaModelo()
        {
            instancia = this;
            cargaCursos();
            this.NombreUsuario = usuario.nombre;
            this.CorreoUsuario = usuario.correo;
        }
        #endregion

        public async void cargaCursos()
        {
            this.Proceso = true;
            this.usuario = MainVistaModelo.getInstancia().usuarioCurso;
            await Task.Run(() => {
                this.Cursos = new ObservableCollection<MenuVistaModelo>(convierteCursoAMenuVistaModelo());
                this.Proceso = false;
            });
        }

        #region Variables con get y set
        public string NombreUsuario { get; set; }
        public string CorreoUsuario { get; set; }

        public bool Proceso
        {
            get { return this.proceso; }
            set { setValor(ref this.proceso, value); }
        }
        public ObservableCollection<MenuVistaModelo> Cursos
        {
            get { return this.lCurso; }
            set { setValor(ref this.lCurso, value); }
        }
        #endregion

        #region Eventos
        public ICommand actionRefreshCursos
        {
            get { return new RelayCommand(cargaCursos); }
        }
        public ICommand UserProfile
        {
            get { return new RelayCommand(() => Util.mensaje(0, "Muesta opcion user")); }
        }
        #endregion

        #region Metodos
        public IEnumerable<MenuVistaModelo> convierteCursoAMenuVistaModelo()
        {
            cursoModelo[] cursos = usuario.cursos;

            cursoModelo cursoM = new cursoModelo { idCurso = -1, nombre = "Crear un nuevo curso" };

            List<cursoModelo> lCurso = new List<cursoModelo> { cursoM };

            if (cursos != null)
            {
                foreach (cursoModelo curso in cursos)
                {
                    lCurso.Add(curso);
                }
            }

            return lCurso.Select(l => new MenuVistaModelo
            {
                anio = l.anio,
                idCurso = l.idCurso,
                nombre = l.nombre,
                periodo = l.periodo
            });
        }
        #endregion
    }
}
