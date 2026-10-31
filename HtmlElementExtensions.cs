namespace פרקטיקוד_2;

public static class HtmlElementExtensions
{
    // כל האלמנטים בעץ שעונים לסלקטור. HashSet מונע כפילויות
    public static HashSet<HtmlElement> Query(this HtmlElement element, Selector selector)
    {
        var result = new HashSet<HtmlElement>();
        Query(element, selector, result);
        return result;
    }

    private static void Query(HtmlElement element, Selector selector, HashSet<HtmlElement> result)
    {
        // הצאצאים של האלמנט (בלי האלמנט עצמו) שעונים לרמה הנוכחית
        var matches = element.Descendants().Skip(1).Where(selector.Matches);

        // תנאי עצירה: הגענו לסלקטור האחרון
        if (selector.Child == null)
        {
            result.UnionWith(matches);
            return;
        }

        foreach (var match in matches)
            Query(match, selector.Child, result);
    }
}
