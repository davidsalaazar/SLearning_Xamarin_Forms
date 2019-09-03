using SlearningWS;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_LEARNING.vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VCurso : ContentPage
	{
        public VCurso (string nombreCurso)
		{
			InitializeComponent ();
            this.Title = nombreCurso;
		}
    }
}