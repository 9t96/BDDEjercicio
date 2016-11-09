using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SqlClient;
using System.Data;

namespace AccesoDatos
{
    public class AccesoDatos
    {
        #region Atributos
        private SqlConnection _conexion;
        private SqlCommand _comando;
        #endregion

        #region Constructores
        public AccesoDatos() 
        {
            // CREO UN OBJETO SQLCONECTION//Parametro conexion a mi base de datos Properties.Settings.Default.CadenaConexion en este caso "segundaCadena".
            this._conexion = new SqlConnection(Properties.Settings.Default.SegundaCadena);//Permite conectarme a la base.
            // CREO UN OBJETO SQLCOMMAND
            this._comando = new SqlCommand();//Guarda los comandos, ej: insert, select , delete.
            // INDICO EL TIPO DE COMANDO
            this._comando.CommandType = System.Data.CommandType.Text;//Guarda el tipo de comando 
            // ESTABLEZCO LA CONEXION
            this._comando.Connection = this._conexion;//Asigno la conexion al comando.
        }
        #endregion

        #region Métodos
        public List<Persona> ObtenerListaPersonas()
        {
            bool TodoOk = false;
            List<Persona> lista = new List<Persona>();

            try
            {
                // LE PASO LA INSTRUCCION SQL//Indico la instruccion.
                _comando.CommandText = "SELECT * FROM Personas ORDER BY apellido, nombre";

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND  //Apuntan y solo se puede leer ese objeto.               
                SqlDataReader oDr = _comando.ExecuteReader();

                // MIENTRAS TENGA REGISTROS...
                while (oDr.Read()) //Leo los datos y los almaceno en la lista.
                {
                    // ACCEDO POR NOMBRE O POR INDICE
                    lista.Add(new Persona(int.Parse(oDr[0].ToString()),oDr["apellido"].ToString(), oDr["nombre"].ToString(), int.Parse(oDr[3].ToString())));                    
                }

                //CIERRO EL DATAREADER
                oDr.Close();

                TodoOk = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (TodoOk)
                    this._conexion.Close();
            }
            return lista;
        }


        public DataTable ObtenerTablaPersonas()
        {
            bool TodoOk = false;
            DataTable tabla = new DataTable("Personas");//Nombre tabla en la base.

            try
            {
                // INDICO EL TIPO DE COMANDO
                this._comando.CommandType = System.Data.CommandType.Text;
                // LE PASO LA INSTRUCCION SQL
                this._comando.CommandText = "SELECT * FROM Personas ORDER BY apellido DESC, nombre";

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND                 
                SqlDataReader oDr = this._comando.ExecuteReader();

                // CARGO LA TABLA CON REGISTROS...
                tabla.Load(oDr);

                //CIERRO EL DATAREADER
                oDr.Close();

                TodoOk = true;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                if (TodoOk)
                    this._conexion.Close();
            }
            return tabla;
        }

        /// <summary>
        /// Recibe la persona a insertar y la inserta por comando
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool InsertarPersona(Persona p)
        {
            bool todoOk = false;

            //Comando para insertar.
            string sql = "INSERT INTO Personas (nombre, apellido, edad) VALUES(";
            sql = sql + "'" + p.Nombre + "','" + p.Apellido + "'," + p.Edad.ToString() + ")";

            try
            {
                // LE PASO LA INSTRUCCION SQL
                this._comando.CommandText = sql;

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND 
                this._comando.ExecuteNonQuery();

                todoOk = true;

            }
            catch (Exception)
            {
                todoOk = false;
            }
            finally
            {
                if (todoOk)
                    this._conexion.Close();               
            }
            return todoOk;
 
        }
        /// <summary>
        /// REciebe un apellido por parametro y lo busca en la base y devuleve el objeto.
        /// </summary>
        /// <param name="apellido"></param>
        /// <returns></returns>
        public Persona ObtenerPersonaPorApellido(string apellido)
        {
            bool TodoOk = false;
            Persona p = null;

            try
            {
                // LE PASO LA INSTRUCCION SQL
                this._comando.CommandText = "SELECT * FROM Personas WHERE apellido = '" + apellido + "'";
                // ESTABLESCO LA CONEXION
                this._comando.Connection = this._conexion;

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND                 
                SqlDataReader oDr = this._comando.ExecuteReader();

                // SI HAY REGISTROS...               
                if (oDr.Read())
                {
                    // ACCEDO POR NOMBRE O POR INDICE
                    p = new Persona(int.Parse(oDr["id"].ToString()), oDr["apellido"].ToString(),
                        oDr["nombre"].ToString(), int.Parse(oDr["edad"].ToString()));
                }
                //CIERRO EL DATAREADER
                oDr.Close();

                TodoOk = true;
            }

            catch (Exception)
            {
                TodoOk = false;
            }
            finally
            {

                if (TodoOk)
                    this._conexion.Close();

            }

            return p;
        }
        /// <summary>
        /// Recibe la persona a modificar, ya validada por ObtenerPersona. 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool ModificarPersona(Persona p)
        {
            bool todoOk = false;
            string sql = "UPDATE Personas SET nombre = '" + p.Nombre + "', apellido = '";//UPDAT comando para modificar en la base.
            sql = sql + p.Apellido + "', edad = " + p.Edad.ToString() + " WHERE id = " + p.ID.ToString();

            try
            {
                // LE PASO LA INSTRUCCION SQL
                this._comando.CommandText = sql;

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND
                this._comando.ExecuteNonQuery();

                todoOk = true;
            }
            catch (Exception)
            {
                todoOk = false;
            }
            finally
            {
                if (todoOk)
                    this._conexion.Close();
            }
            return todoOk;
        }

        public bool EliminarPersona(Persona p)
        {
            bool todoOk = false;

            string sql = "DELETE FROM Personas WHERE id = " + p.ID.ToString();

            try
            {
                // LE PASO LA INSTRUCCION SQL
                this._comando.CommandText = sql;

                // ABRO LA CONEXION A LA BD
                this._conexion.Open();

                // EJECUTO EL COMMAND
                this._comando.ExecuteNonQuery();

                todoOk = true;

            }
            catch (Exception)
            {
                todoOk = false;
            }
            finally
            {
                if (todoOk)
                    this._conexion.Close();
            }
            return todoOk;
        }
        #endregion
    }
}
