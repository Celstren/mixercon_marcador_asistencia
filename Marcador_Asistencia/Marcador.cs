using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Linq;
using System.Text;
using AForge.Video;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using System.Collections.Generic;
using Marcador_Asistencia.Empleado;

namespace Marcador_Asistencia
{
    public partial class Marcador : Form
    {

        #region Variables_Privadas
        private FilterInfoCollection webcam;//Colección de dispositivos con cámara
        private VideoCaptureDevice cam;//Objeto cámara
        private DateTime ServerTime;//Tiempo del servidor de la base de datos
        private string Ruta_Foto_Empleado = @"\\192.168.15.246\Comunes\Rrhh\cery\RECURSOS HUMANOS\FIRMAS\T";//Ruta estática para obtener fotos de los empleados
        private string Ruta_Guardar_Foto_Empleado = @"\\192.168.15.246\Comunes\Rrhh\cery\RECURSOS HUMANOS\Fotos Personal\";//Ruta estática para guardar fotos de los empleados
        private bool Puede_Cerrar = false;
        private int Tiempo_Cerrar = Marcador_Asistencia.Properties.Settings.Default.DialogCloseTime;
        private string Direccion_IP = Obtener_DireccionIP();
        private string Nombre_Usuario = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
        private string Nombre_PC = System.Environment.MachineName;
        #endregion

        #region Funciones_Privadas

        private void Obtener_Tiempo_Servidor()
        {
            #region Declarar_Inicializar_Variables
            //Clase empleado y tabla en la que se guardarán los resultados
            ClassEmpleado clase = ClassEmpleado.ConseguirInstancia();
            DataTable dataTale = new DataTable();
            //Mensaje y número de error
            string mensajeErrorBase = string.Empty;
            string numeroError = string.Empty;

            #endregion

            clase.ObtenerTiempoServidor(ref mensajeErrorBase, ref numeroError, ref dataTale);//Obtiene la hora del servidor de la base de datos

            if (dataTale.Rows.Count > 0)//Condición para verificar que ha conseguido un resultado
            {
                DataRow r = dataTale.Rows[0];//Variable para guardar la primera línea del resultado obtenido

                ServerTime = Convert.ToDateTime(r["TIEMPO"].ToString());//Establece la variable privada con la hora del servidor
            }

        }

        private void Prender_Camara()
        {
            if (!cam.IsRunning)//Si la cámara está encendida
            {

                cam = new VideoCaptureDevice(webcam[DispositivosVideocbx.SelectedIndex].MonikerString);//Establece la variable privada para obtener la cámara que se va utilizar

                cam.NewFrame += new NewFrameEventHandler(cam_NewFrame);//Evento para controlar el cambio continuo de la imagen captada por la webcam frame por frame

                cam.Start();//Se enciende la cámara

            }
        }

        private void cam_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bit = (Bitmap)eventArgs.Frame.Clone();//Obtiene la imagen de la cámara y la guarda en una variable
            Fotopbx.Image = bit;//La variable se muestra en el picturebox Fotopbx
        }

        private void Limpiar_Valores()
        {
            //Establece una cadena vacía a todos los campos que corresponden a la respuesta
            Nombreselbl.Text = "";
            ApelMateelbl.Text = "";
            ApelPateelbl.Text = "";
            EstaLlegelbl.Text = "";
            Respuestaelbl.Text = "";
            Respuesta2elbl.Text = "";
            DifMinutoselbl.Text = "";            
            Comentarioelbl.Text = "";
            HoraMarcacionelbl.Text = "";
            HoraProgSalidaelbl.Text = "";
            HoraProgIngresoelbl.Text = "";
            ToleranciaEntradaelbl.Text = "";
            ToleranciaTardanzaelbl.Text = "";

            //Oculta los valores            
            Nombreselbl.Visible = false;
            ApelPateelbl.Visible = false;
            ApelMateelbl.Visible = false;
            EstaLlegelbl.Visible = false;
            Respuestaelbl.Visible = false;
            Comentarioelbl.Visible = false;
            DifMinutoselbl.Visible = false;
            Respuesta2elbl.Visible = false;            
            HoraMarcacionelbl.Visible = false;
            HoraProgSalidaelbl.Visible = false;
            HoraProgIngresoelbl.Visible = false;
            ToleranciaEntradaelbl.Visible = false;
            ToleranciaTardanzaelbl.Visible = false;
            
            Empleadopbx.Image = null;

        }

