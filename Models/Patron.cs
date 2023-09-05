using System.ComponentModel.DataAnnotations;

namespace LoncotesLibrary.Models;

public class Patron
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Address { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public bool IsActive { get; set; }
    public List<Checkout> Checkouts { get; set; }
    public decimal Balance
    {
        get
        {
            if (Checkouts == null)
            {
                return 0;
            }
            else
            {
                //  Calculate total unpaid fines
                return Checkouts
                    .Where(co => !co.Paid)
                    .Sum(co => co.LateFee ?? 0);
            }
        }
    }
}