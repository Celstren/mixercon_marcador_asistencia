

namespace Marcador_Asistencia
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Data.SqlClient;
    using System.Data;
    using System.Runtime.CompilerServices;
    class ClassConexion
    {
        static SqlConnection miConexion;
        private static ClassConexion Instancia = null;


        private ClassConexion() { }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void CrearInstancia()
        {
            Instancia = new ClassConexion();
            miConexion = new SqlConnection(Properties.Settings.Default.ConnectionString);
        }

        public static ClassConexion ConseguirInstancia()
        {
            if (Instancia == null)
            {
                CrearInstancia();
            }
            return Instancia;
        }
        public bool primeraConexion(ref string mesajeErrorBase, ref string Numero_Error)
        {
            bool algunError = true;
            SqlConnection miConexion = new SqlConnection(Properties.Settings.Default.ConnectionString);
            try
            {
                miConexion.Open();
                miConexion.Close();
            }
            catch (SqlException ex)
            {
                Numero_Error = ex.Number.ToString();
                mesajeErrorBase = ex.Message;
                algunError = false;
            }
            return algunError;
        }

        //***************Conectar Nuevo..


        public bool ejecutarConsulta(string Nombre, object[,] Parametros, ref string mesajeErrorBase, ref string Numero_Error, ref DataTable Consulta)
        {
            bool operacionExitosa = true;
            DataTable Tabla = new DataTable();
            SqlDataAdapter Adaptador = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            try
            {


                miConexion.Open();

                cmd.CommandText = Nombre;
                cmd.Connection = miConexion;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                string paramProcedumiento = string.Empty;
                object asignarValor = string.Empty;
                for (int i = 0; i < Parametros.GetLength(0); i++)
                {
                    paramProcedumiento = string.Empty;
                    asignarValor = string.Empty;
                    for (int j = 0; j < Parametros.GetLength(1); j++)
                    {
                        if (j == 0) paramProcedumiento = Parametros[i, j].ToString();
                        if (j == 1) asignarValor = Parametros[i, j];
                    }
                    cmd.Parameters.AddWithValue(paramProcedumiento, asignarValor);
                }
                SqlParameter boolError = new SqlParameter("bool_Error", SqlDbType.Bit);
                boolError.Direction = ParameterDirection.Output;

                cmd.Parameters.Add(boolError);
                Adaptador.SelectCommand = cmd;
                Adaptador.Fill(Consulta);
                operacionExitosa = Convert.ToBoolean(boolError.Value);

            }
            catch (SqlException ex)
            {
                mesajeErrorBase = ex.Message;
                Numero_Error = ex.Number.ToString();
                operacionExitosa = false;
            }
            catch (Exception ex)
            {
                mesajeErrorBase = ex.Message;
                Numero_Error = string.Empty;
                operacionExitosa = false;
            }
            miConexion.Close();

            return operacionExitosa;
        }

        public bool EjecutarSentencia(string Nombre, object[,] Parametros, ref string mesajeErrorBase, ref string Numero_Error)
        {
            bool operacionExitosa = true;
            try
            {
                SqlCommand cmd = new SqlCommand();
                miConexion.Open();

                cmd.CommandText = Nombre;
                cmd.Connection = miConexion;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Clear();

                string paramProcedumiento = null;
                object asignarValor = null;

                for (int i = 0; i < Parametros.GetLength(0); i++)
                {
                    paramProcedumiento = string.Empty;
                    asignarValor = string.Empty;
                    for (int j = 0; j < Parametros.GetLength(1); j++)
                    {
                        if (j == 0) paramProcedumiento = Parametros[i, j].ToString();
                        if (j == 1) asignarValor = Parametros[i, j];
                    }
                    cmd.Parameters.AddWithValue(paramProcedumiento, asignarValor);
                }

                SqlParameter boolError = new SqlParameter("bool_Error", SqlDbType.Bit);

                boolError.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(boolError);
                cmd.ExecuteNonQuery();
                operacionExitosa = Convert.ToBoolean(boolError.Value);

            }

            catch (SqlException ex)
            {
                mesajeErrorBase = ex.Message;
                Numero_Error = ex.Number.ToString();
                operacionExitosa = false;
            }
            catch (Exception ex)
            {
                mesajeErrorBase = ex.Message;
                Numero_Error = string.Empty;
                operacionExitosa = false;
            }
            miConexion.Close();
            return operacionExitosa;
        }

        //****************Conectar antiguo
       // public void conectar(string consulta)
       // {
       //     SqlCommand command = new SqlCommand();

       //     miConexion.Open();
       //     command = new SqlCommand(consulta, miConexion);

       //     command.ExecuteNonQuery();

       //     miConexion.Close();
          

       // }
       //public DataTable Consulta(string consulta)
       // {
       //    miConexion.Open();
       //    SqlDataAdapter adapter = new SqlDataAdapter(consulta, miConexion);
       //     DataTable Demo = new DataTable();
       //     adapter.Fill(Demo);
       //     miConexion.Close();
       //     return (Demo);
             
       // }
    }
}
