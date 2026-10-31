using System.Text.RegularExpressions;

namespace פרקטיקוד_2;

// רמה אחת בשאילתת CSS. כל רמה מצביעה על הרמה הבאה (Child) - עץ בינארי מנוון
public class Selector
{
    public string? TagName { get; set; }
    public string? Id { get; set; }
    public List<string> Classes { get; set; } = new List<string>();

    public Selector? Parent { get; set; }
    public Selector? Child { get; set; }

    // "div#main .item" => div#main -> .item
    public static Selector Parse(string query)
    {
        Selector? root = null;
        Selector? current = null;

        foreach (var level in query.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            // סלקטור חדש לכל רמה, כבן של הנוכחי
            var selector = new Selector { Parent = current };
            if (current == null)
                root = selector;
            else
                current.Child = selector;
            current = selector;

            // פירוק לפי # ו-. (ה-lookahead שומר את התו המפריד בתחילת כל חלק)
            foreach (var part in Regex.Split(level, "(?=[#.])").Where(p => p.Length > 0))
            {
                if (part.StartsWith('#'))
                    current.Id = part.Substring(1);
                else if (part.StartsWith('.'))
                    current.Classes.Add(part.Substring(1));
                else if (HtmlHelper.Instance.HtmlTags.Contains(part.ToLower()))
                    current.TagName = part.ToLower();
            }
        }

        return root ?? new Selector();
    }

    // האם האלמנט עונה לתנאי הרמה הזו
    public bool Matches(HtmlElement element) =>
        (TagName == null || element.Name == TagName) &&
        (Id == null || element.Id == Id) &&
        Classes.All(element.Classes.Contains);
}
