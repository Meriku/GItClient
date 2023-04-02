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
        public TreeNode<T> Head { get; set; }

        public Dictionary<string, TreeNode<T>> AllNodes { get; set; }
        public Dictionary<int, List<TreeNode<T>>> AllNodesByGeneration { get; set; }


        public Tree() 
        {
            AllNodes = new Dictionary<string, TreeNode<T>>();
            AllNodesByGeneration = new Dictionary<int, List<TreeNode<T>>>();
        }

        public void Add(T data)
        {
            var node = new TreeNode<T>(data);
            AllNodes[node.GetHash()] = node;

            if (Head == null)
            {
                Head = node;
                node.Generation = 0;
                return;
            }
            else
            {
                var parentHashes = node.GetParentHashes();
                foreach (var hash in parentHashes)
                {
                    var parent = AllNodes[hash];
                    node.AddParent(parent);
                    parent.AddChild(node);
                }
            }
        }

        public void CalculateGenerations()
        {
            foreach (var node in AllNodes.Values)
            {
                node.CalculateGeneration();
                if (!AllNodesByGeneration.ContainsKey(node.Generation))
                {
                    AllNodesByGeneration[node.Generation] = new List<TreeNode<T>>();
                }

                AllNodesByGeneration[node.Generation].Add(node);
            }
        }
    }



    public class TreeNode<T> where T : IGetHash, IGetParentHashes
    {

        public T Data { get; set; }

        public List<TreeNode<T>> Children { get; set; }
        public List<TreeNode<T>> Parents { get; set; }

        public int Generation { get; set; }

        public TreeNode(T data)
        {
            Data = data;
            Children = new List<TreeNode<T>>();
            Parents = new List<TreeNode<T>>();
        }

        public void AddChild(TreeNode<T> node)
        {
            Children.Add(node);
        }
        public void AddParent(TreeNode<T> node)
        {
            Parents.Add(node);
        }
        public void CalculateGeneration()
        {
            Generation = Parents.Count == 0 ? 0 : Parents.Select(x => x.Generation).Max() + 1;
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