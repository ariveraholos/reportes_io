using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace ingreso_kpi_1.Pages
{
    public class IngresoKPIModel : PageModel
    {
        private IConfiguration configuracion;

        public DataTable tablaKPI;
        public DataTable tablaSemanas;
        public DataTable tablaValores;
        public DataTable tablaUnidades;

        public int idKPI = 0;
        public int idUnidad = 0;

        public IngresoKPIModel(IConfiguration conf)
        {
            configuracion = conf;
        }

        public void OnGet()
        {
            cargaControles();
        }

        public void OnPostSelKPI(int Unidad, int kpi) 
        {
            cargaControles();
            cargaValores(kpi,Unidad);
            this.idUnidad = Unidad;
            this.idKPI = kpi;
        }

        public void OnPostSelUnidad(int Unidad)
        {
            cargaControles();
            this.idUnidad = Unidad;
            this.idKPI = 0;
        }


        public void OnPostCargaValor(int kpi, int valor, DateTime fecha, int unidad)
        {

            grabaValor(kpi,fecha,valor,unidad);
            cargaControles();
            cargaValores(kpi,unidad);
            this.idKPI = kpi;
            this.idUnidad= unidad;
        }

        private void grabaValor(int kpi, DateTime fecha, int valor, int unidad)
        {

            int semana = obtieneIdSemana(fecha);

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select max(id) from prueba1.valores_semanales";

            SqlCommand cmd = new SqlCommand(sql, con);

            int idNuevo = 1;

            try
            {
                idNuevo = (int)cmd.ExecuteScalar()+1;
            }
            catch (Exception e)
            {
                idNuevo = 1;
            }

            // Buscando si ya hay ingresado un valor

            sql = "select id from prueba1.valores_semanales where id_semana = @id_semana and id_kpi = @id_kpi and id_unidad = @unidad ";

            cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@id_semana", semana));
            cmd.Parameters.Add(new SqlParameter("@id_kpi", kpi));
            cmd.Parameters.Add(new SqlParameter("@unidad", unidad));

            var respuesta = cmd.ExecuteScalar();

            if (respuesta == null)
            {
                sql = "insert into prueba1.valores_semanales(id,id_semana,id_kpi,actual, id_unidad) values(@id,@id_semana,@id_kpi,@actual,@unidad)";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("@id", idNuevo));
                cmd.Parameters.Add(new SqlParameter("@id_semana", semana));
                cmd.Parameters.Add(new SqlParameter("@id_kpi", kpi));
                cmd.Parameters.Add(new SqlParameter("@actual", valor));
                cmd.Parameters.Add(new SqlParameter("@unidad", unidad));

                cmd.ExecuteNonQuery();
            }
            else
            {
                sql = "update prueba1.valores_semanales set actual = @actual where id = @id";

                cmd = new SqlCommand(sql, con);
                cmd.Parameters.Add(new SqlParameter("@id", respuesta));
                cmd.Parameters.Add(new SqlParameter("@actual", valor));

                cmd.ExecuteNonQuery();
            }



        }

        private void cargaValores(int idKPI, int unidad)
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql;

            sql = "select prueba1.kpi.nombre, vs.id, vs.id_semana, s.inicio_semana, vs.actual, prueba1.unidades.gerencia, prueba1.unidades.superintendencia " +
                "from prueba1.valores_semanales vs inner join prueba1.semanas s " +
                "on vs.id_semana = s.id inner join prueba1.kpi on vs.id_kpi = prueba1.kpi.id " + 
                "inner join prueba1.unidades on prueba1.unidades.id = vs.id_unidad " +
                "where vs.id_kpi = @id_kpi and vs.id_unidad = @unidad order by s.inicio_semana";


            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.Parameters.Add(new SqlParameter("@id_kpi", idKPI));
            cmd.Parameters.Add(new SqlParameter("@unidad", unidad));

            tablaValores = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            adapter.Fill(tablaValores);
        }

        private void cargaControles()
        {
            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select id, nombre from prueba1.kpi order by nombre";

            SqlDataAdapter adapter = new SqlDataAdapter(sql, con);

            tablaKPI = new DataTable();

            adapter.Fill(tablaKPI);

            sql = "select id, inicio_semana from prueba1.semanas";

            tablaSemanas = new DataTable();

            adapter = new SqlDataAdapter(sql, con);

            adapter.Fill(tablaSemanas);

            sql = "select id, gerencia_general, gerencia, superintendencia, nivel " +
                    "from prueba1.unidades where gerencia <> '-' order by gerencia_general, gerencia, superintendencia";

            tablaUnidades = new DataTable();

            adapter = new SqlDataAdapter(sql, con);

            adapter.Fill(tablaUnidades);
        }

        private int obtieneIdSemana(DateTime fecha)
        {
            int diff = fecha.DayOfWeek - DayOfWeek.Monday;

            if (diff < 0)
            {
                diff += 7;
            }


            DateTime inicioSemana = fecha.AddDays(-1 * diff).Date;

            string con_str = configuracion["con_str"];

            SqlConnection con = new SqlConnection(con_str);
            con.Open();

            string sql = "select id from prueba1.semanas where inicio_semana = @inicio_semana";

            SqlCommand cmd = new SqlCommand(sql, con);

            cmd.Parameters.Add(new SqlParameter("@inicio_semana", inicioSemana));

            object id = cmd.ExecuteScalar();

            if (id != null)
            {
                return (int)id;
            }

            return 0;
        }
    }
}
