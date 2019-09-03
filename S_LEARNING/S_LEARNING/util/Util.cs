using Xamarin.Forms;

namespace S_LEARNING.util
{
    public class Util
    {
        public async static void mensaje(int opcion, string mensaje)
        {
            string tipoMensaje = string.Empty;

            switch (opcion)
            {
                case 0:
                    tipoMensaje = "HECHO";
                    break;
                case 1:
                    tipoMensaje = "ERROR";
                    break;
                case 2:
                    tipoMensaje = "¡ATENCIÓN!";
                    break;
            }

            await Application.Current.MainPage.DisplayAlert(tipoMensaje, mensaje, "OK");
        }

        public async static void muestraPantalla(Page pagina)
        {
            await Application.Current.MainPage.Navigation.PushAsync(pagina);
        }

        public async static void pantallaAnterior()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
