using ContactosApi.Models;

namespace ContactosApi.Services;

public class ContactoService
{
    // Lista en memoria que simula una base de datos.
    private readonly List<Contacto> _contactos =
    [
        new Contacto
        {
            Id = 1,
            Nombre = "Ada",
            Apellido = "Lovelace",
            Telefono = "11-5555-0001",
            Email = "ada@contactos.com"
        },
        new Contacto
        {
            Id = 2,
            Nombre = "Alan",
            Apellido = "Turing",
            Telefono = "11-5555-0002",
            Email = "alan@contactos.com"
        }
    ];

    public List<Contacto> ObtenerTodos()
    {
        return _contactos;
    }

    public Contacto? ObtenerPorId(int id)
    {
        return _contactos.FirstOrDefault(contacto => contacto.Id == id);
    }

    public Contacto Crear(Contacto contacto)
    {
        // Genera un Id nuevo para evitar repetir contactos.
        var nuevoId = _contactos.Count == 0 ? 1 : _contactos.Max(c => c.Id) + 1;
        contacto.Id = nuevoId;
        _contactos.Add(contacto);

        return contacto;
    }

    public bool Editar(int id, Contacto contacto)
    {
        var contactoExistente = ObtenerPorId(id);

        if (contactoExistente is null)
        {
            return false;
        }

        // Se actualizan solo los datos editables del contacto.
        contactoExistente.Nombre = contacto.Nombre;
        contactoExistente.Apellido = contacto.Apellido;
        contactoExistente.Telefono = contacto.Telefono;
        contactoExistente.Email = contacto.Email;

        return true;
    }

    public bool Eliminar(int id)
    {
        var contacto = ObtenerPorId(id);

        if (contacto is null)
        {
            return false;
        }

        _contactos.Remove(contacto);
        return true;
    }
}
