namespace S_LEARNING.vistamodelo
{
    using GalaSoft.MvvmLight.Command;
    using S_LEARNING.Servicios;
    using S_LEARNING.util;
    using S_LEARNING.vista;
    using SlearningWS;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Xamarin.Forms;

    public class LoginVistaModelo : BaseVistaModelo
    {
        private string email;
        private string password;
        private bool proceso;
        private bool btnActivo;

        private APIServicio apiServicio;

        public LoginVistaModelo()
        {
            this.Email = "davidsm54";
            this.Password = "123";
            this.BtnActivo = true;
            this.apiServicio = new APIServicio();
        }

        #region GET AND SET
        public string Email
        {
            get { return this.email; }
            set { setValor(ref this.email, value); }
        }

        public string Password
        {
            get { return this.password; }
            set { setValor(ref this.password, value); }
        }

        public bool Proceso
        {
            get { return this.proceso; }
            set { setValor(ref this.proceso, value); }
        }

        public bool BtnActivo
        {
            get { return this.btnActivo; }
            set { setValor(ref this.btnActivo, value); }
        }
        #endregion

        #region Eventos que arroja la Vista
        public ICommand actionIniciar
        {
            get { return new RelayCommand(Iniciar); }
        }

        public ICommand actionRegistrar
        {
            get { return new RelayCommand(Registrar); }
        }
        #endregion

        #region Metodos
        public async void Iniciar()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                Util.mensaje(1, "Email vacío");
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                Util.mensaje(1, "Password vacío");
                return;
            }

            this.BtnActivo = false;
            this.Proceso = true;

            bool correo = this.Email.Contains("@");

            var conexion = await this.apiServicio.verificaConexion();

            if (!conexion.IsSuccess)
            {
                this.BtnActivo = true;
                this.Proceso = false;
                Util.mensaje(1, conexion.Mensaje);
                return;
            }

            usuarioModelo usuario = null;
            try
            {
                var client = MainVistaModelo.getInstancia().getWS();
                client.Timeout = 10000;

                await Task.Run(() => usuario = client.login(Email, Password, correo));

                usuario = client.login(Email, Password, correo);
                this.limpiaPantalla();

                if (usuario == null)
                {
                    this.BtnActivo = true;
                    this.Proceso = false;
                    Util.mensaje(1, "Usuario o contraseña incorrectos");
                    return;
                }

                MainVistaModelo.getInstancia().usuarioCurso = usuario;
                MainVistaModelo.getInstancia().Home = new HomeVistaModelo();

                this.BtnActivo = true;
                this.Proceso = false;

                Util.muestraPantalla(new VMenu());
            }
            catch (Exception e)
            {
                limpiaPantalla();
                Util.mensaje(1, e.Message);
            }

        }

        public void Registrar()
        {
            MainVistaModelo.getInstancia().Registro = new RegistroVistaModelo();
            Util.muestraPantalla(new VRegistro());
        }
        #endregion

        private void limpiaPantalla()
        {
            this.Email = string.Empty;
            this.Password = string.Empty;
            this.Proceso = false;
            this.BtnActivo = true;
        }
    }
}
