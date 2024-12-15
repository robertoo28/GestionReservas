using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionReservas.model;

[Table("reservations")]
public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int reservation_id { get; set; }

    public int room_number { get; set; }
    public string customer_name { get; set; }
    public DateTime start_date { get; set; }
    public DateTime end_date { get; set; }
    public string status { get; set; } 
}