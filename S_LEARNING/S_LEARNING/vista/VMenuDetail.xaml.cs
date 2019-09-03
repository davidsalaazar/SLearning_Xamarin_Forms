using SlearningWS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace S_LEARNING.vista
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VMenuDetail : ContentPage
    {
        public VMenuDetail(cursoModelo curso)
        {
            InitializeComponent();
        }
    }
}