        private void Limpiar_Valores_No_Datos_Personales()
        {
            //Establece una cadena vacía a todos los campos que corresponden a la respuesta
            EstaLlegelbl.Text = "";
            Respuestaelbl.Text = "";
            Respuesta2elbl.Text = "";
            DifMinutoselbl.Text = "";
            Comentarioelbl.Text = "";
            HoraMarcacionelbl.Text = "";
            HoraProgSalidaelbl.Text = "";
            HoraProgIngresoelbl.Text = "";
            ToleranciaEntradaelbl.Text = "";
            ToleranciaTardanzaelbl.Text = "";

            //Oculta los valores
            EstaLlegelbl.Visible = false;
            Respuestaelbl.Visible = false;
            Respuesta2elbl.Visible = false;
            Comentarioelbl.Visible = false;
            DifMinutoselbl.Visible = false;
            HoraMarcacionelbl.Visible = false;
            HoraProgSalidaelbl.Visible = false;
            HoraProgIngresoelbl.Visible = false;
            ToleranciaEntradaelbl.Visible = false;
            ToleranciaTardanzaelbl.Visible = false;

        }

        private void Completar_Datos(DataRow row)
        {
            #region Establecer_Valores_Campos

            int diferencia_horas = Convert.ToInt32(row["DIF_MIN"].ToString()) / 60;
            int diferencia_minutos = Convert.ToInt32(row["DIF_MIN"].ToString()) % 60;

            //Muestra la hora marcada según el servidor
            EstaLlegelbl.Text = row["EST_LLEG"].ToString();//Obtiene el estado de su llegada según lo que se le programó
            DifMinutoselbl.Text = diferencia_horas.ToString() + " horas " + diferencia_minutos + " minutos";
            HoraMarcacionelbl.Text = ServerTime.ToString("HH:mm");
            ToleranciaEntradaelbl.Text = row["TOLE_INGR_ANTES"].ToString();
            ToleranciaTardanzaelbl.Text = row["TOLE_INGR_DESPU"].ToString();
            HoraProgIngresoelbl.Text = row["NU_HRAS_INIC"].ToString().Substring(0, 2) + ":" + row["NU_HRAS_INIC"].ToString().Substring(2, 2);//Obtiene la hora de ingreso que se le programó
            HoraProgSalidaelbl.Text = row["NU_HRAS_FINA"].ToString().Substring(0, 2) + ":" + row["NU_HRAS_FINA"].ToString().Substring(2, 2);//Obtiene la hora de salida que se le programó



            EstaLlegelbl.Visible = true;
            DifMinutoselbl.Visible = true;
            HoraMarcacionelbl.Visible = true;
            HoraProgSalidaelbl.Visible = true;
            HoraProgIngresoelbl.Visible = true;
            ToleranciaEntradaelbl.Visible = true;
            ToleranciaTardanzaelbl.Visible = true;
            #endregion

            #region Validar_Respuesta

            if (row["EST_LLEG"].ToString() == "TARDANZA" || row["EST_LLEG"].ToString() == "INGRESO PERMITIDO" || row["EST_LLEG"].ToString() == "SALIDA PERMITIDA")//Condición para validar el tipo de respuesta obtenida
            {
                Respuestaelbl.Text = "Horario programado";
                Respuestaelbl.ForeColor = Color.Green;//Establece el color de fuente del campo a verde
                Respuesta2elbl.Text = row["EST_LLEG"].ToString();
                Respuesta2elbl.ForeColor = Color.Green;
                Comentarioelbl.Visible = false;
            }
            else if (row["EST_LLEG"].ToString() == "FUERA DE TIEMPO" || row["EST_LLEG"].ToString() == "INGRESO NO PERMITIDO" || row["EST_LLEG"].ToString() == "SALIDA NO PERMITIDA")
            {
                Respuestaelbl.Text = "Horario programado";
                Respuestaelbl.ForeColor = Color.Red;//Establece el color de fuente del campo a rojo
                Respuesta2elbl.Text = row["EST_LLEG"].ToString();
                Respuesta2elbl.ForeColor = Color.Red;
                Comentarioelbl.Text = "Comunicarse con su jefatura.";//Añade una sugerencia al empleado que llegó
                Comentarioelbl.Visible = true;
            }
            else
            {
                Respuestaelbl.Text = "Horario no programado";//Si es "N" entonces su llegada no está dentro de su horario programado
                Respuestaelbl.ForeColor = Color.Red;//Establece el color de fuente del campo a rojo
                Respuesta2elbl.Text = row["EST_LLEG"].ToString();
                Respuesta2elbl.ForeColor = Color.White;
                Comentarioelbl.Text = "Comunicarse con su jefatura.";//Añade una sugerencia al empleado que llegó
                Comentarioelbl.Visible = true;
            }

            if (Respuesta2elbl.Text == "TARDANZA")
            {
                Respuesta2elbl.ForeColor = Color.Red;
                Comentarioelbl.Text = "Comunicarse con su jefatura.";//Añade una sugerencia al empleado que llegó
                Comentarioelbl.Visible = true;
            }

            Respuestaelbl.Visible = true;
            Respuesta2elbl.Visible = true;
            #endregion
        }

