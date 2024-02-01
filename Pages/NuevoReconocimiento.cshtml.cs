using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace ingreso_kpi_1.Pages
{
    public class NuevoReconocimientoModel : PageModel
    {

        private IConfiguration configuracion;
        public int IdReunion;
        public int IdUnidad;
        public DataTable TablaPersonas;

        public NuevoReconocimientoModel(IConfiguration conf)
        {
            configuracion = conf;
        }

        public void OnGet(int id_unidad, int id_reunion)
        {
            IdUnidad = id_unidad;
            IdReunion = id_reunion;
            cargaPersonas();
        }

        private void cargaPersonas()
        {
            TablaPersonas = new DataTable();

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select id, nombre, id_unidad, rol " +
                    "from prueba1.personas where id_unidad = @id_unidad order by nombre";


            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@id_unidad", IdUnidad));

            TablaPersonas = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(TablaPersonas);
        }
    }
}
