using RegistroUsuarios.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace RegistroUsuarios.Infrastructure.Repositories.SQL
{
    public class PersonaRepository
    {
        private readonly string _connectionString;
        public PersonaRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Registrar(Persona persona)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                using (var tx = conn.BeginTransaction())
                {
                    try
                    {
                        // Insertar persona
                        using (var cmd = new SqlCommand("sp_RegistrarPersona", conn, tx))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@Documento", persona.DocumentoIdentidad);
                            cmd.Parameters.AddWithValue("@Nombres", persona.Nombres);
                            cmd.Parameters.AddWithValue("@Apellidos", persona.Apellidos);
                            cmd.Parameters.AddWithValue("@FechaNacimiento", persona.FechaNacimiento);
                            cmd.ExecuteNonQuery();
                        }

                        // Insertar contactos
                        InsertarContactos(conn, tx, persona.DocumentoIdentidad, 1, persona.Telefonos);
                        InsertarContactos(conn, tx, persona.DocumentoIdentidad, 2, persona.Correos);
                        InsertarContactos(conn, tx, persona.DocumentoIdentidad, 3, persona.Direcciones);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }
        private void InsertarContactos(SqlConnection conn, SqlTransaction tx,
                                       string doc, byte tipo, System.Collections.Generic.List<string> valores)
        {
            if (valores == null) return;

            foreach (var v in valores)
            {
                using (var cmd = new SqlCommand(
                    "INSERT INTO Contactos (DocumentoIdentidad, TipoContacto, Valor) VALUES (@Doc, @Tipo, @Valor)",
                    conn, tx))
                {
                    cmd.Parameters.AddWithValue("@Doc", doc);
                    cmd.Parameters.AddWithValue("@Tipo", tipo);
                    cmd.Parameters.AddWithValue("@Valor", v);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
