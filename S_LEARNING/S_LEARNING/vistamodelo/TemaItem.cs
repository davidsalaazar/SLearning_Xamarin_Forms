using GalaSoft.MvvmLight.Command;
using S_LEARNING.util;
using S_LEARNING.vista;
using SlearningWS;
using System.Windows.Input;

namespace S_LEARNING.vistamodelo
{
    public class TemaItem : temaModelo
    {
        public ICommand actionRedireccionaTema
        {
            get
            {
                return new RelayCommand(() =>
                {
                    MainVistaModelo.getInstancia().Tema = new TemaVistaModelo(1, this, 0);
                    Util.muestraPantalla(new VTema(this.nombre));
                });
            }
        }
    }
}
