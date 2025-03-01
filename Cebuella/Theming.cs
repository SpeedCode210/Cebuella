namespace Cebuella;

public static class Theming
{
    public static Theme[] Themes =
    [
        new ("https://bootswatch.com/5/lumen/bootstrap.min.css", "Lumen (Default)"),
        new ("https://bootswatch.com/5/cyborg/bootstrap.min.css", "Cyborg"),
        new ("https://bootswatch.com/5/flatly/bootstrap.min.css", "Flatly"),
        new ("https://bootswatch.com/5/solar/bootstrap.min.css", "Solar"),
        new ("https://bootswatch.com/5/vapor/bootstrap.min.css", "Vapor"),
        new ("https://bootswatch.com/5/minty/bootstrap.min.css", "Minty"),
    ];
}

public struct Theme
{
    public string Url { get; set; }
    public string Name { get; set; }

    public Theme(string url, string name)
    {
        Url = url;
        Name = name;
    }
}