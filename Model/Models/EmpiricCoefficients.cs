namespace ModelPolosin.Models;

public record EmpiricCoefficientsToDataGrid(
    int IdEc,
    string Name,
    string Symbol,
    string? Unit,
    double Value);