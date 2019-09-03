using GalaSoft.MvvmLight.Command;
using S_LEARNING.util;
using S_LEARNING.vista;
using SlearningWS;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace S_LEARNING.vistamodelo
{
    public class RegistroVistaModelo : BaseVistaModelo
    {
        #region Variables de clase
        private string soyProfesor;
        private string nombre;
        private string correo;
        private string nickname;
        private string contrasena;
        private string contrasena1;
        private bool btnActivo;
        private bool proceso;
        private bool formsVisibles;
        #endregion

        public RegistroVistaModelo()
        {
            this.nombre = "David Salazar Mejia";
            this.correo = "davidsm54@gmail.com";
            this.nickname = "davidsm54";
            this.contrasena = "123";
            this.contrasena1 = "123";

            this.BtnActivo = true;
            this.FormsVisibles = false;
        }

        #region GET & SET
        public string Nombre
        {
            get { return nombre; }
            set { setValor(ref this.nombre, value); }
        }

        public string Correo
        {
            get { return correo; }
            set { setValor(ref this.correo, value); }
        }

        public string Nickname
        {
            get { return nickname; }
            set { setValor(ref this.nickname, value); }
        }

        public string Contrasena
        {
            get { return contrasena; }
            set { setValor(ref this.contrasena, value); }
        }

        public string Contrasena1
        {
            get { return contrasena1; }
            set { setValor(ref this.contrasena1, value); }
        }

        public bool BtnActivo
        {
            get { return btnActivo; }
            set { setValor(ref this.btnActivo, value); }
        }

        public bool Proceso
        {
            get { return proceso; }
            set { setValor(ref this.proceso, value); }
        }

        public bool FormsVisibles
        {
            get { return formsVisibles; }
            set { setValor(ref this.formsVisibles, value); }
        }

        public string SoyProfesor
        {
            get { return soyProfesor; }
            set { setValor(ref this.soyProfesor, value); this.FormsVisibles = true; }
        }
        #endregion

        #region EVENTOS
        public ICommand actionIniciar
        {
            get { return new RelayCommand(Iniciar); }
        }

        public ICommand actionCancelar
        {
            get { return new RelayCommand(limpiaPantalla); }
        } 
        #endregion

        public async void Iniciar()
        {
            if (string.IsNullOrEmpty(Nombre))
            {
                Util.mensaje(1, "Nombre vacío");
                return;
            }
            if (string.IsNullOrEmpty(Correo))
            {
                Util.mensaje(1, "Correo vacío");
                return;
            }
            if (string.IsNullOrEmpty(Nickname))
            {
                Util.mensaje(1, "Nickname vacío");
                return;
            }
            if (string.IsNullOrEmpty(Contrasena))
            {
                Util.mensaje(1, "Contrasena vacía");
                return;
            }
            if (string.IsNullOrEmpty(Contrasena1))
            {
                Util.mensaje(1, "Contraseña vacía");
                return;
            }

            if (!Contrasena1.Equals(Contrasena))
            {
                Util.mensaje(1, "Las contraseñas no coinciden");
                return;
            }

            this.Proceso = true;

            usuarioModelo usuario = new usuarioModelo();
            rolModelo rol = new rolModelo
            {
                idRol = (this.soyProfesor.Equals("Soy Profesor")) ? 1 : 2
            };

            usuario.nombre = this.Nombre;
            usuario.correo = this.Correo;
            usuario.nickname = this.Nickname;
            usuario.contrasena = this.Contrasena;
            usuario.rol = rol;

            try
            {
                var mainVistaM = MainVistaModelo.getInstancia();
                var cliente = mainVistaM.getWS();

                cliente.Timeout = 6000;
                bool res = false;

                await Task.Run(() => res = cliente.registro(usuario));

                if (!res)
                {
                    Util.mensaje(1, "Nickname o correo no disponibles");
                }
                else
                {
                    usuarioModelo usuarioM = null;

                    await Task.Run(() => usuarioM = cliente.login(usuario.nickname, usuario.contrasena, true));

                    mainVistaM.usuarioCurso = usuarioM;
                    mainVistaM.Home = new HomeVistaModelo();
                    Util.muestraPantalla(new VMenu());
                }
                this.Proceso = false;
            }
            catch
            {
                this.Proceso = false;
            }
        }

        public void limpiaPantalla()
        {
            this.Nombre = string.Empty;
            this.Correo = string.Empty;
            this.Nickname = string.Empty;
            this.Contrasena = string.Empty;
            this.Contrasena1 = string.Empty;
            this.SoyProfesor = null;
            this.BtnActivo = true;
            this.FormsVisibles = false;
            Util.pantallaAnterior();
        }

    }
}
