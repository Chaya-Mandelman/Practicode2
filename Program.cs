using System.Text.RegularExpressions;
using פרקטיקוד_2;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var url = args.Length > 0 ? args[0] : "http://info.cern.ch/hypertext/WWW/TheProject.html";

// קריאה לדף אינטרנט
HttpClient client = new HttpClient();
var response = await client.GetAsync(url);
var html = await response.Content.ReadAsStringAsync();

// פירוק לפי תגיות: תגית מתחילה ב-< ומסתיימת ב->. הסוגריים בביטוי שומרים את התגית עצמה בתוצאה
// ניקוי: רצף רווחים / ירידות שורה הופך לרווח אחד, ומחרוזות ריקות נמחקות
var parts = Regex.Split(html, "(<[^>]*>)")
    .Select(part => Regex.Replace(part, @"\s+", "").Trim())
    .Where(part => part.Length > 0)
    .ToList();

// בניית העץ בלולאה (ולא בריקורסיה, כדי לא לקבל Stack Overflow ב-HTML גדול)
var root = new HtmlElement { Name = "root" };
var current = root;

// attribute: שם, ואחריו אופציונלית = וערך במרכאות כפולות / בודדות / בלי מרכאות
var attributeRegex = new Regex(@"([^\s=/]+)(?:\s*=\s*(?:""([^""]*)""|'([^']*)'|([^\s""']+)))?");

foreach (var part in parts)
{
    // טקסט פנימי של האלמנט הנוכחי
    if (!part.StartsWith('<'))
    {
        current.InnerHtml = current.InnerHtml.Length == 0 ? part : current.InnerHtml + " " + part;
        continue;
    }

    // הסרת < ו-> וחיתוך המילה הראשונה
    var tag = part.Substring(1, part.Length - 2).Trim();
    var firstWord = tag.Split(' ')[0];

    // סוף ה-html
    if (firstWord == "/html")
        break;

    // תגית סוגרת - עולים לרמה הקודמת בעץ
    if (firstWord.StartsWith('/'))
    {
        current = current.Parent ?? root;
        continue;
    }

    var name = firstWord.TrimEnd('/').ToLower();

    // לא תגית HTML מוכרת (למשל <!DOCTYPE html> או הערה <!-- -->) - מדלגים
    if (!HtmlHelper.Instance.HtmlTags.Contains(name))
        continue;

    var element = new HtmlElement { Name = name, Parent = current };
    current.Children.Add(element);

    // פירוק המשך המחרוזת (בלי המילה הראשונה) ל-Attributes
    var rest = tag.Substring(firstWord.Length);
    foreach (Match match in attributeRegex.Matches(rest))
    {
        var attrName = match.Groups[1].Value;
        var attrValue = match.Groups[2].Success ? match.Groups[2].Value
                      : match.Groups[3].Success ? match.Groups[3].Value
                      : match.Groups[4].Value;

        element.Attributes.Add($"{attrName}=\"{attrValue}\"");

        if (attrName.ToLower() == "class")
            element.Classes = attrValue.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        else if (attrName.ToLower() == "id")
            element.Id = attrValue;
    }

    // תגית שסוגרת את עצמה - האלמנט הנוכחי נשאר כמו שהוא
    // אחרת יש תגית סגירה נפרדת, ולכן האלמנט החדש הופך לנוכחי
    bool isSelfClosing = tag.EndsWith('/') || HtmlHelper.Instance.HtmlVoidTags.Contains(name);
    if (!isSelfClosing)
        current = element;
}



// שאילתות לדוגמה. אפשר להשוות בדפדפן עם $$("...") ב-console
foreach (var query in new[] { "a", "dl dt a", "h1" })
{
    var found = root.Query(Selector.Parse(query));
    Console.WriteLine($"\n\"{query}\" - {found.Count} results:");
    foreach (var element in found)
        Console.WriteLine($"  {element.Name} {string.Join(" ", element.Attributes)} {element.InnerHtml}");
}

