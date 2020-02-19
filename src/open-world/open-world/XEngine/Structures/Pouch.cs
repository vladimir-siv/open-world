using System.Collections.Generic;

namespace XEngine.Structures
{
	internal sealed class Pouch<TKey, TValue>
	{
		private class Node
		{
			public TValue Value;
			public Node Next;
			public Node() { }
			public Node(TValue value, Node next = null) { Value = value; Next = next; }
		}
		private class NodePooler
		{
			private Stack<Node> Pool = new Stack<Node>();
			public void Release(Node node) => Pool.Push(node);
			public Node Create(TValue value, Node next = null)
			{
				Node node;
				if (Pool.Count > 0) node = Pool.Pop();
				else node = new Node();
				node.Value = value;
				node.Next = next;
				return node;
			}
		}

		private Dictionary<TKey, Node> Collection = new Dictionary<TKey, Node>();
		private NodePooler Nodes = new NodePooler();

		public int Count { get; private set; } = 0;

		public void Add(TKey key, TValue value)
		{
			var found = Collection.TryGetValue(key, out var node);
			if (found) node.Next = Nodes.Create(value, node.Next);
			else Collection.Add(key, Nodes.Create(value));
			++Count;
		}

		public bool Retrieve(TKey key, out TValue value)
		{
			var found = Collection.TryGetValue(key, out var node);

			if (!found)
			{
				value = default;
				return false;
			}

			if (node.Next != null)
			{
				var temp = node.Next;
				node.Next = temp.Next;
				node = temp;
			}
			else Collection.Remove(key);

			--Count;
			value = node.Value;
			Nodes.Release(node);
			return true;
		}
	}
}
