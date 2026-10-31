using System.Text.Json;

namespace פרקטיקוד_2;

// מחלקת עזר שמספקת את רשימות תגיות ה-HTML. Singleton - הקבצים נטענים פעם אחת בלבד
public class HtmlHelper
{
    private static readonly HtmlHelper _instance = new HtmlHelper();
    public static HtmlHelper Instance => _instance;

    // כל התגיות הקיימות ב-HTML
    public string[] HtmlTags { get; }

    // תגיות שלא דורשות תגית סגירה
    public string[] HtmlVoidTags { get; }

    private HtmlHelper()
    {
        // הקבצים מועתקים לתיקיית הפלט (CopyToOutputDirectory), לכן קוראים אותם משם
        var folder = AppContext.BaseDirectory;
        HtmlTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Combine(folder, "HtmlTags.json"))) ?? [];
        HtmlVoidTags = JsonSerializer.Deserialize<string[]>(File.ReadAllText(Path.Combine(folder, "HtmlVoidTags.json"))) ?? [];
    }
}
