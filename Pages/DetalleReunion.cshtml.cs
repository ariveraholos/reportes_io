using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ingreso_kpi_1.Pages
{
    public class DetalleReunionModel : PageModel
    {

        private IConfiguration configuracion;
        public int IdReunion;
        public string SafetyShare;
        public DataTable TablaReconocimientos;
        public int IdUnidad;

        public DetalleReunionModel(IConfiguration conf)
        {
            configuracion = conf;
        }

        public void OnGet(int id_reunion, int id_unidad)
        {
            IdReunion = id_reunion;
            IdUnidad = id_unidad;
            SafetyShare = obtieneSafetyShare(IdReunion);
            cargaReconocimientos(id_reunion);
        }

        public void OnPostSafetyShare(int id_reunion, string safety_share, int id_unidad)
        {
            IdReunion = id_reunion;
            IdUnidad = id_unidad;

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "update prueba1.reuniones set safety_share = @safety_share where id = @id_reunion";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id_reunion", id_reunion);
            cmd.Parameters.AddWithValue("@safety_share", safety_share);

            cmd.ExecuteNonQuery();

            SafetyShare = safety_share;
        }

        private string obtieneSafetyShare(int idReunion)
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select safety_share from prueba1.reuniones where id = @id_reunion";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id_reunion", idReunion);

            object resp = cmd.ExecuteScalar();

            SafetyShare = "";

            if (resp != null)
            {
                SafetyShare = (string)resp;
            }

            return SafetyShare;
        }

        private void cargaReconocimientos(int idReunion)
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select p1.nombre as quien_reconoce, p2.nombre as reconocido, r.pilar, r.motivo " 
                + " from prueba1.reconocimientos r inner join prueba1.personas p1 on r.id_quien_reconoce " + 
                " = p1.id inner join prueba1.personas p2 on r.id_reconocido = p2.id where r.id_reunion = @id_reunion";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id_reunion", idReunion);

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            TablaReconocimientos = new DataTable();

            adapter.Fill(TablaReconocimientos);
        }

        public void OnPostGrabaReconocimiento(int id_unidad, int id_reunion, int quien_reconoce, int reconocido, string pilar, string motivo)
        {
            IdReunion = id_reunion;
            IdUnidad = id_unidad;

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select max(id) from prueba1.reconocimientos";

            SqlCommand cmd = new SqlCommand(sql, con);

            object resultado = cmd.ExecuteScalar();

            int NuevoId = 1;

            if (resultado !=null)
            {
                NuevoId = (int)resultado + 1;
            }

            sql = "insert into prueba1.reconocimientos(id, id_reunion,id_quien_reconoce, id_reconocido, pilar, motivo) " +
                "values(@id,@id_reunion,@id_quien_reconoce,@id_reconocido,@pilar,@motivo)";

            cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@id", NuevoId);
            cmd.Parameters.AddWithValue("@id_reunion", id_reunion);
            cmd.Parameters.AddWithValue("@id_quien_reconoce", quien_reconoce);
            cmd.Parameters.AddWithValue("@id_reconocido", reconocido);
            cmd.Parameters.AddWithValue("@pilar", pilar);
            cmd.Parameters.AddWithValue("@motivo", motivo);

            cmd.ExecuteNonQuery();

            SafetyShare = obtieneSafetyShare(IdReunion);
            cargaReconocimientos(id_reunion);
        }
    }
}
