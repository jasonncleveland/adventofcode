public class CustomLinkedListNode
{
    public long Value { get; set; }
    public CustomLinkedListNode Previous { get; set; }
    public CustomLinkedListNode Next { get; set; }

    public CustomLinkedListNode(long value)
    {
        Value = value;
        Previous = null;
        Next = null;
    }
}

public static class ExtensionMethods
{
    public static void AddAfter(this CustomLinkedListNode currentNode, CustomLinkedListNode newNode)
    {
        newNode.Previous = currentNode;
        newNode.Next = currentNode.Next;
        currentNode.Next.Previous = newNode;
        currentNode.Next = newNode;
    }

    public static void Remove(this CustomLinkedListNode currentNode)
    {
        currentNode.Previous.Next = currentNode.Next;
        currentNode.Next.Previous = currentNode.Previous;
    }
}