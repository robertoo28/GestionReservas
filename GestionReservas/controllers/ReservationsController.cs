using GestionReservas.data;
using GestionReservas.model;
using Microsoft.AspNetCore.Mvc;

namespace GestionReservas.controllers;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly SoapClient _soapClient;

    public ReservationsController(AppDbContext context, SoapClient soapClient)
    {
        _context = context;
        _soapClient = soapClient;
    }

    // POST /reservations
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
    {
        // Llamar al servicio SOAP para verificar disponibilidad
        bool isAvailable = await _soapClient.CheckAvailability(
            reservation.start_date,
            reservation.end_date,
            "Deluxe"
        );

        if (!isAvailable)
        {
            return BadRequest("No hay disponibilidad para las fechas seleccionadas.");
        }

        // Configurar campos adicionales
        reservation.status = "Active";

        // Registrar la reserva
        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetReservation), new { id = reservation.reservation_id }, reservation);
    }

    // GET /reservations/{id}
    [HttpGet("{id}")]
    public IActionResult GetReservation(int id)
    {
        var reservation = _context.Reservations.Find(id);

        if (reservation == null)
        {
            return NotFound("Reserva no encontrada.");
        }

        return Ok(reservation);
    }

    // DELETE /reservations/{id}
    [HttpDelete("{id}")]
    public IActionResult CancelReservation(int id)
    {
        var reservation = _context.Reservations.Find(id);

        if (reservation == null)
        {
            return NotFound("Reserva no encontrada.");
        }

        _context.Reservations.Remove(reservation);
        _context.SaveChanges();

        return NoContent();
    }
}
