using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_LEARNING.vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VTema : ContentPage
	{
		public VTema (string nombreTema)
		{
			InitializeComponent ();
            Title = nombreTema;
		}
	}
}