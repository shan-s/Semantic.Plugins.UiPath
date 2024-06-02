namespace LongreachAi.Connectors.UiPath;
internal static class Extensions
{
    internal static bool IsSuccessful(this OrchestratorResponse response)
    {
        int[] successcodes= [200, 201, 202, 203, 204];
        return successcodes.Contains(response.StatusCode);    
    }

    /// <summary>   
    ///Check if the two strings are similar
    /// </summary>
    internal static bool IsMatchOf(this string str1, string str2)
    {
        var nstr1 = new string(str1.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();
        var nstr2 = new string(str2.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant();

       return nstr1.Contains(nstr2);
    }
}