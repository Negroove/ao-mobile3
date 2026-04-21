using ContactosApi.Models;
using ContactosApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContactosApi.Controllers;

[ApiController]
[Route("api/contacto")]
// Solo usuarios con rol Admin pueden usar este controller.
[Authorize(Roles = "Admin")]
public class ContactosController(ContactoService contactoService) : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult<Contacto> ObtenerPorId(int id)
    {
        var contacto = contactoService.ObtenerPorId(id);

        if (contacto is null)
        {
            return NotFound($"No se encontro el contacto con id {id}.");
        }

        return Ok(contacto);
    }

    [HttpPost("add")]
    public ActionResult<Contacto> Crear(Contacto contacto)
    {
        var nuevoContacto = contactoService.Crear(contacto);

        // Devuelve 201 e informa como recuperar el recurso creado.
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoContacto.Id }, nuevoContacto);
    }

    [HttpPut("edit/{id:int}")]
    public ActionResult Editar(int id, Contacto contacto)
    {
        var editado = contactoService.Editar(id, contacto);

        if (!editado)
        {
            return NotFound($"No se encontro el contacto con id {id}.");
        }

        // 204 indica que la edicion fue correcta y no hace falta devolver body.
        return NoContent();
    }

    [HttpDelete("delete/{id:int}")]
    public ActionResult Eliminar(int id)
    {
        var eliminado = contactoService.Eliminar(id);

        if (!eliminado)
        {
            return NotFound($"No se encontro el contacto con id {id}.");
        }

        return NoContent();
    }
}
