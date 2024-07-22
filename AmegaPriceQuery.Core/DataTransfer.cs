namespace AmegaPriceQuery.Core;

public enum ResponseCode
{
    Failed = 06,
    Successful = 00
}
public record RestResponse(object? result, string responseDescription, ResponseCode responseCode);