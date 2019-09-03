using Plugin.Connectivity;
using S_LEARNING.modelo;
using System.Threading.Tasks;

namespace S_LEARNING.Servicios
{
    public class APIServicio
    {
        public async Task<RespuestaModelo> verificaConexion()
        {
            RespuestaModelo respuesta = new RespuestaModelo();
            respuesta.IsSuccess = true;
            respuesta.Mensaje = "OK";

            if (!CrossConnectivity.Current.IsConnected)
            {
                respuesta.IsSuccess = false;
                respuesta.Mensaje = "Sin conexión WI-FI o Datos 3G";
            }
            else
            {
                var hayConexionInternet = await CrossConnectivity.Current.IsRemoteReachable("google.com");

                //if (!hayConexionInternet)
                //{
                //    respuesta.IsSuccess = false;
                //    respuesta.Mensaje = "UPPS! Actualmente no hay conexión a internet";
                //}
            }
            return respuesta;
        }
    }
}