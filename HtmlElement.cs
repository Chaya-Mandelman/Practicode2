namespace פרקטיקוד_2;

// מייצגת תגית HTML אחת בעץ האוביקטים
public class HtmlElement
{
    public string? Id { get; set; }
    public string Name { get; set; } = "";
    public List<string> Attributes { get; set; } = new List<string>();
    public List<string> Classes { get; set; } = new List<string>();
    public string InnerHtml { get; set; } = "";

    public HtmlElement? Parent { get; set; }
    public List<HtmlElement> Children { get; set; } = new List<HtmlElement>();

    // האלמנט וכל צאצאיו מכל הדורות. עם תור ולא ריקורסיה, כדי לא לתקוע את ה-Stack
    public IEnumerable<HtmlElement> Descendants()
    {
        var queue = new Queue<HtmlElement>();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var element = queue.Dequeue();
            yield return element;

            foreach (var child in element.Children)
                queue.Enqueue(child);
        }
    }

    // כל האבות של האלמנט, מהאבא ועד השורש
    public IEnumerable<HtmlElement> Ancestors()
    {
        for (var parent = Parent; parent != null; parent = parent.Parent)
            yield return parent;
    }
}
