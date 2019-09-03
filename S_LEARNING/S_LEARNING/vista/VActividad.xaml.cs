using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_LEARNING.vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class VActividad : ContentPage
	{
		public VActividad (string titulo)
		{
			InitializeComponent ();
            this.Title = titulo;
		}
	}
}