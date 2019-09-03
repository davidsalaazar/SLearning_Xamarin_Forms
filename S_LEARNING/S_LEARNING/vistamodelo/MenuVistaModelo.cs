using GalaSoft.MvvmLight.Command;
using S_LEARNING.vista;
using System.Windows.Input;

namespace S_LEARNING.vistamodelo
{
    public class MenuVistaModelo : SlearningWS.cursoModelo
    {
        public ICommand actionRedireccionaCurso
        {
            get { return new RelayCommand(RedireccionaCurso); }
        }

        public void RedireccionaCurso()
        {
            this.nombre = this.nombre.Replace("~", "");
            VMenu.getInstancia().push(this);
        }
    }
}
