using Microsoft.JSInterop;

public static class JsRuntimeExtensions
{
    public static bool IsJSRuntimeAvailable(this IJSRuntime jsRuntime)
    {
        return jsRuntime is not null && jsRuntime is not IJSInProcessRuntime;
    }
}
