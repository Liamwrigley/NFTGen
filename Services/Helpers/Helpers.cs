namespace NFTGenApi.Services.Helpers;

public static class Helpers
{
    public static decimal IntToDecimal(decimal value) => value switch
    {
        < 0 => -1M,
        0 => 0M,
        > 1 => decimal.Divide(value, 100),
        _ => 0M
    };
}
