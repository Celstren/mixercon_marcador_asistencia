

namespace Generico
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Runtime.CompilerServices;
    class ClassMensajes
    {
        private static ClassMensajes Instancia = null;

         private ClassMensajes() { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CrearInstancia()
        {
            Instancia = new ClassMensajes();
        }
        public static ClassMensajes ConseguirInstancia()
        {
            if (Instancia == null)
            {
                CrearInstancia();
            }
            return Instancia;
        }



        public void mesanjeError(string contedido)
        {
            MessageBox.Show(contedido, "Operacion Invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        public void mesanjeExistoso(string contedido)
        {
            MessageBox.Show(contedido, "Operacion Existosa", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        public DialogResult mesajePreguntar(string detalle, string titulo)
        {
            return MessageBox.Show(detalle, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }


        public void mesanjeErrorBase(string numeroError, string Tabla, string contenido)
        {

            switch (numeroError)
            {
                case "-2147467259":
                    contenido = Tabla + " ya existe en la Base";
                    break;
                case "1042":
                    contenido = "Error conexion Base de datos" + (char)13 + "Compruebe conexion al Servidor";
                    break;
            }
            MessageBox.Show(contenido + (char)13 + (char)10 + "Numero Error: " + numeroError, "Operacion Invalida", MessageBoxButtons.OK, MessageBoxIcon.Error);

        }
    }
}
