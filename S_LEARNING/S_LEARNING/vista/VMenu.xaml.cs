using S_LEARNING.vistamodelo;
using SlearningWS;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_LEARNING.vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VMenu : MasterDetailPage
    {
        private static VMenu instancia;

        public static VMenu getInstancia()
        {
            return (instancia == null) ? instancia = new VMenu() : instancia;
        }

        public VMenu()
        {
            InitializeComponent();
            instancia = this;
            Detail = new NavigationPage(new VHome());
        }

        public void irAlInicio()
        {
            Detail = new NavigationPage(new VHome());
        }

        public void push(cursoModelo curso)
        {
            MainVistaModelo.getInstancia().Curso = new CursoVistaModelo(curso);
            Detail = new NavigationPage(new VCurso(curso.nombre + "   ~   " + curso.periodo));
            IsPresented = false;
        }
    }
}