        private void Tomar_Foto(string nombre)
        {
            if (Fotopbx.Image != null)//Valida si el picturebox tiene una imagen que guardar
            {
                PictureBox tbmp = Fotopbx;
                using (Bitmap bitmap = new Bitmap(Fotopbx.Width, Fotopbx.Height))
                {
                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(Color.Transparent);
                        graphics.DrawImage(tbmp.Image, 100, 100);
                    }
                }

                if (Directory.Exists(Ruta_Guardar_Foto_Empleado))
                {
                    try
                    {
                        tbmp.Image.Save(Ruta_Guardar_Foto_Empleado + nombre + ".jpg");//Guarda la imagen en la ruta establecida  
                    }
                    catch (Exception ex)
                    {

                    }
                }
                              
            }
        }

        private void Cargar_Foto(DataRow r)
        {
            if (File.Exists(Ruta_Foto_Empleado + r["CO_TRAB"] + ".jpg"))//Valida si existe una foto del trabajador en la ruta de la carpeta compartida
            {
                Cargar_Imagen_Ruta(Ruta_Foto_Empleado + r["CO_TRAB"] + ".jpg");//Llama la función para cargar la imagen desde una ruta
            }
            else
            {
                Empleadopbx.Image = null;
            }
        }

        private void Cargar_Imagen_Ruta(string Path)
        {
            try
            {
                Empleadopbx.Image = new Bitmap(Path);//Trata de cargar la imagen desde la ruta y la guarda en el picturebox
            }
            catch (Exception ex)
            {
                Empleadopbx.Image = null;//Si no pudo cargar la imagen establece un valor nulo
            }
        }

        private void validarusuario()
        {

            if (Usuariotxt.Text != string.Empty)
            {
                if (Contrasenatxt.Text != string.Empty)
                {
                    ClassEmpleado clase = ClassEmpleado.ConseguirInstancia();//Declara la variable clase de ClaseEmpleado
                    DataTable tempConsultaUsuario = new DataTable();
                    
                    string errorMensaje = string.Empty;
                    string errorNumero = string.Empty;



                    if (clase.Consulta(0, Usuariotxt.Text, ref errorMensaje, ref errorNumero, ref tempConsultaUsuario))
                    {

                        if (tempConsultaUsuario.Rows.Count > 0)
                        {
                            if (Contrasenatxt.Text.CompareTo(tempConsultaUsuario.Rows[0][0].ToString().TrimEnd()) == 0)
                            {

                                Puede_Cerrar = true;
                                this.Close();

                            }
                            else
                            {
                                MessageBox.Show(string.Concat("Usuario o clave Mal Ingresada"));
                                Contrasenatxt.Clear();
                                Contrasenatxt.Focus();
                            }
                        }
                        else
                        {
                            MessageBox.Show(string.Concat("Usuario o clave Mal Ingresada"));
                            Contrasenatxt.Clear();
                            Contrasenatxt.Focus();
                        }
                    }


                }
                else
                {
                    MessageBox.Show(string.Concat("Por favor Ingresar Clave de Acceso"));
                    Contrasenatxt.Focus();
                }
            }
            else
            {
                MessageBox.Show(string.Concat("Por favor Ingresar Usuario"));
                Usuariotxt.Focus();
            }
        }

        private void cerrarMensaje()
        {
            CloseTimer.Enabled = false;
            Tiempo_Cerrar = Marcador_Asistencia.Properties.Settings.Default.DialogCloseTime;
            Usuariotxt.Text = "";
            Contrasenatxt.Text = "";
            ValidarCerrarAplicacion.Visible = false;
            Codigotxt.Enabled = true;
            Marcarbtn.Enabled = true;
        }

        private void abrirMensaje()
        {
            CloseTimer.Enabled = true;
            ValidarCerrarAplicacion.Visible = true;
            Codigotxt.Enabled = false;
            Marcarbtn.Enabled = false;            
        }

        public static string Obtener_DireccionIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        #endregion

        #region Eventos
        public Marcador()
        {
            InitializeComponent();

            Contrasenatxt.CharacterCasing = CharacterCasing.Lower;

            #region Usar_Camara
            try
            {
                #region Inicializar_Dispositivos_Camara

                webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);//Obtiene los dispositivos de video

                foreach (FilterInfo VideoCaptureDevice in webcam)//Recorre los dispositivos encontrados
                {
                    DispositivosVideocbx.Items.Add(VideoCaptureDevice.Name);//Muestra en el combobox los dispositivos encontrados
                }

                cam = new VideoCaptureDevice();//Inicializa la cámara

                DispositivosVideocbx.SelectedIndex = 0;//Selecciona de forma predeterminada el primer dispositivo encontrado

                #endregion

                
            }
            catch
            {
                Fotopbx.Image = null;//Si no pudo prender la cámara muestra una imagen en blanco
                CamaraDisponiblelbl.Visible = true;
            }
            #endregion

            Obtener_Tiempo_Servidor();//Obtiene la hora del servidor y la establece por primera vez

            #region Establece_Fecha_Hora_Formulario

            Fechalbl.Text = ServerTime.ToString("dd/MM/yyyy");//Obtiene la fecha actual del servidor en formato Día/Mes/Año
            Tiempolbl.Text = ServerTime.ToString("HH:mm:ss");//Obtiene la hora exacta del servidor en formato Horas:Minutos:Segundos

            #endregion
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Prender_Camara();//Intenta prender la cámara al momento de cargar el formulario
            }
            catch
            {
                CamaraDisponiblelbl.Visible = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            abrirMensaje();

            if (Puede_Cerrar)
            {
                if (cam != null && cam.IsRunning)
                    cam.Stop();//Detiene la cámara en cuanto se cierra el formulario

                Fotopbx.Image = null;//Vacía la imagen en el picturebox Fotopbx
                Fotopbx.Invalidate();//Invalida cualquier acción externa y vuelve a cargar la imagen en el picturebox
            }
            else
            {
                e.Cancel = true;
            } 
        }

        private void Datetimer_Tick(object sender, EventArgs e)
        {
            Obtener_Tiempo_Servidor();//Actualiza la variable "ServerTime" con la hora del servidor
            Tiempolbl.Text = ServerTime.ToString("HH:mm:ss");//Muestra la hora según el servidor
            Fechalbl.Text = ServerTime.ToString("dd/MM/yyyy");//Muestra la fecha según el servidor

            if (cam.IsRunning && Fotopbx.Image == null)
            {
                CamaraDisponiblelbl.Text = "Cargando...";
                CamaraDisponiblelbl.Visible = true;
            }
            else
            {
                CamaraDisponiblelbl.Text = "No disponible";
                CamaraDisponiblelbl.Visible = false;
            }

            if (Tiempo_Cerrar <= 0)
            {
                cerrarMensaje();
            }
        }

        private void Marcarbtn_Click(object sender, EventArgs e)
        {
            if (Codigotxt.Text != "" && Codigotxt.Text.Length >= 8 && Nombreselbl.Visible && ApelPateelbl.Visible && ApelMateelbl.Visible)//Valida si el campo Trabajadortxt tiene un valor distinto a nulo y con un tamaño mayor o igual a 8
            {
                #region Declarar_Inicializar_Variables
                ClassEmpleado clase = ClassEmpleado.ConseguirInstancia();//Declara la variable clase de ClaseEmpleado
                //Mensaje y número de error
                string mensajeErrorBase = string.Empty;
                string numeroError = string.Empty;
                string cod = Codigotxt.Text.Trim();//Obtiene el código del trabajador sin espacios al inicio o al final
                DataTable dataTale = new DataTable();//Crea la tabla donde se guardaron los resultados
                #endregion

                clase.ValidaMarcacion(cod, Nombre_Usuario, Nombre_PC, Direccion_IP, ref mensajeErrorBase, ref numeroError, ref dataTale);//Función para validar el ingreso mediante un procedimiento almacenado

                #region Muestra_Resultados_Toma_Foto
                if (dataTale.Rows.Count > 0)//Valida si encontró respuesta válida
                {
                    Completar_Datos(dataTale.Rows[0]);//Completa los datos en sus respectivos campos
                    Tomar_Foto(dataTale.Rows[0]["CO_TRAB"].ToString() + "(" + ServerTime.ToString("yyyy-MM-dd HH.mm.ss") + ")");//Toma la foto en el picturebox Fotopbx
                }
                else
                {
                    Limpiar_Valores_No_Datos_Personales();//Si no encontró respuesta limpia los campos
                    Respuestaelbl.Text = "Horario no programado";//Muestra la respuesta "No hay registros" debido a que no obtuvo una respuesta válida
                    Respuesta2elbl.Text = "Día de descanso";//Muestra la respuesta "No hay registros" debido a que no obtuvo una respuesta válida
                    Respuesta2elbl.ForeColor = Color.White;
                    Comentarioelbl.Text = "Comunicarse con su jefatura.";//Añade una sugerencia al empleado que llegó
                    HoraMarcacionelbl.Text = ServerTime.ToString("HH:mm");
                    Comentarioelbl.Visible = true;
                    Respuesta2elbl.Visible = true;
                    HoraMarcacionelbl.Visible = true;
                }
                #endregion
            }
        }

        private void Codigotxt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && Fotopbx.Image != null)
            {
                if (Codigotxt.Text.Length >= 8)//Valida que el campo del código del trabajador tenga por lo menos 8 caracteres
                {
                    #region Declarar_Inicializar_Valores
                    ClassEmpleado clase = ClassEmpleado.ConseguirInstancia();//Declara la variable clase de ClaseEmpleado
                    string cod = Codigotxt.Text.Trim();//Obtiene el código del trabajador sin espacios al inicio o al final
                    //Mensaje y número de error
                    string mensajeErrorBase = string.Empty;
                    string numeroError = string.Empty;
                    DataTable dataTale = new DataTable();//Crea la tabla donde se guardaron los resultados
                    #endregion

                    clase.ObtenerDatosPersonales(cod, ref mensajeErrorBase, ref numeroError, ref dataTale);//Función para obtener datos personales del Empleado con el código de trabajador correspondiente

                    #region Carga_Resultados
                    if (dataTale.Rows.Count > 0)//Valida si encontró respuesta
                    {
                        DataRow row = dataTale.Rows[0];//Obtiene la primera fila de los resultados obtenidos

                        Cargar_Foto(row);//Carga la foto del registro encontrado

                        Nombreselbl.Text = row["NO_TRAB"].ToString();//Muestra el nombre del trabajador encontrado
                        ApelPateelbl.Text = row["NO_APEL_PATE"].ToString();//Muestra el apellido paterno del trabajador encontrado
                        ApelMateelbl.Text = row["NO_APEL_MATE"].ToString();//Muestra el apellido materno del trabajador encontrado

                        Nombreselbl.Visible = true;
                        ApelPateelbl.Visible = true;
                        ApelMateelbl.Visible = true;

                        Limpiar_Valores_No_Datos_Personales();
                    }
                    else
                    {
                        Limpiar_Valores();//Sino encuentra registros con el código de trabajador limpia los campos
                    }
                    #endregion
                }
                else
                {
                    Limpiar_Valores();
                }
            }
        }

        private void Codigotxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void BtnYes_Click(object sender, EventArgs e)
        {
            validarusuario();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            cerrarMensaje();
        }

        private void CloseTimer_Tick(object sender, EventArgs e)
        {
            if (ValidarCerrarAplicacion.Visible)
            {
                Tiempo_Cerrar--;
            }
        }

        #endregion

    }
}
