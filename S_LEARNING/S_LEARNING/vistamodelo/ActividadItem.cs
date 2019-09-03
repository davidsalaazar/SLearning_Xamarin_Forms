using GalaSoft.MvvmLight.Command;
using S_LEARNING.util;
using S_LEARNING.vista;
using SlearningWS;
using System.Windows.Input;

namespace S_LEARNING.vistamodelo
{
    public class ActividadItem : actividadModelo
    {
        public ICommand actionAbreActividad
        {
            get { return new RelayCommand(() =>
            {
                MainVistaModelo.getInstancia().Actividad = new ActividadVistaModelo(null, this);
                Util.muestraPantalla(new VActividad(this.nombre));
            }); }
        }

    }
}
