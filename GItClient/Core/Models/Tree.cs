using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    public class Tree<T> where T : IGetHash, IGetParentHashes
    {
        public Node<T> Head { get; set; }

        public Dictionary<string, Node<T>> AllNodes { get; set; }

        public Tree() 
        {
            AllNodes = new Dictionary<string, Node<T>>();
        }

        public void Add(T data)
        {
            var node = new Node<T>(data);
            AllNodes[node.GetHash()] = node;

            if (Head == null)
            {
                Head = node;
                return;
            }
            else
            {
                var parentHashes = node.GetParentHashes();
                foreach (var hash in parentHashes)
                {
                    AllNodes[hash].AddChild(node);
                }

            }
        }

    }

    public class Node<T> where T : IGetHash, IGetParentHashes
    {

        public T Data { get; set; }

        public List<Node<T>> Children { get; set; }

        public Node(T data)
        {
            Data = data;
            Children = new List<Node<T>>();
        }

        public void AddChild(Node<T> node)
        {
            Children.Add(node);
        }

        public string GetHash()
        {
            return Data.GetHash();
        }
        public string[] GetParentHashes()
        {
            return Data.GetParentHashes();
        }
    }
}