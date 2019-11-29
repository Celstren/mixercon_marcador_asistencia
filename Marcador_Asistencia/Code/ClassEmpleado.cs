

namespace Marcador_Asistencia.Empleado
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data;
    using System.Runtime.CompilerServices;
    class ClassEmpleado
    {

        ClassConexion conn = ClassConexion.ConseguirInstancia();

        private static ClassEmpleado Instancia = null;
        private ClassEmpleado() { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CrearInstancia()
        {
            Instancia = new ClassEmpleado();
        }

        public static ClassEmpleado ConseguirInstancia()
        {
            if (Instancia == null)
            {
                CrearInstancia();
            }
            return Instancia;
        }


        public enum ConsultaEmpleado : int
        {
            xAll = 0,
            CodEmpresaxCodInterno=1
        }

        public string COD_INTERNO { get; set; }
        public string COD_EMPRESA { get; set; }
        public int PRIVILEGIO { get; set; }
        public string PASSWORD { get; set; }


        public bool Sentencias(int P_TIPO_EVENTO, ref string mensajeErrorBase, ref string numeroError)
        {
            object[,] Parametros =  
            {
                {"P_TIPO_EVENTO",P_TIPO_EVENTO},
                {"P_COD_INTERNO",this.COD_INTERNO},
                {"P_COD_EMPRESA",this.COD_EMPRESA },
                {"P_PRIVILEGIO",this.PRIVILEGIO },
                {"P_PASSWORD",this.PASSWORD}
            };
            return conn.EjecutarSentencia("SP_ATTEmpleadoGuardar", Parametros, ref mensajeErrorBase, ref numeroError);

        }

        public bool Consulta(int P_TIPO_EVENTO, string ID_USUARIO, ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                {"P_TIPO_EVENTO", P_TIPO_EVENTO},
                {"P_ID_USUARIO" , ID_USUARIO}

            };
            return conn.ejecutarConsulta("SP_ATTUsuarioConsulta", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);

        }

        public bool ConsultaOFIPLAN(int P_TIPO_EVENTO, ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                {"P_TIPO_EVENTO", P_TIPO_EVENTO},
                {"P_COD_INTERNO", this.COD_INTERNO},
                {"P_COD_EMPRESA", this.COD_EMPRESA}

            };
            return conn.ejecutarConsulta("SP_ATTEmpleadoConsulta", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);

        }

        public bool ObtenerDatosPersonales(string codigoEmpleado, ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                {"@Codigo_Trabajador", codigoEmpleado}

            };
            return conn.ejecutarConsulta("usp_selectemplbycodtrab", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);
        }

        public bool ObtenerTiempoServidor(ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                

            };
            return conn.ejecutarConsulta("usp_getserverdatetime", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);
        }

        public bool ValidaIngreso(string codigoEmpleado,  ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                {"@ID", codigoEmpleado}

            };
            return conn.ejecutarConsulta("usp_getemployeeschedulebyid1", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);

        }

        public bool ValidaMarcacion(string codigoEmpleado, string nombreUsuario, string nombrePC, string direccionIP, ref string mensajeErrorBase, ref string numeroError, ref DataTable Consulta)
        {
            object[,] Parametros =  
            {
                {"@Id", codigoEmpleado},
                {"@WindowsUser", nombreUsuario},
                {"@PCname", nombrePC},
                {"@Ip", direccionIP}

            };
            return conn.ejecutarConsulta("tusp_getemployeeschedulebyid1", Parametros, ref mensajeErrorBase, ref numeroError, ref Consulta);

        }

    }
}
