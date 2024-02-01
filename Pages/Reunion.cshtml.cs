using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace ingreso_kpi_1.Pages
{
    public class ReunionModel : PageModel
    {
        private IConfiguration configuracion;
        public DataTable TablaUnidades;
        public int IdUnidad;
        public DataTable TablaReuniones;

        public ReunionModel(IConfiguration conf)
        {
            configuracion = conf;
        }

        private void cargaUnidades()
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select id, gerencia_general, gerencia, superintendencia, nivel " +
                    "from prueba1.unidades where gerencia <> '-' order by gerencia_general, gerencia, superintendencia";

            TablaUnidades = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(sql, con);

            adapter.Fill(TablaUnidades);
        }

        public void OnGet()
        {
            cargaUnidades();
        }

        public void OnPostSelUnidad(int id_unidad)
        {
            IdUnidad = id_unidad;
            cargaUnidades();
            cargaReuniones(IdUnidad);
        }

        public void OnPostNuevaReunion(int id_unidad, string tipo_reunion, DateTime fecha)
        {
            IdUnidad = id_unidad;

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select max(id) from prueba1.reuniones";

            SqlCommand cmd = new SqlCommand(sql, con);

            object respuesta = cmd.ExecuteScalar();

            int id = 1;

            if (respuesta != null)
            {
                id = (int)respuesta + 1;
            }

            sql = "insert into prueba1.reuniones(id, tipo, id_unidad, fecha) values(@id, @tipo, @id_unidad, @fecha)";

            cmd = new SqlCommand(sql, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@tipo", tipo_reunion);
            cmd.Parameters.AddWithValue("@id_unidad", id_unidad);
            cmd.Parameters.AddWithValue("@fecha", fecha);

            cmd.ExecuteNonQuery();

            cargaUnidades();
            cargaReuniones(IdUnidad);

        }




        public void cargaReuniones(int IdUnidad)
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select id, fecha, tipo, estado, actual " +
                    "from prueba1.reuniones where id_unidad = @id_unidad";


            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@id_unidad",IdUnidad));

            TablaReuniones = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(TablaReuniones);
        }
    }
}
