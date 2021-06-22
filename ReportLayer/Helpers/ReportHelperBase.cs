using System.Configuration;

public class ReportHelperBase
{
    public ReportHelperBase(string field)
    {
        Field = field;
        Template = Get("Template").ToString();
    }

    protected object Get(string key)
    {
        if (string.IsNullOrEmpty(Field))
            return ConfigurationManager.AppSettings[key];
        else
            return ConfigurationManager.AppSettings[$"{Field}.{key}"];
    }

    private string Field { get; }
    public string Template { get; }
}


