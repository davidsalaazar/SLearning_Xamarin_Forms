using SlearningWS;

namespace S_LEARNING.vistamodelo
{
    public class MainVistaModelo
    {
        public usuarioModelo usuarioCurso { get; set; }

        private static MainVistaModelo mainVistaModelo;
        private static ServidorService sLearningWS;
        
        /**
         * El constructor solo se inicializa una vez entonces 
         * el Login es el principal que se debe de mostrar
         * */
        public MainVistaModelo()
        {
            mainVistaModelo = this;
            this.Login = new LoginVistaModelo();
        }

        #region Instancias de controladores de vistas
        public LoginVistaModelo Login { get; set; }
        public RegistroVistaModelo Registro { get; set; }
        public HomeVistaModelo Home { get; set; }
        public CursoVistaModelo Curso { get; set; }
        public TemaVistaModelo Tema { get; set; }
        public ActividadVistaModelo Actividad { get; set; }
        #endregion

        /**
         * Instancia unica del Main
         * */
        public static MainVistaModelo getInstancia()
        {
            return (mainVistaModelo == null) ? mainVistaModelo = new MainVistaModelo() : mainVistaModelo;
        }

        /**
         * Crea instancia unica hacia el web service y asigna un time out para evitar 
         * que se demore en la ejecucion
         * */
        public ServidorService getWS()
        {
            return (sLearningWS == null) ? sLearningWS = new ServidorService() : sLearningWS;
        }
    }
}
