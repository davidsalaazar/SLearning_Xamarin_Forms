namespace S_LEARNING.util
{
    using S_LEARNING.vistamodelo;
    public class InstanceLocator
    {
        #region propiedades
        public MainVistaModelo Main { get; set; }
        #endregion

        #region Constructores
        public InstanceLocator()
        {
            this.Main = new MainVistaModelo();
        }
        #endregion
    }
}
