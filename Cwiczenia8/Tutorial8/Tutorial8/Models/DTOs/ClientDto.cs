using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.DotNet.Scaffolding.Shared.Project;

namespace Tutorial8.Models.DTOs;

public class ClientDto
{
    public int IdClient { get; set; }
    
    [StringLength(120)]
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Telephone { get; set; }
    public string Pesel { get; set; }

    public static bool ValidateEmail(string inputEmail)
    {
        if (inputEmail.Equals(null)) return false;
        
        string emailPattern = "^[a-zA-Z0-9.-]+@[a-zA-Z]+\\.[a-zA-Z]{2,}$";
        Regex emailRegex = new Regex(emailPattern);

        if (!emailRegex.IsMatch(inputEmail)) return false;
        
        return true;
    }

    public static bool ValidateFirstName(string inputFirstName)
    {
        if (inputFirstName.Equals(null)) return false;
        if (inputFirstName.Contains(';')) return false;

        return true;
    }
    
    public static bool ValidateLastName(string inputLastName)
    {
        if (inputLastName.Equals(null)) return false;
        if (inputLastName.Contains(';')) return false;

        return true;
    }

    public static bool ValidateTelephone(string inputTelephoneNumber)
    {
        if (inputTelephoneNumber.Equals(null)) return false;
        if (inputTelephoneNumber.Length != 11) return false;

        return true;
    }

    public static bool ValidatePesel(string inputPesel)
    {
        if (inputPesel.Equals(null)) return false;
        if (inputPesel.Length != 11) return false;

        // Jeśli ma długość równą 11 to sprawdza, czy inputPesel to faktycznie liczba
        int peselInt = -1;
        if (Int32.TryParse(inputPesel, out peselInt) == false) return false;
        
        int peselSum = 0;
        for (int i = 0; i < 11; i++)
        {
            int digit = Int32.Parse(inputPesel[i].ToString());
            
            if (i == 0 || i == 4 || i == 8 || i == 10) 
                peselSum += 1 * digit;
            else if (i == 1 || i == 5 || i == 9)
                peselSum += 3 * digit;
            else if (i == 2 || i == 6)
                peselSum += 7 * digit;
            else if (i == 3 || i == 7)
                peselSum += 9 * digit;
        }

        // Ostatnia cyfra sumy powinna być równa zeru
        int lastSumDigit = peselSum % 10;
        if (lastSumDigit != 0) return false;
        
        return true;
    }
}