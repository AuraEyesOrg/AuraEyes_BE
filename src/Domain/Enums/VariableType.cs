namespace Domain.Enums;

/// <summary>
/// Defines the data type / UI control hint for a contract template variable.
/// </summary>
public enum VariableType
{
    /// <summary>Free-text input.</summary>
    Text = 1,

    /// <summary>Numeric value (integer or decimal).</summary>
    Number = 2,

    /// <summary>Monetary amount (formatted with currency unit).</summary>
    Currency = 3,

    /// <summary>Time value, e.g. "08:00".</summary>
    Time = 4,

    /// <summary>Date value.</summary>
    Date = 5,

    /// <summary>Predefined list of options (SelectOptions contains the allowed values).</summary>
    Select = 6,
}
