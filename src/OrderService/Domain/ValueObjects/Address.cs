using SharedKernel.Domain;

namespace OrderService.Domain.ValueObjects;

/// <summary>
/// Value Object pra endereço - dados que sempre andam juntos
/// </summary>
public class Address : ValueObject
{
    public string Street { get; }
    public string Number { get; }
    public string? Complement { get; }
    public string Neighborhood { get; }
    public string City { get; }
    public string State { get; }
    public string ZipCode { get; }
    public string Country { get; }

    public Address(
        string street,
        string number,
        string neighborhood,
        string city,
        string state,
        string zipCode,
        string country = "Brasil",
        string? complement = null)
    {
        // Validações - endereço incompleto não serve pra entregar nada
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street is required", nameof(street));
        
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("Number is required", nameof(number));
        
        if (string.IsNullOrWhiteSpace(neighborhood))
            throw new ArgumentException("Neighborhood is required", nameof(neighborhood));
        
        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required", nameof(city));
        
        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State is required", nameof(state));
        
        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode is required", nameof(zipCode));

        Street = street.Trim();
        Number = number.Trim();
        Complement = complement?.Trim();
        Neighborhood = neighborhood.Trim();
        City = city.Trim();
        State = state.Trim();
        ZipCode = zipCode.Trim().Replace("-", ""); // Remove hífen do CEP
        Country = country.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return Number;
        yield return Complement ?? string.Empty;
        yield return Neighborhood;
        yield return City;
        yield return State;
        yield return ZipCode;
        yield return Country;
    }

    public override string ToString()
    {
        var complement = string.IsNullOrWhiteSpace(Complement) ? "" : $", {Complement}";
        return $"{Street}, {Number}{complement}, {Neighborhood}, {City} - {State}, {ZipCode}, {Country}";
    }

    // Método útil pra labels de entrega
    public string ToShippingLabel()
    {
        var lines = new List<string>
        {
            $"{Street}, {Number}",
            Neighborhood,
            $"{City} - {State}",
            $"CEP: {FormatZipCode()}",
            Country
        };

        if (!string.IsNullOrWhiteSpace(Complement))
            lines.Insert(1, Complement);

        return string.Join("\n", lines);
    }

    private string FormatZipCode()
    {
        // Formata CEP brasileiro (12345678 -> 12345-678)
        if (ZipCode.Length == 8 && ZipCode.All(char.IsDigit))
            return $"{ZipCode[..5]}-{ZipCode[5..]}";
        
        return ZipCode;
    }
}