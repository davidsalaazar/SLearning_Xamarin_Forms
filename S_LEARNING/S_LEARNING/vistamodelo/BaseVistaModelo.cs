using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace S_LEARNING.vistamodelo
{
    public class BaseVistaModelo : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void alCambiarPropiedad([CallerMemberName] string nombrePropiedad = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nombrePropiedad));
        }

        protected void setValor<T>(ref T variableSecundaria, T valor, [CallerMemberName] string nombrePropiedad = null)
        {
            if (!EqualityComparer<T>.Default.Equals(variableSecundaria, valor))
            {
                variableSecundaria = valor;
                alCambiarPropiedad(nombrePropiedad);
                return;
            }
        }
    }
}
