using System.Text.RegularExpressions;
using RegistroUsuarios.Domain;
using RegistroUsuarios.Domain.Interfaces;
using RegistroUsuarios.Infrastructure.Repositories.SQL;

namespace RegistroPersonas.Negocio
{
    public class PersonaService : IRegistrarPersona
    {
        private readonly PersonaRepository _repo;

        public PersonaService(PersonaRepository repo)
        {
            _repo = repo;
        }

        public void RegistrarPersona(Persona p)
        {
            Validar(p);
            _repo.Registrar(p);
        }

        private void Validar(Persona p)
        {
            // II. Obligatorios
            if (string.IsNullOrWhiteSpace(p.DocumentoIdentidad))
                throw new ArgumentException("El documento de identidad es obligatorio.");
            if (string.IsNullOrWhiteSpace(p.Nombres))
                throw new ArgumentException("Los nombres son obligatorios.");
            if (string.IsNullOrWhiteSpace(p.Apellidos))
                throw new ArgumentException("Los apellidos son obligatorios.");
            if (p.FechaNacimiento == default)
                throw new ArgumentException("La fecha de nacimiento es obligatoria.");

            // II. Documento alfanumérico
            if (!Regex.IsMatch(p.DocumentoIdentidad, @"^[a-zA-Z0-9]+$"))
                throw new ArgumentException("El documento solo acepta valores alfanuméricos.");

            // II. Nombres/apellidos solo letras del alfabeto latino (incluye acentos y ñ)
            var regexAlfabeto = new Regex(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$");
            if (!regexAlfabeto.IsMatch(p.Nombres))
                throw new ArgumentException("Los nombres solo aceptan letras del alfabeto latino.");
            if (!regexAlfabeto.IsMatch(p.Apellidos))
                throw new ArgumentException("Los apellidos solo aceptan letras del alfabeto latino.");

            // III. Al menos un correo o una dirección física
            bool tieneCorreo = p.Correos != null && p.Correos.Any(c => !string.IsNullOrWhiteSpace(c));
            bool tieneDireccion = p.Direcciones != null && p.Direcciones.Any(d => !string.IsNullOrWhiteSpace(d));
            if (!tieneCorreo && !tieneDireccion)
                throw new ArgumentException("Debe registrar al menos un correo o una dirección física.");

            // III. Máximos
            if (p.Telefonos != null && p.Telefonos.Count > 2)
                throw new ArgumentException("Máximo 2 números telefónicos.");
            if (p.Correos != null && p.Correos.Count > 2)
                throw new ArgumentException("Máximo 2 correos electrónicos.");
            if (p.Direcciones != null && p.Direcciones.Count > 2)
                throw new ArgumentException("Máximo 2 direcciones físicas.");
        }
    }
}
